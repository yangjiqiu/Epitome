/*
 * 文件名：NetWork
 * 文件功能描述：网络
----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace  Epitome.Utility
{
    public class NetWork
	{
        static NetWork mInstance;

        public static NetWork GetSingleton() { if (mInstance == null) { mInstance = new NetWork(); } return mInstance; }

        //++++++++++++++++++++     检测当前网络环境     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 当前网络环境.
        /// </summary>
        public NetWorkType NetWorkState()
		{
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) { return NetWorkType.GPRS; }
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork) { return NetWorkType.WiFi; }
            else  return NetWorkType.NO;
		}

		/// <summary>
		/// 网络延迟检测(越小网络越好).
		/// </summary>
		public int NetWorkPing(string varIP)
		{
			Ping tempPing=new Ping (varIP);
			while(true)
			{
				if(tempPing!=null && tempPing.isDone)
				{
					int Time;
					Time = tempPing.time;
					tempPing.DestroyPing ();
					tempPing = null;
					return Time;//单位 ms
				}
			}
		}
	}

    /// <summary>
    /// 网络状态.
    /// </summary>
    public enum NetWorkType
    {
        /// <summary>
        /// 无连接.
        /// </summary>
        NO,

        /// <summary>
        /// GPRS网络.
        /// </summary>
        GPRS,

        /// <summary>
        /// WiFi网络.
        /// </summary>
        WiFi,
    }
}