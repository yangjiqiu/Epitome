using System.Collections;
using UnityEngine;

namespace Epitome.Hardware
{
    /// <summary>
    /// 摄像头
    /// </summary>
    public static class Webcam
    {
        static WebCamTexture _WebCam;

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
        public static IEnumerator OpenWebcam(Renderer renderer)
        {
            CloseWebcam();

            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                WebCamDevice[] devices = WebCamTexture.devices;
                string deviceName = devices[0].name;
                _WebCam = new WebCamTexture(deviceName, Screen.width, Screen.height);
                renderer.material.mainTexture = _WebCam;
                _WebCam.Play();
            }
        }

        public static void CloseWebcam() { if (_WebCam != null) _WebCam.Stop(); }
    }
}
