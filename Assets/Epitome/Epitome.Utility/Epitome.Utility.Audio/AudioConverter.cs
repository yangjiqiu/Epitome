using NAudio.Wave;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using NAudio.Mixer;
using NAudio.Wave.SampleProviders;
using Epitome.Utility;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Epitome.Audio
{
    public static class AudioConverter
    {
        /// <summary>
        /// MP3文件转WAV文件
        /// </summary>
        /// <param name="filePath">MP3文件读取路径</param>
        /// <param name="savePath">WAV文件保存路径</param>
        public static void MP3TurnWAV(string filePath, string savePath)
        {
            using (FileStream stream = File.Open(filePath, FileMode.Open))
            {
                Mp3FileReader reader = new Mp3FileReader(stream);
                WaveFileWriter.CreateWaveFile(savePath, reader);
            }
        }

        public static void MP3TurnWAV(string filePath, string savePath, List<string> fileNames)
        {
            List<string> strs = Project.DirectoryAllFileNames(filePath, new List<string>() { "mp3" });

            if (strs != null)
            {
                string targetFilePath, saveFilePath;
                for (int j = 0; j < strs.Count; j++)
                {
                    targetFilePath = filePath + "/" + strs[j];
                    saveFilePath = savePath + "/" + strs[j].Substring(0, strs[j].Length - 3) + "wav";
                    MP3TurnWAV(targetFilePath, saveFilePath);
                }
            }
        }

        /// <summary>
        /// Wav音频转换字节流
        /// </summary>
        public static byte[] WavTurnBytes(string varStr)
        {
            FileStream tempFile = new FileStream(varStr, FileMode.Open);
            byte[] tempBuffer = new byte[tempFile.Length];
            tempFile.Read(tempBuffer, 0, tempBuffer.Length);
            tempFile.Flush();
            tempFile.Close();
            return tempBuffer;
        }

        /// <summary>
        /// 字节流转换Wav音频
        /// </summary>
        public static string BytesTurnWav(string varPath, byte[] varByte)
        {
            FileStream tempFile = new FileStream(varPath, FileMode.Create);//新建文件
            tempFile.Write(varByte, 0, varByte.Length);
            tempFile.Flush();
            tempFile.Close();
            return varPath;
        }

        /// <summary>
        /// 转换音频
        /// </summary>
        public static byte[] Convert(AudioClip varAudioClip)
        {
            float[] tempSamples = new float[varAudioClip.samples];

            varAudioClip.GetData(tempSamples, 0);

            return Data.FloatTurnBytes(tempSamples);
        }

        /// <summary>
        /// 转换音频
        /// </summary>
        public static byte[] Convert(AudioClip varAudioClip, int varTime, int varSamplingRate)
        {
            float[] tempSamples1 = new float[varAudioClip.samples];

            varAudioClip.GetData(tempSamples1, 0);

            float[] tempSamples = new float[varTime * varSamplingRate];

            tempSamples1.CopyTo(tempSamples, varTime * varSamplingRate);

            return Data.FloatTurnBytes(tempSamples);
        }
    }
}