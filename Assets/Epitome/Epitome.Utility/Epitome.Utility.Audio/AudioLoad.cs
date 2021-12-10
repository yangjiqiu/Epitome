using NAudio.Wave;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Epitome.Utility.Audio
{
    public delegate void AudioLoadFinish(AudioClip clip);
    public delegate void AudiosLoadFinish(List<AudioClip> clipa, List<string> audioPaths);

    public static class AudioLoad
    {
        public static IEnumerator Load(string url, AudioLoadFinish finish)
        {
            WWW www = new WWW(@"file:///" + url);
            yield return www;
            finish(www.GetAudioClip());
        }

        public static IEnumerator Load(string[] urls, AudiosLoadFinish finish)
        {
            int amount = 0;
            List<AudioClip> clips = new List<AudioClip>();
            List<string> audioPaths = new List<string>();

            while (urls.Length > amount)
            {
                WWW www = new WWW("file://" + urls[amount]);
                yield return www;
                
                AudioClip clip = www.GetAudioClip();
                www.Dispose();
                if (clip != null)
                {
                    clips.Add(clip);
                    audioPaths.Add(urls[amount]);
                }
                amount += 1;
            }
            finish(clips, audioPaths);
        }
    }
}