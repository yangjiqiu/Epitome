using System.Collections.Generic;
using UnityEngine;

namespace Epitome.LogSystem
{
    public class GUIAppender : AsyncTask<GUIAppender>, ILogAppender
    {
        protected const string TIME_FORMATER = "HH:mm:ss,fff";

        //边缘
        private const int margin = 20;
        private Rect windowRect;
        private Rect windowRectMin;

        private Rect titleBar_butArea;

        GUIStyle toolbarStyle; // 工具栏样式
        GUIStyle fontStyle; // 字体样式
        // LOG信息
        GUIStyle LogStyle1;
        GUIStyle SplitterStyle;
        GUIStyle LogStyle2;
        float logHeight = 32;
        List<LogData> logDatas = new List<LogData>();
        string detailedLog;

        LogLevel[] toolbarStr = new LogLevel[] { LogLevel.TRACE, LogLevel.DEBUG, LogLevel.INFO, LogLevel.WARN, LogLevel.ERROR, LogLevel.FATAL };
        bool[] toolbarState = new bool[] { false, true, true, true, true, false, };
        int selectLog = -1;

        GUILayoutOption[] configuration;

        Texture2D background;

        int GUIScale = 1;

        int winStart = 0; // 0 最小化 1 最大化

        private void InitializeStyle()
        {
            Texture2D background = new Texture2D(128, 128, TextureFormat.ARGB32, true);

            toolbarStyle = new GUIStyle();
            background.SetColor(33f);
            toolbarStyle.normal.background = background;
            toolbarStyle.normal.textColor = Color.red;

            background.SetColor(74f);
            toolbarStyle.active.background = background;
            toolbarStyle.active.textColor = new Color(1, 1, 0);
            toolbarStyle.active.background = background;
            toolbarStyle.hover.textColor = new Color(1, 1, 1);
            toolbarStyle.fontSize = 30;
            toolbarStyle.fixedWidth = 100;

            Color fontColor = new Color(178 / 255f, 178 / 255f, 178 / 255f, 1);
            Color logColor = new Color(62 / 255f, 85 / 255f, 150 / 255f, 1);

            LogStyle1 = new GUIStyle();

            background = new Texture2D(128, 128, TextureFormat.ARGB32, true);
            background.SetColor(44f);

            LogStyle1.normal.background = background;
            LogStyle1.normal.textColor = LogStyle1.active.textColor = LogStyle1.hover.textColor = LogStyle1.focused.textColor = fontColor;

            background = new Texture2D(128, 128, TextureFormat.ARGB32, true);
            background.SetColor(logColor);

            LogStyle1.onNormal.background = background;
            LogStyle1.fontSize = 13;
            LogStyle1.padding = new RectOffset(10, 0, 1, 1);

            LogStyle2 = new GUIStyle();

            background = new Texture2D(128, 128, TextureFormat.ARGB32, true);
            background.SetColor(48f);

            LogStyle2.normal.background = background;
            LogStyle2.normal.textColor = LogStyle2.active.textColor = LogStyle2.hover.textColor = LogStyle2.focused.textColor = fontColor;

            background = new Texture2D(128, 128, TextureFormat.ARGB32, true);
            background.SetColor(logColor);

            LogStyle2.onNormal.background = background;
            LogStyle2.fontSize = 13;
            LogStyle2.padding = new RectOffset(10, 0, 1, 1);

            SplitterStyle = new GUIStyle();
            background = new Texture2D(128, 128, TextureFormat.Alpha8, true);
            SplitterStyle.normal.background = background;

            fontStyle = new GUIStyle();

            background = new Texture2D(128, 128, TextureFormat.ARGB32, true);
            background.SetColor(44f);
            fontStyle.normal.background = background;    //设置背景填充
            fontStyle.normal.textColor = new Color(1, 0, 0);   //设置字体颜色
            fontStyle.fontSize = 30;       //字体大小

            configuration = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Height(40) };

            buttonStyle = new GUIStyle() ;

            buttonStyle.normal.background = background;
            buttonStyle.fontSize = 30;

            titleBar_butArea = new Rect(windowRect.width - 200, 0, 200, 20);
        }

        private void Awake()
        {
            winStart = 2;

            GUIScale = 3;

            windowRect = new Rect(margin, margin, (Screen.width - (margin * 2)) / GUIScale, (Screen.height - (margin * 2)) / GUIScale);

            windowRectMin = new Rect(margin, margin, 120, 80);

            //butArea = new Rect((Screen.width - (margin * 2)) / GUIScale-200,0,200,40);

            logLevels = new List<LogLevel>();

            for (int i = 0; i < toolbarState.Length; i++)
            {
                SetLogOutput(toolbarStr[i], toolbarState[i]);
            }

            InitializeStyle();

            CursorsControl.Instance.SetCursor(CursorType.Arrow);
        }

        public void Log(LogData logData)
        {
            logDatas.Add(logData);
        }

        private void OnGUI()
        {
            if (winStart == 1)
            {
                windowRectMin = GUILayout.Window(999, windowRectMin, windowMinimality, "", fontStyle);
            }
            else if (winStart == 2)
            {
                windowRect = GUILayout.Window(666, windowRect, DrawConsolWindow, "", fontStyle);
            }
        }

        GUIStyle buttonStyle;

        Rect titleBar_dragArea;

