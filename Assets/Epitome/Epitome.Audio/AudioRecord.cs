/*----------------------------------------------------------------
 * 文件名：AudioRecord
 * 文件功能描述：录音
----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace Epitome
{
    using Epitome.Utility;
    using Hardware;

    public static class AudioRecord
    {

        public static void BeginRecording(AudioSource audioSource, int duration = 60, bool loop = false, int frequency = 44100, string deviceName = null)
        {
            Mike.OpenMicrophone(audioSource, deviceName, duration, loop, frequency);
        }

        public static int EndRecording(int audioTime = 60, int samplingRate = 44100, string deviceName = null)
        {
            int audioLength;
            int lastPos = Microphone.GetPosition(deviceName);
            if (Microphone.IsRecording(deviceName))
                audioLength = lastPos / samplingRate + 1;
            else
                audioLength = audioTime;

            return audioLength;
        }

        public static void RealtimePlay(AudioSource audioSource,string deviceName = null)
        {
            PlayAudio(audioSource);
            audioSource.timeSamples = Microphone.GetPosition(deviceName);
        }

        public static void PlayAudio(AudioClip audio) { AudioSource.PlayClipAtPoint(audio, Vector3.zero); }
        public static void PlayAudio(AudioClip audio, Vector3 pos) { AudioSource.PlayClipAtPoint(audio, pos); }
        public static void PlayAudio(AudioSource audioSource) { audioSource.Play(); }
        public static void PlayAudio(AudioSource audioSource, float time) { audioSource.PlayScheduled(time); }

        public static void PauseAudio(AudioSource audioSource) { audioSource.Pause(); }

        public static void StopAudio(AudioSource audioSource) { audioSource.Stop(); }

       

        public static void SaveAudio(AudioClip clip, string path)
        {
            Project.CreateDirectory(Path.GetDirectoryName(path));

            using (FileStream fileStream = CreateEmpty(path))
            {
                ConvertAndWrite(fileStream, clip);
                WriteHeader(fileStream, clip);
            }
        }

        /// <summary>
        /// 转换写入音频
        /// </summary>
        static void ConvertAndWrite(FileStream varFileStream, AudioClip tempClip)
        {
            byte[] tempByte = AudioConverter.Convert(tempClip);
            varFileStream.Write(tempByte, 0, tempByte.Length);
        }


        /// <summary>
        /// 创建文件
        /// </summary>
        static FileStream CreateEmpty(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            byte byteFile = new byte();

            for (int i = 0; i < 44; i++)
            {
                fileStream.WriteByte(byteFile);
            }

            return fileStream;
        }

        /// <summary>
        /// 添加报头
        /// </summary>
        static void WriteHeader(FileStream tempFileStream, AudioClip tempClip)
        {
            int tempFrequency = tempClip.frequency;
            int tempChannels = tempClip.channels;
            int tempSamples = tempClip.samples;

            tempFileStream.Seek(0, SeekOrigin.Begin);

            Byte[] tempRiff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            tempFileStream.Write(tempRiff, 0, 4);

            Byte[] tempChunkSize = BitConverter.GetBytes(tempFileStream.Length - 8);
            tempFileStream.Write(tempChunkSize, 0, 4);

            Byte[] tempWave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            tempFileStream.Write(tempWave, 0, 4);

            Byte[] tempFmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            tempFileStream.Write(tempFmt, 0, 4);

            Byte[] tempSubChunk1 = BitConverter.GetBytes(16);
            tempFileStream.Write(tempSubChunk1, 0, 4);

            UInt16 tempOne = 1;
            //UInt16 tempTwo = 2;

            Byte[] tempAudioFormat = BitConverter.GetBytes(tempOne);
            tempFileStream.Write(tempAudioFormat, 0, 2);

            Byte[] tempNumChannels = BitConverter.GetBytes(tempChannels);
            tempFileStream.Write(tempNumChannels, 0, 2);

            Byte[] tempSampleRate = BitConverter.GetBytes(tempFrequency);
            tempFileStream.Write(tempSampleRate, 0, 4);

            Byte[] tempByteRate = BitConverter.GetBytes(tempFrequency * tempChannels * 2);
            tempFileStream.Write(tempByteRate, 0, 4);

            UInt16 temBlockAlign = (ushort)(tempChannels * 2);
            tempFileStream.Write(BitConverter.GetBytes(temBlockAlign), 0, 2);

            UInt16 temBps = 16;
            Byte[] bitsPerSample = BitConverter.GetBytes(temBps);
            tempFileStream.Write(bitsPerSample, 0, 2);

            Byte[] temDatastring = System.Text.Encoding.UTF8.GetBytes("data");
            tempFileStream.Write(temDatastring, 0, 4);

            Byte[] temSubChunk2 = BitConverter.GetBytes(tempSamples * tempChannels * 2);
            tempFileStream.Write(temSubChunk2, 0, 4);
        }
    }
}
