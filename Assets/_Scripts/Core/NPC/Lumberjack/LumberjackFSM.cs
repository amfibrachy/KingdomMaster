namespace _Scripts.Core.NPC
{
    using AI;
    using BuildSystem;
    using JobSystem;
    using ResourceSystem;
    using States;
    using UnityEngine;

    public class LumberjackFSM : FSM<LumberjackFSM>, IHasJob
    {
        [Header("Wandering")]
        [SerializeField] private Transform _destinationTargetCamp;
        [SerializeField] private float _destinationOffsetWanderingMaxDistance;
        [SerializeField] private float _idleWaitMaxTime;

        [Header("Job details")]
        [SerializeField] private float _timeBetweenChops;
        
        [Header("Effects")]
        [SerializeField] private Vector3 _chopParticlesOffset;

        // Privates
        private BuilderParticleSpawner _particleSpawner;
        
        /*************************************** Public Access To Different States and Objects  *******************************************/
        public LumberjackWanderingState WanderingState { get; private set; }
        public LumberjackGoAndChopState GoAndChopState { get; private set; }
        
        /************************************************************* Fields  *************************************************************/
        public JobType Job { get; set; }
        public Direction MovingDirection { get; set; }
        public Transform DestinationTarget { get; set; }
        public float DestinationOffsetDistance { get; set; }
        public TreeScript TreeToChop { get; private set; }
        public bool ChopTreeSet { get; set; }
        public bool IsWandering { get; set; }
        public bool IsWalkingToChopTree { get; set; }
        public bool IsWaitingInIdle { get; set; }
        public bool IsChopping { get; set; }
        public Vector3 ChopParticlesPosition { get; private set; }

        /************************************************************* Readonly Fields  *************************************************************/
        
        public float DestinationOffsetWanderingMaxDistance => _destinationOffsetWanderingMaxDistance;
        public Transform DestinationTargetCamp => _destinationTargetCamp;
        public float IdleWaitMaxTime => _idleWaitMaxTime;
        public float TimeBetweenChops => _timeBetweenChops;
        
        public bool IsAvailable => _currentState == WanderingState;

        private void Awake()
        {
            _particleSpawner = GetComponent<BuilderParticleSpawner>();
        }

        public void ShowChoppingParticles()
        {
            ChopParticlesPosition = transform.position + new Vector3(
                AnimationController.IsFacingRight ? _chopParticlesOffset.x : -_chopParticlesOffset.x, _chopParticlesOffset.y, _chopParticlesOffset.z);
            
            _particleSpawner.ParticlePool.Get();
        }

        public void SetTreeToCut(TreeScript tree)
        {
            TreeToChop = tree;
            ChopTreeSet = true;
            
            CancelCurrentTask();
        }

        public void CancelCurrentTask()
        {
            CancellationSource.Cancel();
        }
        
        public override void InitStates()
        {
            Agent = AgentType.WithJob;
            Job = JobType.Lumberjack;
            
            WanderingState = new LumberjackWanderingState(this);
            GoAndChopState = new LumberjackGoAndChopState(this);
            
            _currentState = WanderingState;
            _currentState.EnterState();
        }
    }
}