        private void windowMinimality(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(" ━ ", GUI.skin.button, null))
            {
                winStart = 1;
            }
            if (GUILayout.Button(" ロ ", GUI.skin.button, null))
            {
                winStart = 2;
            }
            if (GUILayout.Button(" ✖ ", GUI.skin.button, null))
            {
                winStart = 0;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("LogConsole", buttonStyle,null);

            GUILayout.EndVertical();
            titleBar_dragArea = GUILayoutUtility.GetLastRect();

            if (titleBar_dragArea.size.x == 1) return;

            if (titleBar_dragArea.Contains(Event.current.mousePosition))
            {
                Cursors.SetCursor(CursorType.SizeNS);
                if (Event.current.rawType == EventType.MouseDown)
                {
                    dragging = true;
                }
            }
            else
            {
                if (!dragging)
                {
                    Cursors.SetCursor(CursorType.Arrow);
                }
            }

        }

        private void DrawConsolWindow(int id)
        {
            DrawToolbar();
            DrawLog();
        }

        /// <summary>  
        /// 绘制日志工具栏
        /// </summary>  
        private void DrawToolbar()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUILayout.Label("LogConsole", buttonStyle, null);
            GUILayout.BeginArea(titleBar_butArea);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(" ━ ", GUI.skin.button, null))
            {
                winStart = 1;
            }
            if (GUILayout.Button(" ロ ", GUI.skin.button, null))
            {
                winStart = 2;
            }
            if (GUILayout.Button(" ✖ ", GUI.skin.button, null))
            {
                winStart = 0;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            GUILayout.EndHorizontal();

            //GUILayout.Space(50);splitterRect = GUILayoutUtility.GetLastRect();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Clear", GUI.skin.button, configuration))
            {
                logDatas.Clear();
                logDatas = null;
                logDatas = new List<LogData>();

                detailedLog = "";
            }

            if (GUILayout.Button("All", GUI.skin.button, configuration))
            {
                for (int i = 0; i < toolbarStr.Length; i++)
                {
                    if (!toolbarState[i])
                    {
                        toolbarState[i] = true;
                        SetLogOutput(toolbarStr[i], toolbarState[i]);
                    }
                }
            }
            for (int i = 0; i < toolbarStr.Length; i++)
            {
                GUILayout.Space(10);
                if (toolbarState[i] != GUILayout.Toggle(toolbarState[i], toolbarStr[i].ToString(), GUI.skin.toggle, configuration))
                {
                    toolbarState[i] = !toolbarState[i];
                    SetLogOutput(toolbarStr[i], toolbarState[i]);
                }
            }
            if (GUILayout.Button("Off", GUI.skin.button, configuration))
            {
                for (int i = 0; i < toolbarStr.Length; i++)
                {
                    if (toolbarState[i])
                    {
                        toolbarState[i] = false;
                        SetLogOutput(toolbarStr[i], toolbarState[i]);
                    }
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DrawLog()
        {
            GUILayout.BeginVertical();

            posLeft = GUILayout.BeginScrollView(posLeft,
                GUILayout.MinHeight(splitterPos),
                GUILayout.Height(splitterPos),
                GUILayout.MaxHeight(splitterPos),
                GUILayout.ExpandWidth(true));

            int logStart = 0;

            for (int i = 0; i < logDatas.Count; i++)
            {
                LogData logData = logDatas[i];

                if (!logLevels.Contains(logData.logLevel)) continue;
                logStart += 1;
                if (GUILayout.Toggle(selectLog == i, string.Format("[{0}] [{1}] {2}\n{3}", logData.logTime.ToString(TIME_FORMATER), logData.logLevel, logData.logMessage, logData.logBasicData), logStart % 2 == 0 ? LogStyle1 : LogStyle2, GUILayout.ExpandWidth(true), GUILayout.Height(logHeight)))
                {
                    if (selectLog == i) continue;
                    selectLog = i;
                    detailedLog = string.Format("[{0}] [{1}] {2}\n{3}\n{4}", logData.logTime.ToString(TIME_FORMATER), logData.logLevel, logData.logMessage, logData.logBasicData, logData.logTrack);
                }
            }

            GUILayout.EndScrollView();

            // 分割线
            GUILayout.Box("", SplitterStyle,
                GUILayout.Height(6),
                GUILayout.ExpandWidth(true));
            splitterRect = GUILayoutUtility.GetLastRect();

            // 显示详细信息
            posRight = GUILayout.BeginScrollView(posRight,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));
            GUILayout.Label(detailedLog, LogStyle1);
            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            if (Event.current != null)
            {
                if (Event.current.rawType == EventType.MouseUp)
                {
                    if (dragging)
                    {
                        dragging = false;
                        Cursors.SetCursor(CursorType.Arrow);
                    }
                }

                if (dragging)
                {
                    splitterPos = Event.current.mousePosition.y - 83;
                    if (splitterPos >= windowRect.height - logHeight - 110)
                        splitterPos = windowRect.height - logHeight - 110;

                    if (splitterPos <= logHeight) splitterPos = logHeight;
                }

                if (splitterRect.size.x == 1) return;

                if (splitterRect.Contains(Event.current.mousePosition))
                {
                    Cursors.SetCursor(CursorType.SizeNS);
                    if (Event.current.rawType == EventType.MouseDown)
                    {
                        dragging = true;
                    }
                }
                else
                {
                    if (!dragging)
                    {
                        Cursors.SetCursor(CursorType.Arrow);
                    }
                }
            }
            else
            {
                if (dragging)
                {
                    dragging = false;
                }
            }
        }

        bool mouseState;

        bool dragging;
        Vector2 posLeft;
        Vector2 posRight;
        Rect splitterRect;
        float splitterPos;

        Vector2 scrollPosition;

        List<LogLevel> logLevels;

        private void SetLogOutput(LogLevel level, bool state)
        {
            if (logLevels.Contains(level) == state) return;

            switch (state)
            {
                case true:
                    logLevels.Add(level);
                    break;
                case false:
                    logLevels.Remove(level);
                    break;
            }

            detailedLog = "";
        }
    }
}