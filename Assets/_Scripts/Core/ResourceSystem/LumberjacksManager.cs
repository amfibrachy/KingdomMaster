namespace _Scripts.Core.BuildSystem
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AI;
    using Cysharp.Threading.Tasks;
    using Global;
    using global::Zenject;
    using NPC;
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
        
        [Header("Job details")]
        [SerializeField] private float _treeCheckDistanceFromBorder;
        
        public int Count => _availableLumberjacks.Count;

        // Injectables
        private KingdomBordersController _kingdomBordersController;
        [Inject(Id = "LumberjacksParent")] private Transform _lumberjacksParent;
        [Inject] private IDebug _debug;
        
        // Privates
        private List<LumberjackFSM> _availableLumberjacks = new List<LumberjackFSM>();

        private Dictionary<TreeScript, LumberjackFSM> _activeTreeLumberjacksMap = new();
        
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

        public void AddLumberjackHut(BuildingPlacementScript building)
        {
            
        }
        
        private IEnumerator CheckForTreesToChop()
        {
            while (true)
            {
                if (_availableLumberjacks.Count > 0)
                {
                    var availableLumberjack = _availableLumberjacks[_availableLumberjacks.Count - 1];
                    var treesToCut = GetAvailableTrees(availableLumberjack.Position);
                    
                    if (treesToCut.Count > 0)
                    {
                        var tree = treesToCut[0];
                        
                        _availableLumberjacks.RemoveAt(_availableLumberjacks.Count - 1);
                        _activeTreeLumberjacksMap[tree] = availableLumberjack;

                        tree.OnTreeChopped += OnTreeChoppedDown;  // TODO unregister event somewhere
                        
                        tree.MarkToCut();
                        availableLumberjack.SetTreeToCut(tree);
                    }
                }

                yield return new WaitForSeconds(_treeCutCheckFrequency);
            }
        }

        private async void OnTreeChoppedDown(TreeScript tree)
        {
            var workingLumberJack = _activeTreeLumberjacksMap[tree];
    
            await UniTask.WaitUntil(() => workingLumberJack.IsAvailable);

            _availableLumberjacks.Add(workingLumberJack);
            _activeTreeLumberjacksMap.Remove(tree);
        }

        private List<TreeScript> GetAvailableTrees(Vector2 searchOrigin)
        {
            var hitsRight = new RaycastHit2D[_maxTreesToRaycast / 2];
            var hitsLeft = new RaycastHit2D[_maxTreesToRaycast / 2];

            var leftDistance = Vector2.Distance(searchOrigin, _kingdomBordersController.LeftBorderPosition) + _treeCheckDistanceFromBorder;
            var rightDistance = Vector2.Distance(searchOrigin, _kingdomBordersController.RightBorderPosition) + _treeCheckDistanceFromBorder;
            
            Physics2D.RaycastNonAlloc(searchOrigin, Vector2.right, hitsRight, rightDistance, _treesLayer);
            Physics2D.RaycastNonAlloc(searchOrigin, Vector2.left, hitsLeft, leftDistance, _treesLayer);

            var combinedHits = new List<RaycastHit2D>(hitsLeft);
            combinedHits.AddRange(hitsRight);

            // Sort combined hits by distance
            combinedHits = combinedHits.OrderBy(hit => hit.distance).ToList();

            return combinedHits
                .Where(hit => hit.collider != null)
                .Select(hit => hit.collider.GetComponent<TreeScript>())
                .Where(tree => !tree.IsMarked && !tree.IsChoppedDown)
                .ToList();
        }

        public void Dispatch<T>(FSM<T> fsm) where T : IFSM<T>
        {
            _availableLumberjacks.Remove(fsm as LumberjackFSM);
        }
        
        public void Create(Vector3 position)
        {
            var newLumberjack = Instantiate(_lumberjackPrefab, position, Quaternion.identity);
            _availableLumberjacks.Add(newLumberjack);
        }
    }
}
