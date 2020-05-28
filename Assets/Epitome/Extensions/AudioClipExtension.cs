using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Epitome
{
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
