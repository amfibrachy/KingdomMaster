namespace _Scripts.Core.NPC
{
    using System;
    using System.Threading;
    using AI;
    using BuildSystem;
    using global::Zenject;
    using States;
    using UnityEngine;
    using UnityEngine.Serialization;
    using Utils.Debugging;

    public class BuilderFSM : FSM<BuilderFSM>
    {
        // Injectables
        [Inject] private IDebug _debug;
        
        // Privates
        [Header("Wandering")]
        [SerializeField] private Transform _destinationTargetCamp;
        [SerializeField] private float _destinationOffsetWanderingMaxDistance;
        [SerializeField] private float _idleWaitMaxTime;
        
        [Header("Effects")]
        [SerializeField] private Vector3 _buildParticlesOffset;

        private BuilderParticleSpawner _particleSpawner;
        
        /*************************************** Public Access To Different States and Objects  *******************************************/
        
        public IDebug Debug => _debug;

        public BuilderWanderingState WanderingState { get; private set; }
        public BuilderGoAndBuildState GoAndBuildState { get; private set; }
        
        /************************************************************* Fields  *************************************************************/
        
        public CancellationTokenSource CancellationSource { get; set; }
        
        public Direction MovingDirection { get; set; }
        public Transform DestinationTarget { get; set; }
        public float DestinationOffsetMaxDistance { get; set; }
        public BuildingConstructionScript Site { get; private set; }
        public bool BuildTargetSet { get; set; }
        public bool IsWandering { get; set; }
        public bool IsWalkingToConstructionSite { get; set; }
        public bool IsWaitingInIdle { get; set; }
        public bool IsBuilding { get; set; }
        public Vector3 BuildParticlesPosition { get; private set; }

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

        private void Awake()
        {
            _particleSpawner = GetComponent<BuilderParticleSpawner>();
        }

        public void ShowBuildingParticles()
        {
            BuildParticlesPosition = transform.position + new Vector3(
                AnimationController.IsFacingRight ? _buildParticlesOffset.x : -_buildParticlesOffset.x, _buildParticlesOffset.y, _buildParticlesOffset.z);
            
            _particleSpawner.ParticlePool.Get();
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
