/*----------------------------------------------------------------
 * 文件名：External
 * 文件功能描述：外部
----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Epitome.Utility
{
    public class External
    {
        static External mInstance;

        public static External GetSingleton() { if (mInstance == null) { mInstance = new External(); } return mInstance; }


        //=================              ======================

        /// <summary>
        /// 启动外部软件
        /// </summary>
        public void StartExternalSoftware(string varURL)
        {
            Application.OpenURL(varURL);
        }
    }
}