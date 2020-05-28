using NAudio.Wave;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Epitome
{
    public delegate void AudioClipLoadFinish(AudioClip clip);

    public static class AudioLoad
    {
        public static IEnumerator Load(string url, AudioClipLoadFinish finish)
        {
            WWW www = new WWW("file://" + url);
            yield return www;
            finish(www.GetAudioClip());
        }
    }
}