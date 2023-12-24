namespace _Scripts.Core.NPC
{
    using System.Threading;
    using AI;
    using BuildSystem;
    using global::Zenject;
    using States;
    using UnityEngine;
    using Utils.Debugging;

    public class BuilderFSM : FSM<BuilderFSM>
    {
        // Injectables
        [Inject] private IDebug _debug;
        
        // Privates
        [SerializeField] private Transform _destinationTargetCamp;
        [SerializeField] private float _destinationOffsetWanderingMaxDistance;
        [SerializeField] private float _idleWaitMaxTime;

        // Public Access To Different States and Objects
        public IDebug Debug => _debug;

        public BuilderWanderingState WanderingState { get; private set; }
        public BuilderGoAndBuildState GoAndBuildState { get; private set; }
        
        /************************************************************* Fields  *************************************************************/
        
        public CancellationTokenSource CancellationSource { get; set; }
        public Transform DestinationTarget { get; set; }
        public float DestinationOffsetMaxDistance { get; set; }
        public BuildingConstructionScript Site { get; set; }
        public bool BuildTargetSet { get; set; }
        public bool IsWandering { get; set; }
        public bool IsWalkingToConstructionSite { get; set; }
        public bool IsWaitingInIdle { get; set; }
        public bool IsBuilding { get; set; }

        /************************************************************* Readonly Fields  *************************************************************/
        
        public float DestinationOffsetWanderingMaxDistance => _destinationOffsetWanderingMaxDistance;
        public Transform DestinationTargetCamp => _destinationTargetCamp;
        public float IdleWaitMaxTime => _idleWaitMaxTime;
        
        public bool IsAvailable => _currentState == WanderingState;
        
        [Inject]
        public void Construct(IDebug debug)
        {
            _debug = debug;
        }

        public void SetBuildingTask(BuildingConstructionScript site)
        {
            Site = site;
            BuildTargetSet = true;
            
            CancelCurrentTask();
        }

        public void CancelCurrentTask()
        {
            CancellationSource.Cancel();
        }
        
        public override void InitStates()
        {
            WanderingState = new BuilderWanderingState(this);
            GoAndBuildState = new BuilderGoAndBuildState(this);
            
            _currentState = WanderingState;
            _currentState.EnterState();
        }
    }
}
