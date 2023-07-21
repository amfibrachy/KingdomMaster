namespace _Scripts.Core.States
{
    public abstract class BaseState<T>
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
