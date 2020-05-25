using System.Collections;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Epitome.Utility
{
    public class Window
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        const uint SWP_SHOWWINDOW = 0x0040;
        const int GWL_STYLE = -16;
        const int WS_BORDER = 1;
        const int WS_POPUP = 0x800000;

        const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}  
        const int SW_SHOWMAXIMIZED = 3;//最大化  
        const int SW_SHOWRESTORE = 1;//还原  

        /// <summary>
        /// 设置窗口无边框
        /// </summary>
        public static IEnumerator SetNoBorder(int varWidte, int varHeight)
        {
            bool tempResult = false;

            do
            {
                //显示器支持的所有分辨率  
                int tempCount = Screen.resolutions.Length;
                //获取屏幕最大分辨率
                int tempResWidth = Screen.resolutions[tempCount - 1].width;
                int tempResHeight = Screen.resolutions[tempCount - 1].height;

                int tempWinPosX = tempResWidth / 2 - varWidte / 2;
                int tempWinPosY = tempResHeight / 2 - varHeight / 2;

                SetWindowLong(GetForegroundWindow(), GWL_STYLE, WS_POPUP);
                tempResult = SetWindowPos(GetForegroundWindow(), 0, tempWinPosX, tempWinPosY, varWidte, varHeight, SWP_SHOWWINDOW);
                
                yield return new WaitForSeconds(0.1f);
            } while (!tempResult);
        }

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xf010;
        public const int HTCAPTION = 0x0002;

#if NGUI

        /// <summary>
        /// 拖动窗口
        /// </summary>
        public IEnumerator DragTheWindow()
        {
            bool tempWhetherDrag = false;   //是否拖动窗口

            Vector3 tempV3 = Vector3.zero;

            IntPtr tempHandle = GetForegroundWindow();

            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray tempRay = new Ray(UICamera.mainCamera.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, -10), new Vector3(0, 0, 1));
                    RaycastHit tempRaycastHit;

                    if (Physics.Raycast(tempRay, out tempRaycastHit, 20))
                    {
                        if (tempRaycastHit.collider.tag == "GameController")
                        {
                            tempWhetherDrag = true;
                            tempV3 = Input.mousePosition;
                        }
                    }
                }

                if (tempWhetherDrag)
                {
                    if (Vector3.Distance(tempV3, Input.mousePosition) > 1)
                    {
                        ReleaseCapture(); //释放鼠标捕捉 会使窗口的某些Mouse事件无法响应 
                        SendMessage(tempHandle, 0xA1, 0x02, 0); //发送左键点击的消息至该窗体(标题栏)
                        SendMessage(tempHandle, 0x0202, 0, 0);

                        tempWhetherDrag = false;
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    tempWhetherDrag = false;
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

#endif

        public static void WindowMinimize()
        {
            ShowWindow(GetForegroundWindow(), SW_SHOWMINIMIZED);
        }

        public static void WindowMaximize()
        {
            ShowWindow(GetForegroundWindow(), SW_SHOWMAXIMIZED);
        }

        public static void WindowRestore()
        {
            ShowWindow(GetForegroundWindow(), SW_SHOWRESTORE);
        }

        public static void WindowShutDown()
        {
            Application.Quit();
        }
    }
}