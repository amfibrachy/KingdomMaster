namespace _Scripts.Core.NPC
{
    using System.Threading;
    using AI;
    using global::Zenject;
    using States;
    using UnityEngine;
    using Utils.Debugging;

    public class SluggardFSM : FSM<SluggardFSM>
    {
        // Injectables
        private IDebug _debug;
        
        // Privates
        [SerializeField] private Transform _wanderingTarget;
        [SerializeField] private float _destinationOffsetWanderingMaxDistance;
        [SerializeField] private float _idleWaitMaxTime;

        // Public Access To Different States and Objects
        public IDebug Debug => _debug;

        public SluggardWanderingState WanderingState;
        
        /************************************************************* Fields  *************************************************************/
        
        public CancellationTokenSource CancellationSource { get; set; }
        public bool IsWandering { get; set; }
        public bool IsWaitingInIdle { get; set; }

        /************************************************************* Readonly Fields  *************************************************************/

        public Transform WanderingTarget => _wanderingTarget;
        public float DestinationOffsetWanderingMaxDistance => _destinationOffsetWanderingMaxDistance;

        public float IdleWaitMaxTime => _idleWaitMaxTime;

        [Inject]
        public void Construct(IDebug debug)
        {
            _debug = debug;
        }

        public override void InitStates()
        {
            WanderingState = new SluggardWanderingState(this);
            
            _currentState = WanderingState;
            _currentState.EnterState();
        }
    }
}
