namespace _Scripts.Core.AI
{
    using States;

    public interface IFSM<T> where T : IFSM<T>
    {
        public void ChangeState(BaseState<T> newState);
    }
}
