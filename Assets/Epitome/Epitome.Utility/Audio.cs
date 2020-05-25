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
    using Hardware;

    public static class Audio
    {
        public static void BeginRecording(AudioSource audioSource, int duration = 60, bool loop = false, int frequency = 4)
        {
            Mike.OpenMicrophone(audioSource, duration, loop, frequency);
        }

        public static int EndRecording(int audioTime = 60, int samplingRate = 44100)
        {
            int audioLength;
            int lastPos = Microphone.GetPosition(null);
            if (Microphone.IsRecording(null))
                audioLength = lastPos / samplingRate + 1;
            else
                audioLength = audioTime;

            return audioLength;
        }

        public static void PlayAudio(AudioClip audio) { AudioSource.PlayClipAtPoint(audio, Vector3.zero); }
        public static void PlayAudio(AudioClip audio, Vector3 pos) { AudioSource.PlayClipAtPoint(audio, pos); }
        public static void PlayAudio(AudioSource audioSource) { audioSource.Play(); }
        public static void PlayAudio(AudioSource audioSource, float time) { audioSource.PlayScheduled(time); }

        public static void PauseAudio(AudioSource audioSource) { audioSource.Pause(); }

        public static void StopAudio(AudioSource audioSource) { audioSource.Stop(); }

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

        public static IEnumerator SaveAudio(AudioClip clip, string path)
        {
            Project.CreateDirectory(Path.GetDirectoryName(path));

            using (FileStream fileStream = CreateEmpty(path))
            {
                ConvertAndWrite(fileStream, clip);
                WriteHeader(fileStream, clip);
            }
            yield return null;
        }

        /// <summary>
        /// 音频转换WAV字节流
        /// </summary>
        public static byte[] AudioTurnBytes(AudioClip clip, int varTime, int varSampLing)
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

                //UInt16 two = 2;
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
        public static byte[] ConvertAndWrite(AudioClip varAudioClip, int varTime, int varSamplingRate)
        {
            float[] tempSamples1 = new float[varAudioClip.samples];

            varAudioClip.GetData(tempSamples1, 0);

            Debug.Log(tempSamples1.Length);

            float[] tempSamples = new float[varTime * varSamplingRate];

            tempSamples1.CopyTo(tempSamples, varTime * varSamplingRate);

            Debug.Log(tempSamples.Length);

            return Data.FloatTurnBytes(tempSamples);
        }

        //=================   音频转码  ======================

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
        /// 转换写入音频
        /// </summary>
        static void ConvertAndWrite(FileStream varFileStream, AudioClip tempClip)
        {
            //byte[] tempByte = AudioTurnBytes(tempClip);
            //varFileStream.Write(tempByte, 0, tempByte.Length);
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
