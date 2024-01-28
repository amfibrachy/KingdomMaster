namespace _Scripts.Core.NPC
{
    using AI;

    public interface IDispatchable
    {
        public void Dispatch<T>(FSM<T> fsm) where T : IFSM<T>;
    }
}
