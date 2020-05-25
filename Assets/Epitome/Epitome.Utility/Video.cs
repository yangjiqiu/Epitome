/*----------------------------------------------------------------
 * 文件名：Video
 * 文件功能描述：视频
----------------------------------------------------------------*/
using UnityEngine.Video;

namespace Epitome.Utility
{
    using Hardware;

    /// <summary>
    /// 视频
    /// </summary>
    public class Video : Singleton<Video>
    {
#if UNITY_ANDROID || UNITY_IPHONE
        /// <summary>
        /// 播放视频(全屏).
        /// </summary>
        public void PlayVideo(string varPath, Color varColor, FullScreenMovieControlMode varFunctions = FullScreenMovieControlMode.Full)
        {
            Handheld.PlayFullScreenMovie(varPath, varColor, varFunctions);
        }
            
#elif UNITY_STANDALONE_WIN
#if NGUI
        /// <summary>
        /// 创建视频.
        /// </summary>
        public void CreateVideo(MovieTexture varMovieTexture, UITexture varUITexture, bool varBool = false)
        {
            varUITexture.mainTexture = varMovieTexture;
            varMovieTexture.loop = varBool;
        }
#endif

#endif


#if UNITY_5_6_OR_NEWER
        public void StopVideo(VideoPlayer videoPlayer) { videoPlayer.Stop(); }

        public void PlayVideo(VideoPlayer videoPlayer) { videoPlayer.renderMode = VideoRenderMode.CameraNearPlane; }
#else
        /// <summary>
        /// 创建视频.
        /// </summary>
        public void CreateVideo(MovieTexture varMovieTexture, Renderer varRenderer, bool varBool = false)
        {
            varRenderer.material.mainTexture = varMovieTexture;
            varMovieTexture.loop = varBool;
        }

        /// <summary>
        /// 播放视频.
        /// </summary>
        public void PlayVideo(MovieTexture varMovieTexture) { varMovieTexture.Play(); }

        /// <summary>
        /// 暂停视频.
        /// </summary>
        public void PauseVideo(MovieTexture varMovieTexture) { varMovieTexture.Pause(); }

        /// <summary>
        /// 停止视频.
        /// </summary>
        public void StopVideo(MovieTexture varMovieTexture) { varMovieTexture.Stop(); }
#endif
    }
}