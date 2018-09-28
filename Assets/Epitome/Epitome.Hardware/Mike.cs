/*----------------------------------------------------------------
 * 文件名：Mike
 * 文件功能描述：麦克风
----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Epitome.Hardware
{
    //===============命名空间=========================
    using Utility;
    //===============命名空间=========================

    /// <summary>
    /// 麦克风
    /// </summary>
    public class Mike
    {
        static Mike mInstance;

        public static Mike GetSingleton() { if (mInstance == null) { mInstance = new Mike(); } return mInstance; }

        //++++++++++++++++++++     调用麦克风     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 打开麦克风.
        /// </summary>
        public void OpenMicrophone(AudioSource varAudioSource, int varDuration=60, bool varLoop=false,int varFrequency= 44100)
        {
            string[] tempDevices = Microphone.devices;

            ShutDownMike();

            if (tempDevices.Length != 0) { varAudioSource.clip = Microphone.Start(null, varLoop, varDuration, varFrequency); }
        }

        /// <summary>
        /// 关闭麦克风.
        /// </summary>
        public void ShutDownMike() { Microphone.End(null); }
    }
}
