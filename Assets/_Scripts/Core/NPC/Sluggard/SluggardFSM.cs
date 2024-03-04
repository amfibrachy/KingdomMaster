namespace _Scripts.Core.NPC
{
    using AI;
    using BuildSystem;
    using Global;
    using global::Zenject;
    using JobSystem;
    using States;
    using UnityEngine;

    public class SluggardFSM : FSM<SluggardFSM>
    {
        [Header("Wandering")]
        [SerializeField] private float _idleWaitMaxTime;
        
        // Injectables
        private KingdomBordersController _bordersController;

        /********************************************** Public Access To Different States and Objects  **********************************************/

        public SluggardWanderingState WanderingState;
        public SluggardGoAndTrainState GoAndTrainState;
        
        /***************************************************************** Fields  ******************************************************************/
        
        public bool IsWandering { get; set; }
        public Direction MovingDirection { get; set; }
        public bool JobTrainingTargetSet { get; set; }
        public BuildingJobScript JobBuilding { get; private set; }
        public JobType Job { get; private set; }
        public Transform DestinationTarget { get; set; }
        public bool IsWalkingToJobBuilding { get; set; }
        public bool IsWaitingInIdle { get; set; }

        /************************************************************* Readonly Fields  *************************************************************/

        public float IdleWaitMaxTime => _idleWaitMaxTime;

        public KingdomBordersController BordersController => _bordersController;

        [Inject]
        public void Construct(KingdomBordersController bordersController)
        {
            _bordersController = bordersController;
        }
        
        public override void InitStates()
        {
            Agent = AgentType.Sluggard;
            WanderingState = new SluggardWanderingState(this);
            GoAndTrainState = new SluggardGoAndTrainState(this);
            
            _currentState = WanderingState;
            _currentState.EnterState();
        }

        public override void ShowParticles()
        {
        }

        public void SetJobTask(BuildingJobScript jobBuilding, JobType job)
        {
            JobBuilding = jobBuilding;
            Job = job;
            JobTrainingTargetSet = true;
            
            CancelCurrentTask();
        }
        
        public void CancelCurrentTask()
        {
            CancellationSource.Cancel();
        }
    }
}
