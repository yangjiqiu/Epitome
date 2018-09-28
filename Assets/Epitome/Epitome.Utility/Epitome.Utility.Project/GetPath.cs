/*----------------------------------------------------------------
 * 文件名：GetPath
 * 文件功能描述：获取路径
----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Epitome.Utility
{
    /// <summary>
    /// 获取路径
    /// </summary>
    public class GetPath
    {
        //=================  获取平台应用存储路径   ======================

        /// <summary>
        /// 返回 游戏数据文件夹的路径(只读 移动端没有访问权限)
        /// </summary>
        public static string GetDataPath { get { return Application.dataPath; } }

        /// <summary>
        /// 返回 持久化数据存储目录(运行时才能写入或者读取)
        /// </summary>
        public static string GetPersistent { get { return Application.persistentDataPath; } }

        /// <summary>
        /// 返回 流数据的缓存目录(只读不可写 主要用来存放二进制文件 只能用WWW类来读取)
        /// </summary>
        public static string GetStreamingAssets { get { return Application.streamingAssetsPath; } }

        /// <summary>
        /// 返回 临时数据缓存目录(只读)
        /// </summary>
        public static string GetTemporaryCache { get { return Application.temporaryCachePath; } }

        //=================  获取资源路径   ======================

        /// <summary>
        /// 获取目录下所有文件路径
        /// </summary>
        public static string[] GetFilePaths(string varPath) { return Directory.GetFiles(varPath); }

        /// <summary>
        /// 获取目录下后缀名文件路径
        /// </summary>
        public static string[] GetFilePaths(string varPath, string varSuffixName) { return Directory.GetFiles(varPath, "*." + varSuffixName); }

        /// <summary>
        /// 获取目录下所有图片路径
        /// </summary>
        public static List<string> GetPicturePath(string varPath)
        {
            List<string> tempFilePaths = new List<string>();
            string tempType = "BMP|JPG|GIF|PNG";
            string[] tempImageType = tempType.Split('|');
            for (int i = 0; i < tempImageType.Length; i++)
            {
                string[] tempDirs = GetFilePaths(varPath, tempImageType[i]);
                for (int j = 0; j < tempDirs.Length; j++)
                    tempFilePaths.Add(tempDirs[j]);
            }
            return tempFilePaths;
        }
    }
}
