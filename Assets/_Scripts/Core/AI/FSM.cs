namespace _Scripts.Core.AI
{
    using System.Threading;
    using Animations;
    using Global;
    using global::Zenject;using ParticleSystem;
    using Stats;
    using UnityEngine;
    using Utils.Debugging;

    [RequireComponent(typeof(AnimationControllerScript))]
    public abstract class FSM<T> : MonoBehaviour, IParticleEmitter, IFSM<T> where T : IFSM<T>
    {
        [SerializeField] private BaseStats _stats;
        
        protected AgentType Agent;
        
        // Injectables
        [Inject] public IDebug Debug;
        [Inject] protected PopulationController PopulationController;
        
        public Vector3 ParticlePosition { get; set; }
        public AnimationControllerScript AnimationController => _animationController;
        public ParticleSpawnerScript ParticleSpawner => _particleSpawner;
        public CancellationTokenSource CancellationSource { get; set; }
        public BaseStats Stats => _stats;

        public Vector2 Position => transform.position;
        
        private AnimationControllerScript _animationController;
        private ParticleSpawnerScript _particleSpawner;
        
        protected BaseState<T> _currentState;

        private void Start()
        {
            _animationController = GetComponent<AnimationControllerScript>();
            _particleSpawner = GetComponent<ParticleSpawnerScript>();
            
            InitStates();
        }

        public abstract void InitStates();
        public abstract void ShowParticles();

        public virtual void Dispatch()
        {
            CancellationSource.Cancel();
            _currentState.ExitState();
            _currentState = null;
            
            PopulationController.OnDispatch(this, Agent);
        }

        public void ChangeState(BaseState<T> newState)
        {
            _currentState.ExitState();
            _currentState = newState;
            _currentState.EnterState();
            
            // Debug.Log($"{gameObject.name} state changed to: " + newState.GetType().Name);
        }
        
        private void Update()
        {
            if (_currentState != null)
            {
                _currentState.UpdateState();
            }
        }
    }
}
