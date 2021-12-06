using System;
using UnityEngine;

namespace Epitome.LogSystem
{
    public class Log
    {
#if UNITY_EDITOR
        [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            string stackTrace = GetStackTrace();
            if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Contains("Log.cs"))
            {
                System.Text.RegularExpressions.Match matches = System.Text.RegularExpressions.Regex.Match(stackTrace, @"\(at (.+)\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                string pathLine = "";
                while (matches.Success)
                {
                    pathLine = matches.Groups[1].Value;

                    if (!pathLine.Contains("Epitome.LogSystem"))
                    {
                        int splitIndex = pathLine.LastIndexOf(":");
                        string path = pathLine.Substring(0, splitIndex);
                        line = System.Convert.ToInt32(pathLine.Substring(splitIndex + 1));
                        string fullPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                        fullPath = fullPath + path;
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath.Replace('/', '\\'), line);
                        break;
                    }
                    matches = matches.NextMatch();
                }
                return true;
            }
            return false;
        }

        private static string GetStackTrace()
        {
            var ConsoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            var fieldInfo = ConsoleWindowType.GetField("ms_ConsoleWindow", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var consoleInstance = fieldInfo.GetValue(null);
            if (consoleInstance != null)
            {
                if ((object)UnityEditor.EditorWindow.focusedWindow == consoleInstance)
                {
                    fieldInfo = ConsoleWindowType.GetField("m_ActiveText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    string activeText = fieldInfo.GetValue(consoleInstance).ToString();
                    return activeText;
                }
            }
            return null;
        }
#endif
        public static void EnableLog(bool enable)
        {
            Logging.Instance.EnableLog = enable;
        }

        public static void LogLevel(LogLevel level)
        {
            Logging.Instance.logLevel = level;
        }

        public static void RegisterLogMessage()
        {
            Application.logMessageReceived += HandleLog;
        }

        public static void UnRegisterLogMessage()
        {
            Application.logMessageReceived -= HandleLog;
        }

        public static void HandleLog(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    Error((object)condition, stackTrace);
                    break;
                case LogType.Assert:
                    break;
                case LogType.Log:
                    Debug((object)condition, stackTrace);
                    break;
                case LogType.Exception:
                    Error((object)condition, stackTrace);
                    break;
                case LogType.Warning:
                    Warn((object)condition, stackTrace);
                    break;
            }
        }

        public static void LoadAppenders(AppenderType type)
        {
            switch (type)
            {
                case AppenderType.Console:
                    LoadAppenders(new ConsoleAppender());
                    break;
                case AppenderType.GUI:
                    LoadAppenders(GUIAppender.Instance);
                    break;
                case AppenderType.MobileGUI:
                    LoadAppenders(MobileGUIAppender.Instance);
                    break;
                case AppenderType.File:
                    LoadAppenders(FileAppender.Instance);
                    break;
                case AppenderType.Window:
                    LoadAppenders(new WindowAppender());
                    break;
            }
        }

        public static void LoadAppenders(ILogAppender appender)
        {
            Logging.Instance.LoadAppenders(appender);
        }

        public static void UnloadAppenders(ILogAppender appender)
        {
            Logging.Instance.UnloadAppenders(appender);
        }

        public static void UnloadAppenders(AppenderType type)
        {
            Logging.Instance.UnloadAppenders(type);
        }

        public static void Trace(object message)
        {
            Logging.Instance.Trace(message);
        }

        public static void Debug(object message)
        {
            Logging.Instance.Debug(message);
        }
        public static void Debug(object message, string track)
        {
            Logging.Instance.Debug(message, track);
        }
        public static void Debug(object message, Exception e)
        {
            Logging.Instance.Debug(message, e);
        }

        public static void Info(object message)
        {
            Logging.Instance.Info(message);
        }
        public static void Info(object message, string track)
        {
            Logging.Instance.Info(message, track);
        }
        public static void Info(object message, Exception e)
        {
            Logging.Instance.Info(message, e);
        }

        public static void Warn(object message)
        {
            Logging.Instance.Warn(message);
        }
        public static void Warn(object message, string track)
        {
            Logging.Instance.Warn(message, track);
        }
        public static void Warn(object message, Exception e)
        {
            Logging.Instance.Warn(message, e);
        }

        public static void Error(object message)
        {
            Logging.Instance.Error(message);
        }
        public static void Error(object message, string track)
        {
            Logging.Instance.Error(message, track);
        }
        public static void Error(object message, Exception e)
        {
            Logging.Instance.Error(message, e);
        }

        public static void Fatal(object message)
        {
            Logging.Instance.Fatal(message);
        }
        public static void Fatal(object message, string track)
        {
            Logging.Instance.Fatal(message, track);
        }
        public static void Fatal(object message, Exception e)
        {
            Logging.Instance.Fatal(message, e);
        }
    }
}