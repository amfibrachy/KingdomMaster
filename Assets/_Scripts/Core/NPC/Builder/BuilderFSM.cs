namespace _Scripts.Core.NPC
{
    using AI;
    using BuildSystem;
    using Global;
    using global::Zenject;
    using JobSystem;
    using States;
    using UnityEngine;
    using Utils.Debugging;

    public class BuilderFSM : FSM<BuilderFSM>, IHasJob
    {
        [Header("Wandering")]
        [SerializeField] private float _idleWaitMaxTime;
        
        [Header("Effects")]
        [SerializeField] private Vector3 _buildParticlesOffset;
        
        // Injectables
        private KingdomBordersController _bordersController;
        
        /*************************************** Public Access To Different States and Objects  *******************************************/

        public BuilderWanderingState WanderingState { get; private set; }
        public BuilderGoAndBuildState GoAndBuildState { get; private set; }
        
        /************************************************************* Fields  *************************************************************/
        public JobType Job { get; set; }
        public Direction MovingDirection { get; set; }
        public Transform DestinationTarget { get; set; }
        public float DestinationOffsetDistance { get; set; }
        public BuildingConstructionScript Site { get; private set; }
        public bool BuildTargetSet { get; set; }
        public bool IsWandering { get; set; }
        public bool IsWalkingToConstructionSite { get; set; }
        public bool IsWaitingInIdle { get; set; }
        public bool IsBuilding { get; set; }

        /************************************************************* Readonly Fields  *************************************************************/
        
        public float IdleWaitMaxTime => _idleWaitMaxTime;
        public bool IsAvailable => _currentState == WanderingState;

        public KingdomBordersController BordersController => _bordersController;

        [Inject]
        public void Construct(KingdomBordersController bordersController)
        {
            _bordersController = bordersController;
        }
        
        public override void ShowParticles()
        {
            // Show hammer hit build particles
            ParticlePosition = transform.position + new Vector3(
                AnimationController.IsFacingRight ? _buildParticlesOffset.x : -_buildParticlesOffset.x, _buildParticlesOffset.y, _buildParticlesOffset.z);
            
            ParticleSpawner.ParticlePool.Get();
        }

        public void OnHammerHitBuilding()
        {
            ShowParticles();
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
            Agent = AgentType.WithJob;
            Job = JobType.Builder;
            
            WanderingState = new BuilderWanderingState(this);
            GoAndBuildState = new BuilderGoAndBuildState(this);
            
            _currentState = WanderingState;
            _currentState.EnterState();
        }
    }
}
