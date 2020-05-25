/*----------------------------------------------------------------
 * 文件名：Picture
 * 文件功能描述：图片
----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Epitome.Utility
{
    //===============命名空间=========================
    using Hardware;
    using Utility;
    //===============命名空间=========================

    /// <summary>
    /// 图片
    /// </summary>
    public class Picture
    {
        private static Picture _instance;

        public static Picture Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Picture();
                return _instance;
            }
        }

        //++++++++++++++++++++     分界线     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 获取当前拍照照片名字
        /// </summary>
        public string GetPhotosName() { return "IMG_" + Data.GetCurrentTime(new string[] { "", "", "_", "", "" }); }

        /// <summary>
        /// 根据图片路径返回图片的字节流
        /// </summary>
        public byte[] GetImageByte(string varPath) { return ReadFile.ReadFileStream(varPath); }

        /// <summary>
        /// 字节流转换图片
        /// </summary>
        public Texture2D BytesTurnImage(int[] varPixels, byte[] varByte)
        {
            Texture2D tempTexture = new Texture2D(varPixels[0], varPixels[1]);
            tempTexture.LoadImage(varByte);
            return tempTexture;
        }
    }
}
