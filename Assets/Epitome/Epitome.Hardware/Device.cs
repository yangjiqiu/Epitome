/*----------------------------------------------------------------
 * 文件名：Device
 * 文件功能描述：设备
----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Epitome.Hardware
{
    /// <summary>
    /// 设备
    /// </summary>
    public class Device
    {
        static Device mInstance;

        public static Device GetSingleton() { if (mInstance == null) { mInstance = new Device(); } return mInstance; }

        //++++++++++++++++++++     获取设备信息     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 获取唯一码
        /// </summary>
        public string GetUniqueCode()
        {
#if UNITY_ANDROID
            return SystemInfo.deviceUniqueIdentifier;
#elif UNITY_IPHONE
            return SystemInfo.deviceUniqueIdentifier;
#elif UNITY_STANDALONE_WIN
            return SystemInfo.deviceUniqueIdentifier;
#endif
        }

        /// <summary>
        /// 获取全部分辨率
        /// </summary>
        public Resolution[] GetAllResolution() { return Screen.resolutions; }

        /// <summary>
        /// 获取屏幕分辨率
        /// </summary>
        public int[] GetScreenResolution() { return new int[] { Screen.width, Screen.height }; }

        /// <summary>
        /// 获取最大屏幕分辨率
        /// </summary>
        public int[] GetMaxScreenResolution()
        {
            Resolution[] tempRes = GetAllResolution();
            //显示器支持的所有分辨率  
            int tempCount = tempRes.Length;
            //获取屏幕最大分辨率
            int tempResWidth = tempRes[tempCount - 1].width;
            int tempResHeight = tempRes[tempCount - 1].height;

            return new int[] { tempResWidth, tempResHeight };
        }

        //++++++++++++++++++++     控制设备     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 设备振动.
        /// </summary>
        public void DeviceVibration()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            Handheld.Vibrate();
#elif UNITY_STANDALONE_WIN
             
#endif
        }

        //++++++++++++++++++++     设置设备     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 设置分辨率(参数 0:宽度 1:高度 ).
        /// </summary>
        public void SetResolution(int[] varResolution,bool varFullScreen = false)
        {
#if UNITY_ANDROID || UNITY_IPHONE
           
#elif UNITY_STANDALONE_WIN
            Screen.SetResolution(varResolution[0], varResolution[1], varFullScreen);
#endif
        }


        /// <summary>
        /// 设置屏幕全屏.
        /// </summary>
        public void SetFullScreen()
        {
#if UNITY_ANDROID || UNITY_IPHONE

#elif UNITY_STANDALONE_WIN
            SetResolution(GetScreenResolution(), true);
            Screen.fullScreen = true;
#endif
        }
    }
}
