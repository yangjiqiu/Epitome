/*----------------------------------------------------------------
 * 文件名：Webcam
 * 文件功能描述：摄像头
----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Epitome.Hardware
{
    /// <summary>
    /// 摄像头
    /// </summary>
    public class Webcam
    {
        static Webcam mInstance;

        public static Webcam GetSingleton() { if (mInstance == null) { mInstance = new Webcam(); } return mInstance; }

        //++++++++++++++++++++     调用摄像头     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        WebCamTexture mWebCam;

#if NGUI
        /// <summary>
        /// 打开摄像头.
        /// </summary>
        public IEnumerator OpenWebcam(UITexture varTexture)
        {
            ShutDownWebcam();

            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                WebCamDevice[] tempDevices = WebCamTexture.devices;
                string tempDeviceName = tempDevices[0].name;
                mWebCam = new WebCamTexture(tempDeviceName, Screen.width, Screen.height);
                varTexture.mainTexture = mWebCam;
                mWebCam.Play();
            }
        }
#endif
        /// <summary>
        /// 打开摄像头.
        /// </summary>
        public IEnumerator OpenWebcam(Renderer varRenderer)
        {
            ShutDownWebcam();

            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                WebCamDevice[] tempDevices = WebCamTexture.devices;
                string tempDeviceName = tempDevices[0].name;
                mWebCam = new WebCamTexture(tempDeviceName, Screen.width, Screen.height);
                varRenderer.material.mainTexture = mWebCam;
                mWebCam.Play();
            }
        }

        /// <summary>
        /// 关闭摄像头.
        /// </summary>
        public void ShutDownWebcam() { if (mWebCam != null) mWebCam.Stop(); }
    }
}
