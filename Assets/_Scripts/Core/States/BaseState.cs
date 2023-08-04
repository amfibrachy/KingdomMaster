namespace _Scripts.Core.States
{
    using AI;

    public abstract class BaseState<T> where T : IFSM<T>
    {
        protected T _context;

        public BaseState(T context)
        {
            _context = context;
        }

        public virtual void EnterState()
        {
        }

        public abstract void UpdateState();

        public virtual void ExitState()
        {
        }
    }
}
