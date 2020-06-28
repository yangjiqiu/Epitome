/*----------------------------------------------------------------
 * 文件名：Packaged
 * 文件功能描述：打包
----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Security.Cryptography;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Epitome.Utility
{
    //===============命名空间=========================
    //using Manager;
    //===============命名空间=========================

    /// <summary>
    /// 打包
    /// </summary>
    public static class Packaged
    {

        //++++++++++++++++++++     打包资源     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#if UNITY_EDITOR

        /// <summary>
        /// 打包AssetBundle
        /// </summary>
        [MenuItem("Epitome/Packaged/AssetBundle")]
        public static void PackAssetBundle()
        {
            // 打开保存面板，获得用户选择的路径  
            string tempPath = EditorUtility.OpenFolderPanel("Open Folder", Application.dataPath ,"");

            //Object[] tempSelection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            //if (tempPath.Length != 0)
            //{
                BuildTarget buildTarget;
#if UNITY_ANDROID
                buildTarget = BuildTarget.Android;
#elif UNITY_IPHONE
                buildTarget = BuildTarget.Android;
#elif UNITY_STANDALONE_WIN
                buildTarget = BuildTarget.StandaloneWindows;
#endif

                ////文件已经存在就删除  
                //if (Directory.Exists(tempPath))
                //{
                //    Directory.Delete(tempPath, true);
                //}
                ////文件不存在就创建  
                //if (!Directory.Exists(tempPath))
                //{
                //    Directory.CreateDirectory(tempPath);
                //}

                //打包
                //BuildPipeline.BuildAssetBundle(Selection.activeObject, tempSelection, tempPath, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, buildTarget);
                //BuildPipeline.BuildAssetBundles("文件名", BuildAssetBundleOptions.None, buildTarget);
                AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.None, buildTarget);

                if (manifest == null)
                    Debug.LogError("AssetBundle 打包失败！");
                else
                    Debug.Log("AssetBundle 打包完毕");
           // }
        }

#endif
        /// <summary>
        /// 获取版本文件
        /// </summary>
        public static string GetVersionFile(string varPath, out XmlDocument verXml)
        {
            string tempRoot = Path.GetDirectoryName(varPath);
            Project.CreateDirectory(tempRoot);
            verXml = new XmlDocument();
            if (File.Exists(tempRoot + "/Version.xml"))
                verXml.Load(tempRoot + "/Version.xml");
            else
            {
                XmlElement tempXmlE = verXml.CreateElement("root");
                verXml.AppendChild(tempXmlE);
                verXml.Save(tempRoot + "/Version.xml");
            }

            return tempRoot;
        }

        /// <summary>
        /// 获取版本号(MD5)
        /// </summary>
        public static string GetVersionNumberMD5(string varPata)
        {
            FileStream tempFileStream = File.Open(varPata, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] tempByteFile = new byte[(int)tempFileStream.Length];
            tempFileStream.Read(tempByteFile, 0, tempByteFile.Length);
            tempFileStream.Close();
            tempFileStream.Dispose();
            MD5 tempMD5 = MD5.Create();
            tempMD5.ComputeHash(tempByteFile);
            string tempVersionNumber = System.BitConverter.ToString(tempMD5.Hash).Replace("-", "");
            tempMD5.Clear();
            return tempVersionNumber;
        }

        /// <summary>
        /// 修改版本
        /// </summary>
        public static void ModifyEdition(string varPath, string varRoot, XmlDocument verXml, string varVersion)
        {
            XmlNodeList tempFileXnl = verXml.GetElementsByTagName("file");
            XmlNode tempXn = null;
            foreach (XmlNode v in tempFileXnl)
            {
                if (varPath.IndexOf(v.Attributes["name"].InnerText) != -1)
                {
                    tempXn = v;
                    FileInfo fileInfo = new FileInfo(varPath);
                    tempXn.Attributes["size"].InnerText = fileInfo.Length.ToString();
                    tempXn.Attributes["version"].InnerText = varVersion;
                    break;
                }
            }
            if (tempXn == null)
            {
                FileInfo fileInfo = new FileInfo(varPath);
                XmlElement item = verXml.CreateElement("file");
                item.SetAttribute("name", varPath.Remove(0, varRoot.Length + 1));
                item.SetAttribute("size", fileInfo.Length.ToString());
                item.SetAttribute("version", varVersion);
                item.SetAttribute("path", "Upload/PineappleAR/");
                verXml.GetElementsByTagName("root")[0].AppendChild(item);
            }
            verXml.Save(varRoot + "/Version.xml");
            verXml.Clone();
            Debug.Log(varRoot + "/Version.xml");
            new Task(UploadFile(varRoot + "/Version.xml")); 
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public static IEnumerator UploadFile(string varPath)
        {
            WWWForm tempForm = new WWWForm();
            string[] tempSp = varPath.Split('/');
            tempForm.AddField("Directory", "PineappleAR/");
            tempForm.AddBinaryData("FileUpload", ReadFile.ReadFileStream(varPath), tempSp[tempSp.Length - 1]);
            WWW tempWWW = new WWW("http://39.108.137.135/Upload/ReceiveFiles.php", tempForm);
            yield return tempWWW;
            Debug.Log(tempWWW.text);
        }
    }
}