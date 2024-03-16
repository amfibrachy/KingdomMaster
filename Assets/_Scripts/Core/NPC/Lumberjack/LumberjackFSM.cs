namespace _Scripts.Core.NPC
{
    using AI;
    using Global;
    using global::Zenject;
    using JobSystem;
    using ResourceSystem;
    using States;
    using UnityEngine;

    public class LumberjackFSM : FSM<LumberjackFSM>, IHasJob
    {
        [Header("Wandering")]
        [SerializeField] private float _idleWaitMaxTime;

        [Header("Job details")]
        [SerializeField] private float _timeBetweenChops;
        
        [Header("Effects")]
        [SerializeField] private Vector3 _chopParticlesOffset;

        // Injectables
        private KingdomBordersController _bordersController;

        /*************************************** Public Access To Different States and Objects  *******************************************/
        public LumberjackWanderingState WanderingState { get; private set; }
        public LumberjackGoAndChopState GoAndChopState { get; private set; }
        
        /************************************************************* Fields  *************************************************************/
        public JobType Job { get; set; }
        public Direction MovingDirection { get; set; }
        public Transform DestinationTarget { get; set; }
        public float DestinationOffsetDistance { get; set; }
        public TreeScript TreeToChop { get; private set; }
        public bool ChopTreeTargetSet { get; set; }
        public bool IsWandering { get; set; }
        public bool IsWalkingToChopTree { get; set; }
        public bool IsWaitingInIdle { get; set; }
        public bool IsChopping { get; set; }

        /************************************************************* Readonly Fields  *************************************************************/
        
        public float IdleWaitMaxTime => _idleWaitMaxTime;
        public float TimeBetweenChops => _timeBetweenChops;
        
        public bool IsAvailable => _currentState == WanderingState;

        public KingdomBordersController BordersController => _bordersController;

        [Inject]
        public void Construct(KingdomBordersController bordersController)
        {
            _bordersController = bordersController;
        }
        
        public override void ShowParticles()
        {
            ParticlePosition = transform.position + new Vector3(
                AnimationController.IsFacingRight ? _chopParticlesOffset.x : -_chopParticlesOffset.x, _chopParticlesOffset.y, _chopParticlesOffset.z);
            
            ParticleSpawner.ParticlePool.Get();
        }

        public void OnAxeHitTree()
        {
            ShowParticles();
            
            if (_currentState == GoAndChopState)
            {
                GoAndChopState.ChopTree();
            }
        }

        public void SetTreeToCut(TreeScript tree)
        {
            tree.MarkToCut();
            TreeToChop = tree;
            ChopTreeTargetSet = true;
            
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
