namespace _Scripts.Core.NPC
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AI;
    using BuildSystem;
    using Cysharp.Threading.Tasks;
    using global::Zenject;
    using UnityEngine;
    using Utils.Debugging;
    using Utils;

    public class BuildersManager : MonoBehaviour, IDispatchable, ICreatable, ICountable
    {
        [SerializeField] private BuilderFSM _builderPrefab;
        
        [SerializeField] private float _buildingsCheckFrequency = 2f;
        
        public int Count => _availableBuilders.Count;
        
        private List<BuildingConstructionScript> _constructionsActiveList = new List<BuildingConstructionScript>();
        private List<BuildingConstructionScript> _constructionsPendingList = new List<BuildingConstructionScript>();

        private HashSet<BuilderFSM> _availableBuilders = new HashSet<BuilderFSM>();

        private Dictionary<BuildingConstructionScript, List<BuilderFSM>> _activeSitesBuildersMap = new();

        public event Action<BuildingConstructionScript> OnConstructionBuilt;

        [Inject(Id = "BuildersParent")] private Transform _buildersParent;
        [Inject] private DiContainer _container;
        [Inject] private IDebug _debug;
        
        private void Start()
        {
            var initialBuilders = Util.GetActiveChildComponents<BuilderFSM>(_buildersParent);

            if (initialBuilders != null)
            {
                _availableBuilders.AddRange(initialBuilders); // TODO Handle case when builder dies (arrays will throw exception)
            }
            
            StartCoroutine(CheckSitesToBuild());
        }

        public void AddConstructionTask(BuildingConstructionScript site)
        {
            _constructionsPendingList.Add(site);
            site.OnConstructionFinished += OnConstructionFinished;
        }

        private void TryTransferPendingToActiveConstruction()
        {
            if (_constructionsPendingList.Count > 0 && _availableBuilders.Count > 0)
            {
                var firstPending = _constructionsPendingList[0];
                _constructionsPendingList.RemoveAt(0);

                _constructionsActiveList.Add(firstPending);
            }
        }
        
        private void UpdateAvailableBuildersTasks()
        {
            for (int index = 0; index < _constructionsActiveList.Count; index++)
            {
                var activeConstruction = _constructionsActiveList[index];
                
                // If site finished and waiting for builders to get freed (to add to available list) ignore this site
                if (activeConstruction.IsConstructionFinished || activeConstruction.IsConstructionCanceled)
                    continue;
                
                var freeSlotCount = activeConstruction.BuildersNeeded;
                var workingBuilders = new List<BuilderFSM>();

                if (_activeSitesBuildersMap.ContainsKey(activeConstruction))
                {
                    workingBuilders = _activeSitesBuildersMap[activeConstruction];
                    freeSlotCount -= workingBuilders.Count;
                }
                else
                {
                    _activeSitesBuildersMap[activeConstruction] = workingBuilders;
                }

                var buildersToAdd = Mathf.Min(_availableBuilders.Count, freeSlotCount);
                
                if (buildersToAdd > 0)
                {
                    for (int i = 0; i < buildersToAdd; i++)
                    {
                        var availableBuilder = _availableBuilders.GetFirstItem();
                        _availableBuilders.Remove(availableBuilder);
          
                        workingBuilders.Add(availableBuilder);
                    }
                    
                    foreach (var workingBuilder in workingBuilders)
                    {
                        // Check to avoid setting task again for already assigned builders
                        if (!workingBuilder.BuildTargetSet && !workingBuilder.IsBuilding)
                        {
                            workingBuilder.SetBuildingTask(activeConstruction);
                        }
                    }
                }
            }
            
            TryTransferPendingToActiveConstruction();
        }
        
        private IEnumerator CheckSitesToBuild()
        {
            while (true)
            {
                if (_availableBuilders.Count > 0)
                {
                    UpdateAvailableBuildersTasks();
                }

                yield return new WaitForSeconds(_buildingsCheckFrequency);
            }
        }

        private async void OnConstructionFinished(BuildingConstructionScript building, bool canceled)
        {
            var workingBuilders = _activeSitesBuildersMap[building];

            // Free builders only when every one of them becomes available (enters idle state)
            await UniTask.WaitUntil(() => workingBuilders.All(builder => builder.IsAvailable));

            _activeSitesBuildersMap.Remove(building);
            _constructionsActiveList.Remove(building);
            _availableBuilders.AddRange(workingBuilders);

            building.OnConstructionFinished -= OnConstructionFinished;

            if (!canceled)
            {
                OnConstructionBuilt?.Invoke(building);
            }
        }

        public void Dispatch<T>(FSM<T> fsm) where T : IFSM<T>
        {
            var builder = fsm as BuilderFSM;
            if (builder == null) 
                return;
            
            _availableBuilders.Remove(builder);
        }

        public void Create(Vector3 position)
        {
            var newBuilder = Instantiate(_builderPrefab, position, Quaternion.identity, _buildersParent);
            _container.Inject(newBuilder);
            _availableBuilders.Add(newBuilder);
        }
    }
}
