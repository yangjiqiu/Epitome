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

                    if (!pathLine.Contains("Epitome.LogSystem") && !pathLine.Contains("YLog"))
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
                    //var ListViewStateType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ListViewState");
                    //fieldInfo = ConsoleWindowType.GetField("m_ListView", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    //var listView = fieldInfo.GetValue(consoleInstance);
                    //fieldInfo = ListViewStateType.GetField("row", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                    //int row = (int)fieldInfo.GetValue(listView);
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

        public static void LoadAppenders(AppenderType type)
        {
            Logging.Instance.LoadAppenders(type);
        }

        public static void LoadAppenders(ILogAppender appender)
        {
            Logging.Instance.LoadAppenders(appender);
        }

        public static void UnloadAppenders(AppenderType type)
        {
            Logging.Instance.UnloadAppenders(type);
        }

        public static void UnloadAppenders(ILogAppender appender)
        {
            Logging.Instance.UnloadAppenders(appender);
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