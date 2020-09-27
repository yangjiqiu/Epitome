/* $Header:   Assets/Epitome/Epitome.LogSystem/Appender/MobileGUIAppender.cs   1.0   2020/06/28 Sunday AM 11:48:52   Ji Qiu .Yang   2017.1.1f1  $ */
/***********************************************************************************************
 ***              C O N F I D E N T I A L  ---  E P I T O M E  S T U D I O S                 ***
 ***********************************************************************************************
 *                                                                                             *
 *                 Project Name : Epitome                                                      *
 *                                                                                             *
 *                    File Name : MobileGUIAppender.cs                                         *
 *                                                                                             *
 *                   Programmer : Ji Qiu .Yang                                                 *
 *                                                                                             *
 *                   Start Date : 2020/06/28                                                   *
 *                                                                                             *
 *                  Last Update : 2020/06/28                                                   *
 *                                                                                             *
 *---------------------------------------------------------------------------------------------*
 * Functions:                                                                                  *
 * - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Epitome.LogSystem
{
    public class MobileGUIAppender : AsyncTask<MobileGUIAppender>, ILogAppender
    {
        protected const string TIME_FORMATER = "HH:mm:ss,fff";

        // 日志级别
        private LogLevel[] toolbarStr = new LogLevel[] { LogLevel.TRACE, LogLevel.DEBUG, LogLevel.INFO, LogLevel.WARN, LogLevel.ERROR, LogLevel.FATAL };
        private List<LogLevel> logLevels;  // 日志输出级别
        private bool[] toolbarState = new bool[] { false, true, true, true, true, false };  // 工具栏开关状态
        //private int[] logLevelCount = new int[] { 0, 0, 0, 0, 0, 0, };  // 日志各个级别数量
        private int[] logLevelCount = new int[] { 0, 0, 0, 0, 0, 0 };  // 日志各个级别数量

        private List<LogData> logDatas = new List<LogData>();             // 日志数据集合
        private List<LogData> logCollapseDatas = new List<LogData>();     // 折叠日志数据集合
        private List<int> logCollapseCount = new List<int>();             // 折叠日志数量

        Texture2D Texture2D_White;   // 白色
        Texture2D Texture2D_Gray;    // 灰色
        Texture2D Texture2D_Black;   // 黑色

        Texture2D[] LogLevelIcons;   // 日志级别图标

        Texture2D LogCountIcon;   // 日志数量图标

        int GUIScale = 1;

        int winStart = 0; // 0 最小化 1 最大化

        private float scaleFactor = 1f;   // 屏幕缩放因子




        private void Awake()
        {
            DontDestroyOnLoad();

            winStart = 2;

            GUIScale = 1;

            logLevels = new List<LogLevel>();

            for (int i = 0; i < toolbarState.Length; i++)
            {
                SetLogOutput(toolbarStr[i], toolbarState[i]);
            }

            Texture2D_White = new Texture2D(128, 128, TextureFormat.ARGB32, false);
            Texture2D_White.SetColor(255);

            Texture2D_Gray = new Texture2D(128, 128, TextureFormat.ARGB32, false);
            Texture2D_Gray.SetColor(100);

            Texture2D_Black = new Texture2D(128, 128, TextureFormat.ARGB32, false);
            Texture2D_Black.SetColor(Color.black);

        }

        private void Start()
        {
            LogLevelIcons = Logging.Instance.logSystemAsset.LogLevelIcons;
            LogCountIcon = Logging.Instance.logSystemAsset.LogCountIcon;
            InitStyle();
        }

        public void Log(LogData logData)
        {
            logLevelCount[(int)logData.logLevel - 1] += 1;
            logDatas.Add(logData);

            for (int i = 0; i < logCollapseDatas.Count; i++)
            {
                if (logCollapseDatas[i].logLevel.Equals(logData.logLevel))
                {
                    if (logCollapseDatas[i].logMessage.Equals(logData.logMessage))
                    {
                        if (logCollapseDatas[i].logTrack.Equals(logData.logTrack))
                        {
                            logCollapseCount[i] += 1;

                            return;

                        }
                        else continue;
                    }
                    else continue;
                }
                else continue;
            }

            logCollapseDatas.Add(logData);
            logCollapseCount.Add(1);
        }

        private void OnGUI()
        {
            if (winStart == 1)
            {
                windowRectMin = GUILayout.Window(999, windowRectMin, windowMinimality, "", windowStyle);
            }
            else if (winStart == 2)
            {
                windowRect = GUILayout.Window(666, windowRect, DrawConsolWindow, "", windowStyle);
            }
        }

        /// <summary>
        /// 初始化样式
        /// </summary>
        private void InitStyle()
        {
            InitWindowStyle();
            InitTitleBarStyle();
            InitMenuBarStyle();
            InitLogLevelStyle();
            InitLogStyle();
            InitDetailsStyle();
        }

        /// <summary>
        /// 设置日志输出
        /// </summary>
        /// <param name="level"></param>
        /// <param name="state"></param>
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

        #region 绘制窗口
        // 窗口样式
        private const int margin = 0;

        private Rect windowRect;
        private Rect windowRectMin;

        private GUIStyle windowStyle;

        private GUIStyle splitLineStyle;

        /// <summary>
        /// 初始化窗口样式
        /// </summary>
        private void InitWindowStyle()
        {
            windowRect = new Rect(margin, margin, (Screen.width - (margin * 2)) / GUIScale, (Screen.height - (margin * 2)) / GUIScale);

            windowRectMin = new Rect(margin, margin, (Screen.width - (margin * 2)) / GUIScale, 50);

            windowStyle = new GUIStyle();
            windowStyle.normal.background = Texture2D_White;
            windowStyle.fontStyle = FontStyle.Bold;
            windowStyle.fontSize = 30;

            splitLineStyle = new GUIStyle();
            splitLineStyle.normal.background = Texture2D_Black;
        }

        private void windowMinimality(int id)
        {
            DrawTitleBar();
        }

        private void DrawConsolWindow(int id)
        {
            DrawTitleBar();
            DrawMenuBar();
            DrawLogLevel();
            DrawLog();
            GUILayout.FlexibleSpace();
            DrawDetails();
        }

        /// <summary>
        /// 绘制分隔线横向
        /// </summary>
        private void DrawSplitLineHorizontal()
        {
            GUILayout.Box(GUIContent.none, splitLineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(2f));
        }

        /// <summary>
        /// 绘制分隔线纵向
        /// </summary>
        private void DrawSplitLineVertical()
        {
            GUILayout.Box(GUIContent.none, splitLineStyle, GUILayout.Width(2), GUILayout.ExpandHeight(true));
        }

        #endregion

        #region 绘制标题栏

        private float titleBarHeight = 80;       // 标题栏高度

        private GUIStyle headingFontStyle;       // 标题字体样式

        private GUIStyle titleBarButtonStyle;    // 标题栏按钮样式

        private GUILayoutOption[] ltitleBarButtonOptions; // 标题栏按钮布局选项

        /// <summary>
        /// 初始化标题栏样式
        /// </summary>
        private void InitTitleBarStyle()
        {
            headingFontStyle = new GUIStyle() ;
            headingFontStyle.fontStyle =  FontStyle.Bold;
            headingFontStyle.fontSize = 30;
            headingFontStyle.alignment = TextAnchor.MiddleCenter;

            titleBarButtonStyle = new GUIStyle();
            titleBarButtonStyle.fontSize = 30;
            titleBarButtonStyle.alignment = TextAnchor.MiddleCenter;
            titleBarButtonStyle.hover.background = Texture2D_Gray;
            titleBarButtonStyle.focused.background = Texture2D_Gray;

            ltitleBarButtonOptions = new GUILayoutOption[] { GUILayout.Width(titleBarHeight * scaleFactor * 1.6f), GUILayout.Height(titleBarHeight * scaleFactor) };
        }

        /// <summary>
        /// 绘制标题栏
        /// </summary>
        private void DrawTitleBar()
        {
            GUILayout.BeginVertical(GUILayout.Height(titleBarHeight * scaleFactor));

            GUILayout.BeginHorizontal();

            GUILayout.Space(20);

            GUILayout.Label("Epitome.LogSystem", headingFontStyle, GUILayout.Height(titleBarHeight * scaleFactor));

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(" ━ ", titleBarButtonStyle, ltitleBarButtonOptions))
            {
                winStart = 1;
            }
            if (GUILayout.Button(" ロ ", titleBarButtonStyle, ltitleBarButtonOptions))
            {
                winStart = 2;
            }
            if (GUILayout.Button(" ✖ ", titleBarButtonStyle, ltitleBarButtonOptions))
            {
                winStart = 0;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        #endregion


        #region 绘制菜单栏

        // 菜单栏样式
        private float menuBarHeight = 80;
        private GUIStyle menuBarButtonStyle;             // 菜单栏按钮样式
        private GUILayoutOption[] menuBarButtonOptions;  // 菜单栏按钮布局选项
        private GUIStyle menuBarToggleStyle;             // 菜单栏开关样式
        //private GUIStyle menuBarIconToggleStyle;         // 菜单栏带图标开关样式
        //private GUIStyle menuBarIconStyle;               // 菜单栏图标样式

        private bool isCollapse;              // 是否折叠
        private bool isErrorPause;            // 是否错误停止
        /// <summary>
        /// 初始化菜单栏样式
        /// </summary>
        private void InitMenuBarStyle()
        {
            menuBarButtonStyle = new GUIStyle();
            menuBarButtonStyle.fontSize = 25;
            menuBarButtonStyle.alignment = TextAnchor.MiddleCenter;
            //menuBarButtonStyle.hover.background = Texture2D_Gray;
            menuBarButtonStyle.active.background = Texture2D_Gray;
            menuBarButtonStyle.padding = new RectOffset(10, 10, 0, 0);

            menuBarToggleStyle = new GUIStyle();
            menuBarToggleStyle.fontSize = 25;
            menuBarToggleStyle.alignment = TextAnchor.MiddleCenter;
            menuBarToggleStyle.onNormal.background = Texture2D_Gray;
            menuBarToggleStyle.padding = new RectOffset(10, 10, 0, 0);

            //menuBarIconToggleStyle = new GUIStyle(menuBarToggleStyle);
            //menuBarIconToggleStyle.padding = new RectOffset((int)menuBarHeight, 10, 0, 0);

            //menuBarIconStyle = new GUIStyle();
            //menuBarIconStyle.alignment = TextAnchor.MiddleLeft;
            //menuBarIconStyle.padding = new RectOffset(10, 0, 10, 10);

            menuBarButtonOptions = new GUILayoutOption[] {GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true) };
        }

        /// <summary>  
        /// 绘制菜单栏
        /// </summary>  
        private void DrawMenuBar()
        {
            GUILayout.BeginVertical();

            DrawSplitLineHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Height(menuBarHeight * scaleFactor));

            GUILayout.Space(20);

            DrawSplitLineVertical();

            if (GUILayout.Button("Clear", menuBarButtonStyle, menuBarButtonOptions))
            {
                logDatas.Clear();
                logDatas = null;
                logDatas = new List<LogData>();

                logLevelCount = new int[] { 0, 0, 0, 0, 0, 0 };

                detailedLog = "";
            }

            DrawSplitLineVertical();

            if (isCollapse != GUILayout.Toggle(isCollapse,"Collapse", menuBarToggleStyle, menuBarButtonOptions))
            {
                isCollapse = !isCollapse;
            }

            DrawSplitLineVertical();

            if (isErrorPause != GUILayout.Toggle(isErrorPause, "Error Pause", menuBarToggleStyle, menuBarButtonOptions))
            {
                isErrorPause = !isErrorPause;
            }

            DrawSplitLineVertical();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        #endregion

        #region 绘制日志级别

        // 日志级别样式
        private float logLevelHeight = 80;
        private GUIStyle logLevelButtonStyle;             // 日志级别按钮样式
        private GUILayoutOption[] logLevelButtonOptions;  // 日志级别按钮布局选项
        private GUIStyle logLevelIconToggleStyle;         // 日志级别带图标开关样式
        private GUIStyle logLevelIconStyle;               // 日志级别图标样式
        private GUIStyle logLevelStyle;

        /// <summary>
        /// 初始化日志级别样式
        /// </summary>
        private void InitLogLevelStyle()
        {
            logLevelStyle = new GUIStyle();
            logLevelStyle.alignment = TextAnchor.MiddleCenter;

            logLevelButtonStyle = new GUIStyle();
            logLevelButtonStyle.fontSize = 25;
            logLevelButtonStyle.alignment = TextAnchor.MiddleCenter;
            logLevelButtonStyle.active.background = Texture2D_Gray;
            logLevelButtonStyle.padding = new RectOffset(10, 10, 0, 0);

            logLevelIconToggleStyle = new GUIStyle();
            logLevelIconToggleStyle.fontSize = 25;
            logLevelIconToggleStyle.alignment = TextAnchor.MiddleCenter;
            logLevelIconToggleStyle.onNormal.background = Texture2D_Gray;
            logLevelIconToggleStyle.padding = new RectOffset((int)menuBarHeight, 10, 0, 0);

            logLevelIconStyle = new GUIStyle();
            logLevelIconStyle.alignment = TextAnchor.MiddleLeft;
            logLevelIconStyle.padding = new RectOffset(10, 0, 10, 10);

            logLevelButtonOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true) };
        }

        /// <summary>  
        /// 绘制日志级别
        /// </summary>  
        private void DrawLogLevel()
        {
            GUILayout.BeginVertical(logLevelStyle);

            DrawSplitLineHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Height(logLevelHeight * scaleFactor));

            GUILayout.FlexibleSpace();

            DrawSplitLineVertical();

            if (GUILayout.Button("All", menuBarButtonStyle, menuBarButtonOptions))
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
                DrawSplitLineVertical();

                if (toolbarState[i] != GUILayout.Toggle(toolbarState[i], logLevelCount[i].ToString(), logLevelIconToggleStyle, logLevelButtonOptions))
                {
                    toolbarState[i] = !toolbarState[i];
                    SetLogOutput(toolbarStr[i], toolbarState[i]);
                }
                GUI.Label(GUILayoutUtility.GetLastRect(), LogLevelIcons[i], logLevelIconStyle);
            }

            DrawSplitLineVertical();

            if (GUILayout.Button("Off", logLevelButtonStyle, logLevelButtonOptions))
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

            DrawSplitLineVertical();

            GUILayout.Space(20);

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        #endregion

        #region 绘制日志
        private GUIStyle logStyle;           // 日志正常样式
        private float logHeight = 32;        // 日志高度
        private string detailedLog;          // 日志详细信息
        private int selectLog = -1;          // 当前选中日志

        private int lorFontSize = 40;        // 日志字体大小
        private Color logFontColor;          // 日志字体颜色
        private Color logColor;              // 日志背景颜色

        private Vector2 logScrollPos;        // 日志滚动位置

        private GUIStyle logIconStyle;       // 日志级别图标样式

        private GUIStyle logCollapseCountStyle;       // 日志折叠数量样式

        /// <summary>
        /// 初始化日志样式
        /// </summary>
        private void InitLogStyle()
        {
            logHeight = lorFontSize * 2 + 6 + 10;
            logFontColor = Color.black;
            logColor = Color.white;

            logStyle = new GUIStyle();

            logStyle.normal.background = Texture2D_White;
            logStyle.onNormal.background = Texture2D_Gray;
            logStyle.fontSize = lorFontSize;
            logStyle.padding = new RectOffset((int)logHeight, 0, 5, 5);

            logIconStyle = new GUIStyle();
            logIconStyle.alignment = TextAnchor.MiddleLeft;
            logIconStyle.padding = new RectOffset(10, 0, 10, 10);

            logCollapseCountStyle = new GUIStyle();
            logCollapseCountStyle.fontSize = 30;
            logCollapseCountStyle.alignment = TextAnchor.MiddleRight;
            logCollapseCountStyle.normal.background = LogCountIcon;
            logCollapseCountStyle.normal.textColor = Color.white;
            logCollapseCountStyle.padding = new RectOffset(30, 30, 0, 0);  // 内边距
                                                                           //logCollapseCountStyle.border = new RectOffset(10, 10, 0, 0);  // 外边距
                                                                           //logCollapseCountStyle.margin = new RectOffset(20, 20, 20, 20);  // 边缘
                                                                           //logCollapseCountStyle.overflow = new RectOffset(100, 10, 10, 10);  // 溢出
            logCollapseCountStyle.fixedWidth = 100;
            logCollapseCountStyle.fixedHeight = 80;
            logCollapseCountStyle.stretchWidth = true;
            //  
            
            
            
            
            logCollapseCountStyle.CalcMinMaxWidth 
            //logCollapseCountStyle.CalcScreenSize( new Vector2(30,30));
        } 

        /// <summary>
        /// 绘制日志
        /// </summary>
        private void DrawLog()
        {
            GUILayout.BeginVertical();

            DrawSplitLineHorizontal();

            logScrollPos = GUILayout.BeginScrollView(logScrollPos,
                GUILayout.ExpandWidth(true));

            int logStart = 0;

            if (!isCollapse)
            {
                for (int i = 0; i < logDatas.Count; i++)
                {
                    LogData logData = logDatas[i];

                    if (!logLevels.Contains(logData.logLevel)) continue;
                    logStart += 1;
                    if (GUILayout.Toggle(selectLog == i, string.Format("[{0}] [{1}] {2}\n{3}", logData.logTime.ToString(TIME_FORMATER), logData.logLevel, logData.logMessage, logData.logBasicData), logStyle, GUILayout.ExpandWidth(true), GUILayout.Height(logHeight)))
                    {
                        if (selectLog != i)
                        {
                            selectLog = i;
                            detailedLog = string.Format("[{0}] [{1}] {2}\n{3}\n{4}", logData.logTime.ToString(TIME_FORMATER), logData.logLevel, logData.logMessage, logData.logBasicData, logData.logTrack);
                        }
                    }
                    GUI.Label(GUILayoutUtility.GetLastRect(), LogLevelIcons[(int)logData.logLevel - 1], logIconStyle);
                }
            }
            else
            {
                for (int i = 0; i < logCollapseDatas.Count; i++)
                {
                    LogData logData = logCollapseDatas[i];

                    if (!logLevels.Contains(logData.logLevel)) continue;
                    logStart += 1;
                    if (GUILayout.Toggle(selectLog == i, string.Format("[{0}] [{1}] {2}\n{3}", logData.logTime.ToString(TIME_FORMATER), logData.logLevel, logData.logMessage, logData.logBasicData), logStyle, GUILayout.ExpandWidth(true), GUILayout.Height(logHeight)))
                    {
                        if (selectLog != i)
                        {
                            selectLog = i;
                            detailedLog = string.Format("[{0}] [{1}] {2}\n{3}\n{4}", logData.logTime.ToString(TIME_FORMATER), logData.logLevel, logData.logMessage, logData.logBasicData, logData.logTrack);
                        }
                    }
                    Rect rect = GUILayoutUtility.GetLastRect();

                    GUI.Label(rect, LogLevelIcons[(int)logData.logLevel - 1], logIconStyle);

                    Debug.Log(logScrollPos);

                    rect.x += logScrollPos.x + Screen.width - 300;

                    //GUILayout.Label(logCollapseCount[i].ToString(), logLevelIconToggleStyle, logLevelButtonOptions);
                    //GUILayout.Label(LogLevelIcons[(int)logData.logLevel - 1], logCollapseCountStyle);
                    GUI.Label(rect, logCollapseCount[i].ToString(), logCollapseCountStyle);
                }
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
        #endregion

        #region 绘制详细信息

        private GUIStyle detailsStyle;         // 详细信息样式

        private Vector2 detailsScrollPos;      // 日志滚动位置

        /// <summary>
        /// 初始化详细信息样式
        /// </summary>
        private void InitDetailsStyle()
        {
            detailsStyle = new GUIStyle(logStyle);
            detailsStyle.padding = new RectOffset(0, 0 ,5, 5);
        }

        /// <summary>
        /// 绘制详细信息
        /// </summary>
        private void DrawDetails()
        {
            GUILayout.BeginVertical(GUILayout.Height(250));

            DrawSplitLineHorizontal();

            detailsScrollPos = GUILayout.BeginScrollView(detailsScrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            GUILayout.Label(detailedLog, detailsStyle);

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        #endregion
    }
}
