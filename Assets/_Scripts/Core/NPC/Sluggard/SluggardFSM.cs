namespace _Scripts.Core.NPC
{
    using System.Threading;
    using AI;
    using BuildSystem;
    using global::Zenject;
    using JobSystem;
    using States;
    using UnityEngine;
    using Utils.Debugging;

    public class SluggardFSM : FSM<SluggardFSM>
    {
        // Privates
        [SerializeField] private Transform _wanderingTarget;
        [SerializeField] private float _destinationOffsetWanderingMaxDistance;
        [SerializeField] private float _idleWaitMaxTime;

        /*********************************************************** States and Objects  ************************************************************/
        // Public Access To Different States and Objects
        
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

        public Transform WanderingTarget => _wanderingTarget;
        public float DestinationOffsetWanderingMaxDistance => _destinationOffsetWanderingMaxDistance;

        public float IdleWaitMaxTime => _idleWaitMaxTime;

        public override void InitStates()
        {
            Agent = AgentType.Sluggard;
            WanderingState = new SluggardWanderingState(this);
            GoAndTrainState = new SluggardGoAndTrainState(this);
            
            _currentState = WanderingState;
            _currentState.EnterState();
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
