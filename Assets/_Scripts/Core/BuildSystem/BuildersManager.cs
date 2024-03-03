namespace _Scripts.Core.BuildSystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AI;
    using global::Zenject;
    using NPC;
    using UnityEngine;
    using Utils.Debugging;
    using Utils;

    public class BuildersManager : MonoBehaviour, IDispatchable, ICreatable, ICountable
    {
        [SerializeField] private BuilderFSM _builderPrefab;
        
        public int Count => _availableBuilders.Count;
        
        private List<BuildingConstructionScript> _constructionsActiveList = new List<BuildingConstructionScript>();
        private List<BuildingConstructionScript> _constructionsPendingList = new List<BuildingConstructionScript>();

        private List<BuilderFSM> _availableBuilders = new List<BuilderFSM>();

        private Dictionary<BuildingConstructionScript, List<BuilderFSM>> _activeSitesBuildersMap = new();

        public event Action<BuildingConstructionScript> OnConstructionFinished;

        [Inject(Id = "BuildersParent")] private Transform _buildersParent;
        [Inject] private IDebug _debug;
        
        private void Start()
        {
            var initialBuilders = Util.GetActiveChildComponents<BuilderFSM>(_buildersParent);

            if (initialBuilders != null)
            {
                _availableBuilders.AddRange(initialBuilders); // TODO Handle case when builder dies (arrays will throw exception)
            }
        }

        public void AddConstructionTask(BuildingConstructionScript site)
        {
            _constructionsPendingList.Add(site);
        }

        private void TransferPendingToActiveConstruction()
        {
            var firstPending = _constructionsPendingList[0];
            _constructionsPendingList.RemoveAt(0);
                    
            _constructionsActiveList.Add(firstPending);
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
                        var availableBuilder = _availableBuilders[_availableBuilders.Count - 1];
                        _availableBuilders.RemoveAt(_availableBuilders.Count - 1);

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
            
            if (_constructionsPendingList.Count > 0 && _availableBuilders.Count > 0)
            {
                TransferPendingToActiveConstruction(); 
            }
        }

        private void Update()
        {
            if (_availableBuilders.Count > 0)
            {
                UpdateAvailableBuildersTasks();
            }

            for (int index = _constructionsActiveList.Count - 1; index >= 0; --index)
            {
                var site = _constructionsActiveList[index];
                
                if (site.IsConstructionCanceled || site.IsConstructionFinished)  // TODO can switch to event based system like lumberjack
                {
                    var workingBuilders = _activeSitesBuildersMap[site];
        
                    // Free builders only when every one of them becomes available (enters idle state)
                    if (workingBuilders.All(builder => builder.IsAvailable)) {
                        _activeSitesBuildersMap.Remove(site);
                        _constructionsActiveList.RemoveAt(index);

                        if (site.IsConstructionFinished)
                        {
                            OnConstructionFinished?.Invoke(site);
                        }

                        _availableBuilders.AddRange(workingBuilders);
                    }
                } 
            }
        }

        public void Dispatch<T>(FSM<T> fsm) where T : IFSM<T>
        {
            _availableBuilders.Remove(fsm as BuilderFSM);
        }
        
        public void Create(Vector3 position)
        {
            var newBuilder = Instantiate(_builderPrefab, position, Quaternion.identity);
            _availableBuilders.Add(newBuilder);
        }
    }
}
