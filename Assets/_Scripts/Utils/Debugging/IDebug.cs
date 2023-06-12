namespace _Scripts.Utils.Debugging
{
    public interface IDebug
    {
        public void Log(object message);
        public void LogWarning(object message);
        public void LogError(object message);
    }
}
