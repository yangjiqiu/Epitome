using UnityEngine;

namespace Epitome.Hardware
{
    /// <summary>
    /// 设备
    /// </summary>
    public static class Device
    {
        /// <summary>
        /// 获取设备唯一码
        /// </summary>
        public static string GetUniqueCode()
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
        /// 显示器支持的所有全屏分辨率，返回分辨率的列表，较低的分辨率排在前面。
        /// </summary>
        public static Resolution[] AllResolution
        {
            get
            {
#if UNITY_ANDROID
                Debug.Log("在Android设备该列表总是为空");
#endif
                return Screen.resolutions;
            }
        }

        private static Resolution? screenResolution;

        /// <summary>
        /// 屏幕分辨率(宽，高)
        /// </summary>
        public static Resolution ScreenResolution
        {
            get
            {
                if (screenResolution == null)
                {
                    Resolution res = new Resolution();
                    res.width = Screen.width;
                    res.height = Screen.height;
                    screenResolution = res;
                }
                return screenResolution.Value;
            }
        }

        /// <summary>
        /// 获取最大屏幕分辨率
        /// </summary>
        public static int[] MaxScreenResolution()
        {
            Resolution[] tempRes = AllResolution;
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
        public static void DeviceVibration()
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
        public static void SetResolution(Resolution resolution,bool varFullScreen = false)
        {
#if UNITY_ANDROID || UNITY_IPHONE
           
#elif UNITY_STANDALONE_WIN
            Screen.SetResolution(resolution.width, resolution.height, varFullScreen);
#endif
        }


        /// <summary>
        /// 设置屏幕全屏.
        /// </summary>
        public static void SetFullScreen()
        {
#if UNITY_ANDROID || UNITY_IPHONE

#elif UNITY_STANDALONE_WIN
            SetResolution(ScreenResolution, true);
            Screen.fullScreen = true;
#endif
        }
    }
}
