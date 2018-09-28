/*----------------------------------------------------------------
 * 文件名：Audio
 * 文件功能描述：音频
----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace Epitome.Utility
{
    //===============命名空间=========================
    using Hardware;
    //===============命名空间=========================

    /// <summary>
    /// 音频
    /// </summary>
    public class Audio
    {
        static Audio mInstance;

        public static Audio GetSingleton() { if (mInstance == null) { mInstance = new Audio(); } return mInstance; }

        //++++++++++++++++++++     录音     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 开始录音
        /// </summary>
        public void StartRecording(AudioSource varAudioSource, int varDuration = 60, bool varLoop = false, int varFrequency = 4)
        {
            Mike.GetSingleton().OpenMicrophone(varAudioSource, varDuration, varLoop, varFrequency);
        }

        /// <summary>
        /// 停止录音
        /// </summary>
        public int StopRecording(int varAudioTime = 60, int varSamplingRate = 44100)
        {
            int tempAudioLength;
            int tempLastPos = Microphone.GetPosition(null);
            if (Microphone.IsRecording(null))
                tempAudioLength = tempLastPos / varSamplingRate + 1;
            else
                tempAudioLength = varAudioTime;

            
            return tempAudioLength;
        }

        //++++++++++++++++++++     分界线     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 播放音频
        /// </summary>
        public void PlayAudio(AudioClip varAudio) { AudioSource.PlayClipAtPoint(varAudio, Vector3.zero); }

        /// <summary>
        /// 播放音频
        /// </summary>
        public void PlayAudio(AudioClip varAudio,Vector3 varV3) { AudioSource.PlayClipAtPoint(varAudio, varV3); }

        /// <summary>
        /// 播放音频
        /// </summary>
        public void PlayAudio(AudioSource varAudioSource) { varAudioSource.Play(); }

        /// <summary>
        /// 播放音频
        /// </summary>
        public void PlayAudio(AudioSource varAudioSource, float varTime) { varAudioSource.PlayScheduled(varTime); }

        /// <summary>
        /// 暂停音频
        /// </summary>
        public void PauseAudio(AudioSource varAudioSource) { varAudioSource.Pause(); }

        /// <summary>
        /// 停止音频
        /// </summary>
        public void StopAudio(AudioSource varAudioSource) { varAudioSource.Stop(); }

        /// <summary>
        /// 剪切空白部分
        /// </summary>
        public AudioClip CutBlankSection(AudioClip varAudioClip, int varTime, int varSamplingRate)
        {
            float[] tempSamples1 = new float[varAudioClip.samples];

            varAudioClip.GetData(tempSamples1, 0);

            Debug.Log(tempSamples1.Length);

            float[] tempSamples = new float[varTime * varSamplingRate];

            Array.Copy(tempSamples1, 0, tempSamples, 0, varTime * varSamplingRate);

            Debug.Log(tempSamples.Length);

            AudioClip tempAudioClip = AudioClip.Create(varAudioClip.name, varSamplingRate * varTime, 1, varSamplingRate, false);
            tempAudioClip.SetData(tempSamples, 0);

            return tempAudioClip;
        }

        /// <summary>
        /// 合并音频
        /// </summary>
        public AudioClip MergeAudio(int varInterval, params AudioClip[] varClips)
        {
            if (varClips == null || varClips.Length == 0)
                return null;

            int tempChannels = varClips[0].channels;
            int tempFrequency = varClips[0].frequency;
            //for (int i = 1; i < varClips.Length; i++) { if (varClips[i].channels != tempChannels || varClips[i].frequency != tempFrequency) { return null; } }

            using (MemoryStream tempMemoryStream = new MemoryStream())
            {
                for (int i = 0; i < varClips.Length; i++)
                {
                    if (varClips[i] == null)
                        continue;

                    varClips[i].LoadAudioData();

                    var tempBuffer = varClips[i].GetData();

                    tempMemoryStream.Write(tempBuffer, 0, tempBuffer.Length);

                    if (varClips.Length - 1 == i)
                        continue;

                    byte[] tempByte = new byte[varClips[i].frequency * varClips[i].channels * 4 * varInterval];//合并音频间的间隔
                    tempMemoryStream.Write(tempByte, 0, tempByte.Length);
                }

                var tempBytes = tempMemoryStream.ToArray();
                //var tempResult = AudioClip.Create("Merge", tempBytes.Length / 4 / tempChannels, tempChannels, tempFrequency, false);
                var tempResult = AudioClip.Create("Merge", tempBytes.Length / 4 / tempChannels, tempChannels, 44100, false);

                tempResult.SetData(tempBytes);

                return tempResult;
            }
        }

        /// <summary>
        /// 保存音频
        /// </summary>
        public IEnumerator SaveAudio(AudioClip tempClip, string tempPath)
        {
            string tempFilePath = Path.GetDirectoryName(tempPath);
            if (!Directory.Exists(tempFilePath))
            {
                Directory.CreateDirectory(tempFilePath);
            }
            using (FileStream tempFileStream = CreateEmpty(tempPath))
            {
                ConvertAndWrite(tempFileStream, tempClip);
                WriteHeader(tempFileStream, tempClip);
            }
            yield return null;
        }

        /// <summary>
        /// 音频转换WAV字节流
        /// </summary>
        public byte[] AudioTurnBytes(AudioClip clip, int varTime, int varSampLing)
        {
            byte[] bytes = null;

            using (var tempMemoryStream = new MemoryStream())
            {
                tempMemoryStream.Write(new byte[44], 0, 44);//预留44字节头部信息  

                byte[] tempByte = ConvertAndWrite(clip, 60, 8000);

                tempMemoryStream.Write(tempByte, 0, tempByte.Length);

                tempMemoryStream.Seek(0, SeekOrigin.Begin);

                byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
                tempMemoryStream.Write(riff, 0, 4);

                byte[] chunkSize = BitConverter.GetBytes(tempMemoryStream.Length - 8);
                tempMemoryStream.Write(chunkSize, 0, 4);

                byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
                tempMemoryStream.Write(wave, 0, 4);

                byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
                tempMemoryStream.Write(fmt, 0, 4);

                byte[] subChunk1 = BitConverter.GetBytes(16);
                tempMemoryStream.Write(subChunk1, 0, 4);

                UInt16 two = 2;
                UInt16 one = 1;

                byte[] audioFormat = BitConverter.GetBytes(one);
                tempMemoryStream.Write(audioFormat, 0, 2);

                byte[] numChannels = BitConverter.GetBytes(clip.channels);
                tempMemoryStream.Write(numChannels, 0, 2);

                byte[] sampleRate = BitConverter.GetBytes(clip.frequency);
                tempMemoryStream.Write(sampleRate, 0, 4);

                byte[] byteRate = BitConverter.GetBytes(clip.frequency * clip.channels * 2); // sampleRate * bytesPerSample*number of channels  
                tempMemoryStream.Write(byteRate, 0, 4);

                UInt16 blockAlign = (ushort)(clip.channels * 2);
                tempMemoryStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

                UInt16 bps = 16;
                byte[] bitsPerSample = BitConverter.GetBytes(bps);
                tempMemoryStream.Write(bitsPerSample, 0, 2);

                byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
                tempMemoryStream.Write(datastring, 0, 4);

                byte[] subChunk2 = BitConverter.GetBytes(clip.samples * clip.channels * 2);
                tempMemoryStream.Write(subChunk2, 0, 4);

                bytes = tempMemoryStream.ToArray();
            }

            return bytes;
        }



        /// <summary>
        /// 转换音频
        /// </summary>
        public byte[] ConvertAndWrite(AudioClip varAudioClip, int varTime, int varSamplingRate)
        {
            float[] tempSamples1 = new float[varAudioClip.samples];

            varAudioClip.GetData(tempSamples1, 0);

            Debug.Log(tempSamples1.Length);

            float[] tempSamples = new float[varTime * varSamplingRate];

            tempSamples1.CopyTo(tempSamples, varTime * varSamplingRate);

            Debug.Log(tempSamples.Length);

            return Data.GetSingleton().FloatTurnBytes(tempSamples);
        }

        //=================   音频转码  ======================

        /// <summary>
        /// Wav音频转换字节流
        /// </summary>
        public byte[] WavTurnBytes(string varStr)
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
        public string BytesTurnWav(string varPath, byte[] varByte)
        {
            FileStream tempFile = new FileStream(varPath, FileMode.Create);//新建文件
            tempFile.Write(varByte, 0, varByte.Length);
            tempFile.Flush();
            tempFile.Close();
            return varPath;
        }

        /// <summary>
        /// 转换写入音频
        /// </summary>
        void ConvertAndWrite(FileStream varFileStream, AudioClip tempClip)
        {
            //byte[] tempByte = AudioTurnBytes(tempClip);
            //varFileStream.Write(tempByte, 0, tempByte.Length);
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        FileStream CreateEmpty(string varFilepath)
        {
            FileStream tempFileStream = new FileStream(varFilepath, FileMode.Create);
            byte tempByte = new byte();

            for (int i = 0; i < 44; i++)
            {
                tempFileStream.WriteByte(tempByte);
            }

            return tempFileStream;
        }

        /// <summary>
        /// 添加报头
        /// </summary>
        void WriteHeader(FileStream tempFileStream, AudioClip tempClip)
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

    public static class AudioClipExtension
    {
        public static byte[] GetData(this AudioClip varClip)
        {
            float[] tempData = new float[varClip.samples * varClip.channels];

            varClip.GetData(tempData, 0);

            byte[] bytes = new byte[tempData.Length * 4];
            Buffer.BlockCopy(tempData, 0, bytes, 0, bytes.Length);

            return bytes;
        }

        public static void SetData(this AudioClip varClip, byte[] varBytes)
        {
            float[] data = new float[varBytes.Length / 4];
            Buffer.BlockCopy(varBytes, 0, data, 0, varBytes.Length);

            varClip.SetData(data, 0);
        }
    }
}
