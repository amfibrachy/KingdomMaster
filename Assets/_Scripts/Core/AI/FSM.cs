namespace _Scripts.Core.AI
{
    using System;
    using Animations;
    using global::Zenject;
    using Stats;
    using UnityEngine;
    using Utils.Debugging;

    [RequireComponent(typeof(AnimationControllerScript))]
    public abstract class FSM<T> : MonoBehaviour, IFSM<T> where T : IFSM<T>
    {
        [SerializeField] private BaseStats _stats;
        
        [Inject] private IDebug _debug;
        
        public AnimationControllerScript AnimationController => _animationController;
        public BaseStats Stats => _stats;
        
        private AnimationControllerScript _animationController;
        protected BaseState<T> _currentState;

        private void Start()
        {
            _animationController = GetComponent<AnimationControllerScript>();
            InitStates();
        }

        public abstract void InitStates();

        public void ChangeState(BaseState<T> newState)
        {
            _currentState.ExitState();
            _currentState = newState;
            _currentState.EnterState();
            
            _debug.Log($"{gameObject.name} state changed to: " + newState.GetType().Name);
        }
        
        private void Update()
        {
            _currentState.UpdateState();
        }
    }
}
