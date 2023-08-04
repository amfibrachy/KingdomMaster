namespace _Scripts.Core.AI
{
    using States;
    using UnityEngine;

    public abstract class FSM<T> : MonoBehaviour, IFSM<T> where T : IFSM<T>
    {
        protected BaseState<T> _currentState;
        
        public void ChangeState(BaseState<T> newState)
        {
            _currentState.ExitState();
            _currentState = newState;
            _currentState.EnterState();
        }
    }
}
