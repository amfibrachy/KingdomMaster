namespace _Scripts.Core.AI
{
    public interface IFSM<T> where T : IFSM<T>
    {
        public void InitStates();
        public void ChangeState(BaseState<T> newState);
        public void Dispatch();
    }
}
