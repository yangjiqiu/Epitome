using System;

namespace Epitome.LogSystem
{
    public class ConsoleAppender : ILogAppender
    {
        public void Log(LogData data)
        {
            string str = String.Format("{0}\n[{1,-5}] {2}\n", data.logMessage, data.logLevel, data.logBasicData);

            switch (data.logLevel)
            {
                case LogLevel.TRACE:
                case LogLevel.DEBUG:
                case LogLevel.INFO:
                    UnityEngine.Debug.Log(str);
                    break;
                case LogLevel.WARN:
                    UnityEngine.Debug.LogWarning(str);
                    break;
                case LogLevel.ERROR:
                    UnityEngine.Debug.LogError(str);
                    break;
                case LogLevel.FATAL:
                    UnityEngine.Debug.LogError(str);
                    break;
            }
        }
    }
}