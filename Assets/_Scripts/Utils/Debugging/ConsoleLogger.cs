namespace _Scripts.Utils.Debugging
{
    using System;
    using Zenject;
    using StackFrame = System.Diagnostics.StackFrame;
    using Debug = UnityEngine.Debug;
    
    public class ConsoleLogger : IDebug
    {
        private const int SkipFrames = 1;
        private const string BaseString = "[{0}][<b><color=#7BCF3A>{1}</color></b>] <b><color=cyan>{2}</color></b>: {3}";
        private const string MethodBaseNullString = "methodBase is null! Message: {0}";
        private const string TimeFormatString = "HH:mm:ss.fffff";

        [Inject] private DebugSettingsSO _settings;
        
        public void Log(object message)
        {
            if (!_settings.ShowLogs) 
                return;
            
            var frame = new StackFrame(SkipFrames);
            var methodBase = frame.GetMethod();

            if (methodBase == null)
            {
                Debug.LogWarningFormat(MethodBaseNullString, message);
                return;
            }

            var type = DebugUtils.GetCleanDeclaringTypeName(methodBase);
            var method = DebugUtils.GetCleanMethodName(methodBase);
            var time = DateTime.Now.ToString(TimeFormatString);
            Debug.LogFormat(BaseString, time, type, method, message);
        }
        
        public void LogWarning(object message)
        {
            if (!_settings.ShowLogs) 
                return;
            
            var frame = new StackFrame(SkipFrames);
            var methodBase = frame.GetMethod();

            if (methodBase == null)
            {
                Debug.LogWarningFormat(MethodBaseNullString, message);
                return;
            }

            var type = DebugUtils.GetCleanDeclaringTypeName(methodBase);
            var method = DebugUtils.GetCleanMethodName(methodBase);
            var time = DateTime.Now.ToString(TimeFormatString);
            Debug.LogWarningFormat(BaseString, time, type, method, message);
        }
        
        public void LogError(object message)
        {
            if (!_settings.ShowLogs) 
                return;
            
            var frame = new StackFrame(SkipFrames);
            var methodBase = frame.GetMethod();

            if (methodBase == null)
            {
                Debug.LogWarningFormat(MethodBaseNullString, message);
                return;
            }

            var type = DebugUtils.GetCleanDeclaringTypeName(methodBase);
            var method = DebugUtils.GetCleanMethodName(methodBase);
            var time = DateTime.Now.ToString(TimeFormatString);
            Debug.LogErrorFormat(BaseString, time, type, method, message);
        }
    }
}
