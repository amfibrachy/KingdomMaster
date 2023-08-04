namespace _Scripts.Core.AI
{
    using System;
    using Animations;
    using UnityEngine;

    [RequireComponent(typeof(AnimationControllerScript))]
    public abstract class FSM<T> : MonoBehaviour, IFSM<T> where T : IFSM<T>
    {
        public AnimationControllerScript AnimationController => _animationController;
        
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
        }
        
        private void Update()
        {
            _currentState.UpdateState();
        }
    }
}
