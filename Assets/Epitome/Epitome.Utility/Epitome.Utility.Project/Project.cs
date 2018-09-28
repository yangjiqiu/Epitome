/*----------------------------------------------------------------
 * 文件名：Project
 * 文件功能描述：项目
----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;//读写文件和数据流的类型
using System;

namespace Epitome.Utility
{
    //===============命名空间=========================
    using Hardware;
    using Manager;
    //===============命名空间=========================

    public class Project
    {
        static Project mInstance;

        public static Project GetSingleton() { if (mInstance == null) { mInstance = new Project(); } return mInstance; }


        //=================创建资源======================

        /// <summary>
        /// 创建文件.
        /// </summary>               
        public void CreateFile(string varPath)
        {
            if (!RetrieveFiles(varPath))
                File.CreateText(varPath);
        }

        /// <summary>
        /// 创建目录.
        /// </summary>               
        public void CreateDirectory(string varPath)
        {
            if (!Directory.Exists(varPath))
                Directory.CreateDirectory(varPath);
        }

        /// <summary>
        /// 创建目录.
        /// </summary>               
        public void CreateMultipleDirectories(string varPath, string[] varName)
        {
            foreach (string v in varName)
            {
                if (!Directory.Exists(varPath + "/" + v))
                    Directory.CreateDirectory(varPath + "/" + v);
            }
        }

        //=================检索资源======================

        /// <summary>
        /// 检索文件.
        /// </summary>
        public bool RetrieveFiles(string varPath)
        {
            return File.Exists(varPath);
        }

        //=================读取资源======================

        /// <summary>
        /// 读取文件.
        /// </summary>
        public ArrayList ReadFile(string varPath, string varName)
        {
            //使用流的形式读取
            StreamReader tempSR = null;
            try
            {
                tempSR = File.OpenText(varPath + "//" + varName);
            }
            catch (Exception e)
            {
                //路径与名称未找到文件则直接返回空
                return null;
            }
            string tempLine;
            ArrayList tempArrlist = new ArrayList();
            while ((tempLine = tempSR.ReadLine()) != null)
            {
                //一行一行的读取
                //将每一行的内容存入数组链表容器中
                tempArrlist.Add(tempLine);
            }
            //关闭流
            tempSR.Close();
            //销毁流
            tempSR.Dispose();
            //将数组链表容器返回
            return tempArrlist;
        }

        /// <summary>
        /// 根据路径返回文件的字节流
        /// </summary>
        public byte[] GetFileByte(string varPath)
        {
            if (Project.GetSingleton().RetrieveFiles(varPath))
            {
                //创建文件读取流
                FileStream tempFileStream = new FileStream(varPath, FileMode.Open, FileAccess.Read);

                tempFileStream.Seek(0, SeekOrigin.Begin);
                //创建文件长度缓冲区
                byte[] tempBytes = new byte[tempFileStream.Length];
                //读取文件
                tempFileStream.Read(tempBytes, 0, (int)tempFileStream.Length);
                //释放文件读取流
                tempFileStream.Close();
                tempFileStream.Dispose();
                tempFileStream = null;

                return tempBytes;
            }
            return null;
        }

        //=================  保存资源   ======================

        /// <summary>
        /// 保存文件.
        /// </summary>
        public IEnumerator SaveDocument(byte[] varByte,string varPath)
        {
            DeleteFiles(varPath);

            FileStream tempFileStream = new FileStream(varPath, FileMode.CreateNew);

            //将文件内容转换成二进制形式
            BinaryWriter tempBinaryWriter = new BinaryWriter(tempFileStream);

            //写文件
            tempBinaryWriter.Write(varByte);

            tempBinaryWriter.Close();
            tempFileStream.Close();

            yield return null;

#if UNITY_EDITOR
            AssetDatabase.Refresh(); //刷新
#endif
        }

        /// <summary>
        /// 保存图片.
        /// </summary>
        public void SavePicture(Texture2D varTexture2D, string varPath, string varName)
        {
            byte[] tempBytes = varTexture2D.EncodeToPNG();
            Project.GetSingleton().CreateDirectory(Application.dataPath + "/" + varPath);
            File.WriteAllBytes(GetPath.GetPersistent + "/" + varPath + "/" + varName, tempBytes);
        }

        //=================  加载资源   ======================

        /// <summary>
        /// 加载服务器资源
        /// </summary>
        public IEnumerator LoadServerResources(string varURL,string varPath)
        {
            WWW tempWWW = new WWW(varURL);

            while (!tempWWW.isDone)
            {
                EventManager.GetSingleton().BroadcastEvent("LoadProgress", (((int)(tempWWW.progress * 100)) % 100) + "%");
                yield return 0.2f;
            }

            yield return tempWWW;

            object[] fd = tempWWW.assetBundle.LoadAllAssets();

            foreach (object v in fd)
                Debug.Log(v.ToString());

            if (tempWWW.error == null)
            {
                Project.GetSingleton().SaveDocument(tempWWW.bytes, varPath);
                EventManager.GetSingleton().BroadcastEvent("LoadProgress", 100 + "%");
            }
            else
                EventManager.GetSingleton().BroadcastEvent("LoadProgress", "加载失败");

            EventManager.GetSingleton().UnRegisterEvent("LoadProgress");
        }


        //=================  删除资源   ======================

        /// <summary>
        /// 删除文件.
        /// </summary>
        public void DeleteFiles(string varPath) { FileInfo tempFile = new FileInfo(varPath); if (tempFile != null) DeleteFiles(tempFile); }

        /// <summary>
        /// 删除文件.
        /// </summary>
        public void DeleteFiles(FileInfo varFile) { if (varFile != null) varFile.Delete(); }

        /// <summary>
        /// 删除文件夹.
        /// </summary>
        public void DeleteTheFolder(string varPath)
        {
            DirectoryInfo tempDire = new DirectoryInfo(varPath);
            if (tempDire != null)
                DeleteTheFolder(tempDire);
        }

        /// <summary>
        /// 删除文件夹.
        /// </summary>
        public void DeleteTheFolder(DirectoryInfo varDire)
        {
            if (varDire == null || (!varDire.Exists))
                return;

            DirectoryInfo[] tempDires = varDire.GetDirectories();
            if (tempDires != null)
            {
                for (int i = 0; i < tempDires.Length; i++)
                    DeleteTheFolder(tempDires[i]);
                tempDires = null;
            }

            FileInfo[] tempFiles = varDire.GetFiles();
            if (tempFiles != null)
            {
                for (int i = 0; i < tempFiles.Length; i++)
                    DeleteFiles(tempFiles[i]);
                tempFiles = null;
            }

            varDire.Delete();
        }


        //=================  跳转场景   ======================

        /// <summary>
        /// 跳转场景.
        /// </summary>
        public void JumpScene(string varName) { UnityEngine.SceneManagement.SceneManager.LoadScene(varName); }

        /// <summary>
        /// 跳转场景.
        /// </summary>
        public void JumpScene(int varSubscript) { UnityEngine.SceneManagement.SceneManager.LoadScene(varSubscript); }
    }
}
