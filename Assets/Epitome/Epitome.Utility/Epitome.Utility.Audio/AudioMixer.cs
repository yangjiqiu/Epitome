using NAudio.Wave;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using NAudio.Mixer;
using NAudio.Wave.SampleProviders;

namespace Epitome.Audio
{
    public static class AudioMixer
    {
        /// <summary>
        /// 音频混合（混音）
        /// 音频必须具有相同的波形格式，并且必须是相同的
        /// </summary>
        /// <param name="filePath1">音频文件路径</param>
        /// <param name="filePath2">音频文件路径</param>
        /// <param name="mixedPath">混合音频输出路径</param>
        public static void AudioMixing(string filePath1, string filePath2,string mixedPath)
        {
            using (var reader1 = new AudioFileReader(filePath1))
            using (var reader2 = new AudioFileReader(filePath2))
            {
                var mixer = new MixingSampleProvider(new[] { reader1, reader2 });
                WaveFileWriter.CreateWaveFile16(mixedPath, mixer);
            }
        }
    }
}