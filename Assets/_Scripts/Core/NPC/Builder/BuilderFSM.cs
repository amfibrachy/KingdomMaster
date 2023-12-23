namespace _Scripts.Core.NPC
{
    using AI;
    using global::Zenject;
    using States;
    using UnityEngine;
    using Utils.Debugging;

    public class BuilderFSM : FSM<BuilderFSM>
    {
        [SerializeField] private GeneralWorkerStats _stats;
        
        // Injectables
        private IDebug _debug;
        
        // Privates
        [SerializeField] private Transform _wanderingTarget;
        [SerializeField] private float _wanderingMaxDistance;
        [SerializeField] private float _idleWaitMaxTime;

        // Public Access To Different States
        public IDebug Debug => _debug;

        public BuilderWanderingState WanderingState;
        public Transform WanderingTarget => _wanderingTarget;
        public float WanderingMaxDistance => _wanderingMaxDistance;
        public float IdleWaitMaxTime => _idleWaitMaxTime;
        
        public float Speed {get; private set; }

        [Inject]
        public void Construct(IDebug debug)
        {
            _debug = debug;
        }

        private void Awake()
        {
            Speed = _stats.WalkSpeed;
        }

        public override void InitStates()
        {
            WanderingState = new BuilderWanderingState(this);
            
            _currentState = WanderingState;
            _currentState.EnterState();
        }
    }
}
