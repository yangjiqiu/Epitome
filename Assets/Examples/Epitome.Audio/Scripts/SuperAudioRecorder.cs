 /* $Header:   Assets/Epitome/Epitome.Audio/Demo/Audio/Scripts/SuperAudioRecorder.cs   1.0   2020/06/28 Sunday AM 10:11:35   Ji Qiu .Yang   2017.1.1f1  $ */
/***********************************************************************************************
 ***              C O N F I D E N T I A L  ---  E P I T O M E  S T U D I O S                 ***
 ***********************************************************************************************
 *                                                                                             *
 *                 Project Name : Epitome                                                      *
 *                                                                                             *
 *                    File Name : SuperAudioRecorder.cs                                        *
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
using System.IO;
using Epitome.Audio;
using Epitome.LogSystem;

namespace Epitome.Demo
{
    public class SuperAudioRecorder : MonoBehaviour
    {
        public AudioSource accompany;   // 伴奏
        public AudioSource recording;   // 录制

        private string accompanyPath;   // 伴奏音频路径
        private string originalPath;    // 录音原声路径
        private string accompanyClipPath;    // 伴奏剪辑音频路径
        private string mixedEffectPath; // 混音效果路径
        private string finalPath;       // 最终效果路径

        public int duration = 60;       // 录制时长 默认：60s
        public bool loop = false;
        public int frequency = 44100;

        private AssetBundle ab;

        IEnumerator LoadABPackage5(string path)
        {
            Log.Debug(path + "/musiclibrary");
            WWW www = WWW.LoadFromCacheOrDownload(path + "/musiclibrary", 1);
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Log.Debug(www.error);
                yield break;
            }

            ab = www.assetBundle;
            AudioClip clip = ab.LoadAsset<AudioClip>("告白气球.wav");
            accompany.clip = clip;
            duration = (int)clip.length;
        }

        private void sdf()
        {
            Log.Debug("sdf");
            Debug.Log("sdf");
        }

        private void Start()
        {

            for (int i = 0; i < 100; i++)
            {
                sdf();
            }


            StartCoroutine(LoadABPackage5(ProjectPath.GetStreamingAssets));
            MusicDataItem musicData = MusicManager.Instance.accompanyData;

            string musicName;

            Log.Debug("开始执行数据加载");
            Log.Debug("musicData" + musicData);
            if (musicData != null)
            {
                // 获取数据
                musicName = musicData.MusicName;
                Log.Debug(musicName);
                DirectoryInfo direction = new DirectoryInfo(musicData.Path);
                Log.Debug(direction);
                FileInfo[] files = direction.GetFiles("*", SearchOption.TopDirectoryOnly);
                Log.Debug(files[0].FullName.ToString());
                accompanyPath = files[0].FullName.ToString();

                Log.Debug("获取数据");
            }
            else
            {
                // 测试数据
                musicName = "往后余生";
                accompanyPath = ProjectPath.GetDataPath + "/Epitome/Epitome.Audio/Demo/Audios/MusicLibrary_WAV/";
                accompanyPath += musicName + ".wav";

                Log.Debug("测试数据");
            }

            // 录音原声路径
            originalPath = ProjectPath.GetDataPath + "/Epitome/Epitome.Audio/Demo/Audios/RecordFile/" + "OriginalSound.wav";
            // 伴奏剪辑路径
            accompanyClipPath = ProjectPath.GetDataPath + "/Epitome/Epitome.Audio/Demo/Audios/RecordFile/" + "AccompanyClip.wav";
            // 混合音频路径
            mixedEffectPath = ProjectPath.GetDataPath + "/Epitome/Epitome.Audio/Demo/Audios/RecordFile/" + "MixedEffect.wav";
            // 最终效果路径
            finalPath = ProjectPath.GetDataPath + "/Epitome/Epitome.Audio/Demo/Audios/RecordFile/";
            finalPath += musicName + ".wav";

            // 加载伴奏音频
            //StartCoroutine(AudioLoad.Load(accompanyPath, AudioClipLoadFinish));
        }

        /// <summary>
        /// 音频加载回调
        /// </summary>
        /// <param name="clip"></param>
        public void AudioClipLoadFinish(AudioClip clip)
        {
            accompany.clip = clip;
            duration = (int)clip.length;
            Log.Debug("音频加载回调");
        }

        /// <summary>
        /// 开始录音
        /// </summary>
        public void BeginRecording()
        {
            // 伴奏开始
            accompany.Play();
            // 打开麦克风开始录音
            AudioRecord.BeginRecording(recording, duration, loop, frequency);
        }

        /// <summary>
        /// 结束录音
        /// </summary>
        public void EndRecording()
        {
            int audioLength = AudioRecord.EndRecording(duration, frequency);
            AudioClip audioClip = AudioClips.CutBlankSection(recording.clip, audioLength, frequency);

            // 保存原声音频
            AudioRecord.SaveAudio(audioClip, originalPath);

            AudioClip audioClip1 = AudioClips.CutBlankSection(accompany.clip, audioLength, frequency);

            // 保存伴奏剪辑音频
            AudioRecord.SaveAudio(audioClip1, accompanyClipPath);

            // 伴奏混合原声
            AudioMixer.AudioMixing(accompanyClipPath, originalPath, finalPath);


            //TimeSpan startSpan = new TimeSpan(0,0,0);
            //TimeSpan endSpan = new TimeSpan(0,0, audioLength);
            //// 剪辑出最终效果
            //AudioClips.TrimWavFile(mixedEffectPath, finalPath, startSpan, endSpan);
        }

        /// <summary>
        /// 实时播放
        /// </summary>
        public void RealtimePlay()
        {
            AudioRecord.RealtimePlay(recording);
        }
    }
}