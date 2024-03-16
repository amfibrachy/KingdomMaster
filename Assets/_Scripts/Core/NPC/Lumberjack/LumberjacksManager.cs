namespace _Scripts.Core.NPC
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AI;
    using BuildSystem;
    using Cysharp.Threading.Tasks;
    using Global;
    using global::Zenject;
    using ResourceSystem;
    using UnityEngine;
    using Utils.Debugging;
    using Utils;

    public class LumberjacksManager : MonoBehaviour, IDispatchable, ICreatable, ICountable
    {
        [SerializeField] private LumberjackFSM _lumberjackPrefab;
        
        [Header("Tree raycast")]
        [SerializeField] private int _maxTreesToRaycast;
        [SerializeField] private LayerMask _treesLayer;
        [SerializeField] private float _treeCutCheckFrequency;
        
        public int Count => _availableLumberjacks.Count;

        // Injectables
        private KingdomBordersController _kingdomBordersController;
        [Inject(Id = "LumberjacksParent")] private Transform _lumberjacksParent;
        [Inject] private DiContainer _container;
        [Inject] private IDebug _debug;
        
        // Privates
        private HashSet<LumberjackFSM> _availableLumberjacks = new HashSet<LumberjackFSM>();
        private Dictionary<BuildingDataScript, List<TreeScript>> _hutMap = new();

        private Dictionary<TreeScript, LumberjackFSM> _activeTreeLumberjacksMap = new();
        
        private List<TreeScript> _treesActiveList = new List<TreeScript>();
        private List<TreeScript> _treesPendingList = new List<TreeScript>();
        
        [Inject]
        public void Construct(KingdomBordersController kingdomBordersController)
        {
            _kingdomBordersController = kingdomBordersController;
        }
        
        private void Start()
        {
            var initialLumberjacks = Util.GetActiveChildComponents<LumberjackFSM>(_lumberjacksParent);

            if (initialLumberjacks != null)
            {
                _availableLumberjacks.AddRange(initialLumberjacks); // TODO Handle case when lumberjack dies (arrays will throw exception)
            }
            
            StartCoroutine(CheckForTreesToChop());
        }
        
        public void AddLumberjackHut(BuildingDataScript building)
        {
            var radius = building.Data.EffectiveRange;
            var colliders = new Collider2D[_maxTreesToRaycast];
            
            var count = Physics2D.OverlapBoxNonAlloc(building.transform.position, new Vector2(radius, 5f), 0f, colliders, _treesLayer);
            var treesWithinArea = new List<TreeScript>();
            
            if (count >= _maxTreesToRaycast)
            {
                Debug.LogError($"More than {_maxTreesToRaycast} trees are in radius of lumberjack hut");
            }
            
            for (int i = 0; i < count; i++)
            {
                var tree = colliders[i].GetComponent<TreeScript>();
                if (tree != null && !tree.IsChoppedDown)
                {
                    treesWithinArea.Add(tree);
                }
            }

            _hutMap.Add(building, treesWithinArea);

            AddTreesChopTask(treesWithinArea);
        }

        private void AddTreesChopTask(IEnumerable<TreeScript> trees)
        {
            _treesPendingList.AddRange(trees);

            foreach (var tree in _treesPendingList)
            {
                tree.OnTreeChopped += OnTreeChoppedDown;
            }
        }
        
        private void TryTransferPendingToActiveTrees()
        {
            var treeCount = Mathf.Min(_treesPendingList.Count, _availableLumberjacks.Count);

            if (treeCount > 0)
            {
                var pendingTrees = _treesPendingList.GetRange(0, treeCount);
                _treesPendingList.RemoveRange(0, treeCount);

                _treesActiveList.AddRange(pendingTrees);
            }
        }
        
        private void UpdateAvailableLumberjacksTasks()
        {
            for (int index = 0; index < _treesActiveList.Count; index++)
            {
                var activeTree = _treesActiveList[index];
                
                // If tree is chopped down and waiting for lumberjack to get freed (to add to available list) ignore this tree
                if (activeTree.IsMarked || activeTree.IsChoppedDown)
                    continue;
                
                if (_availableLumberjacks.Count > 0)
                {
                    var availableLumberjack = _availableLumberjacks.GetFirstItem();
                    
                    // Check to avoid setting task again for already assigned lumberjack
                    if (!availableLumberjack.ChopTreeTargetSet && !availableLumberjack.IsChopping)
                    {
                        _availableLumberjacks.Remove(availableLumberjack);
                        _activeTreeLumberjacksMap[activeTree] = availableLumberjack;
                        
                        availableLumberjack.SetTreeToCut(activeTree);
                    }
                }
            }

            TryTransferPendingToActiveTrees();
        }
        
        private IEnumerator CheckForTreesToChop()
        {
            while (true)
            {
                if (_availableLumberjacks.Count > 0)
                {
                    UpdateAvailableLumberjacksTasks();
                }

                yield return new WaitForSeconds(_treeCutCheckFrequency);
            }
        }

        private async void OnTreeChoppedDown(TreeScript tree)
        {
            var workingLumberJack = _activeTreeLumberjacksMap[tree];
    
            await UniTask.WaitUntil(() => workingLumberJack.IsAvailable);

            _availableLumberjacks.Add(workingLumberJack);
            _treesActiveList.Remove(tree);
            _activeTreeLumberjacksMap.Remove(tree);

            tree.OnTreeChopped -= OnTreeChoppedDown;
        }
        
        public void Dispatch<T>(FSM<T> fsm) where T : IFSM<T>
        {
            var lumberjack = fsm as LumberjackFSM;
            if (lumberjack == null) 
                return;
            
            _availableLumberjacks.Remove(lumberjack);

            if (_activeTreeLumberjacksMap.ContainsKey(lumberjack.TreeToChop))
            {
                _activeTreeLumberjacksMap.Remove(lumberjack.TreeToChop);
                lumberjack.TreeToChop.UnMarkToCut();
            }
        }
        
        public void Create(Vector3 position)
        {
            var newLumberjack = Instantiate(_lumberjackPrefab, position, Quaternion.identity, _lumberjacksParent);
            _container.Inject(newLumberjack);
            _availableLumberjacks.Add(newLumberjack);
        }
    }
}
