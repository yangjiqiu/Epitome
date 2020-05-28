using NAudio.Wave;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Epitome
{
    public class AudioClips
    {
        /// <summary>
        /// 剪切空白部分
        /// </summary>
        public static AudioClip CutBlankSection(AudioClip audioClip, int time, int samplingRate)
        {
            float[] samples_one = new float[audioClip.samples];

            audioClip.GetData(samples_one, 0);

            float[] samples_two = new float[time * samplingRate];

            Array.Copy(samples_one, 0, samples_two, 0, time * samplingRate);

            AudioClip newAudioClip = AudioClip.Create(audioClip.name, samplingRate * time, 1, samplingRate, false);
            newAudioClip.SetData(samples_two, 0);

            return newAudioClip;
        }

        /// <summary>
        /// 修剪音频文件
        /// </summary>
        public static AudioClip TrimAudioFiles(AudioClip audioClip, int startTime, int endTime, int samplingRate)
        {
            float[] samples_one = new float[audioClip.samples];

            audioClip.GetData(samples_one, 0);

            float[] samples_two = new float[(endTime - startTime) * samplingRate];

            Array.Copy(samples_one, endTime * samplingRate, samples_two, startTime * samplingRate, (endTime - startTime) * samplingRate);

            AudioClip newAudioClip = AudioClip.Create(audioClip.name, samplingRate * (endTime - startTime), 1, samplingRate, false);
            newAudioClip.SetData(samples_two, 0);

            return newAudioClip;
        }

        /// <summary>
        /// 修剪Wav文件
        /// </summary>
        /// <param name="filePath">修剪文件路径</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="cutFromStart">开始间隔</param>
        /// <param name="cutFromEnd">结束间隔</param>
        public static void TrimWavFile(string filePath, string savePath, TimeSpan cutFromStart, TimeSpan cutFromEnd)
        {
            using (WaveFileReader reader = new WaveFileReader(filePath))
            {
                using (WaveFileWriter writer = new WaveFileWriter(savePath, reader.WaveFormat))
                {
                    int bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000;

                    int startPos = (int)cutFromStart.TotalMilliseconds * bytesPerMillisecond;
                    startPos = startPos - startPos % reader.WaveFormat.BlockAlign;

                    int endBytes = (int)cutFromEnd.TotalMilliseconds * bytesPerMillisecond;
                    endBytes = endBytes - endBytes % reader.WaveFormat.BlockAlign;
                    int endPos = (int)reader.Length - endBytes;

                    TrimWavFile(reader, writer, startPos, endPos);
                }
            }
        }

        /// <summary>
        /// 修剪Wav文件
        /// </summary>
        private static void TrimWavFile(WaveFileReader reader, WaveFileWriter writer, int startPos, int endPos)
        {
            reader.Position = startPos;
            byte[] buffer = new byte[1024];
            while (reader.Position < endPos)
            {
                int bytesRequired = (int)(endPos - reader.Position);
                if (bytesRequired > 0)
                {
                    int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        /// <summary>
        /// 合并音频
        /// </summary>
        public static AudioClip MergeAudio(int interval, params AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0)
                return null;

            int channels = clips[0].channels;
            int frequency = clips[0].frequency;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                for (int i = 0; i < clips.Length; i++)
                {
                    if (clips[i] == null)
                        continue;

                    clips[i].LoadAudioData();

                    var buffer = clips[i].GetData();

                    memoryStream.Write(buffer, 0, buffer.Length);

                    if (clips.Length - 1 == i)
                        continue;

                    byte[] byteClips = new byte[clips[i].frequency * clips[i].channels * 4 * interval];//合并音频间的间隔
                    memoryStream.Write(byteClips, 0, byteClips.Length);
                }

                var bytes = memoryStream.ToArray();

                var result = AudioClip.Create("Merge", bytes.Length / 4 / channels, channels, 44100, false);

                result.SetData(bytes);

                return result;
            }
        }
    }
}
