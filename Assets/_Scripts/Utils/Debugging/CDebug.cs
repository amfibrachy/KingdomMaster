namespace _Scripts.Utils.Debugging
{
    using System;
    using StackFrame = System.Diagnostics.StackFrame;
    using Debug = UnityEngine.Debug;

    /// <summary>
    /// Class that wraps UnityEngine.Debug methods (Log, LogWarning, LogError) to add highlight of calling method and type containing it.
    /// So all logs will match style guide requirements:
    /// "[ClassName] MethodName: Your message goes here"
    /// </summary>
    public static class CDebug
    {
        private const int SkipFrames = 1;
        private const string BaseString = "[{0}][<b><color=#7BCF3A>{1}</color></b>] <b><color=cyan>{2}</color></b>: {3}";
        private const string MethodBaseNullString = "methodBase is null! Message: {0}";
        private const string TimeFormatString = "HH:mm:ss.fffff";


        /// <summary>
        /// Logs a message to the Unity Console, highlights calling method and type containing it.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Log(object message)
        {
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

        /// <summary>
        /// A variant of CDebug.Log that logs a warning message to the console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogWarning(object message)
        {
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

        /// <summary>
        /// A variant of CDebug.Log that logs an error message to the console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogError(object message)
        {
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
