using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

namespace Epitome.Utility
{
    //===============命名空间=========================
    using Manager;
    //===============命名空间=========================

    public enum SelectUpdate
    {
        /// <summary>
        /// 更新全部资源
        /// </summary>
        AllResources,

        /// <summary>
        /// 更新已有资源
        /// </summary>
        ExistingResources,
    }

    /// <summary>
    /// 热更新
    /// </summary>
    public class HotUpdate : MonoBehaviour
    {
        static HotUpdate mInstance;
        public static HotUpdate GetSingleton()
        {
            if (mInstance == null)
            {
                //尝试寻找该类的实例。此处不能用GameObject.Find，因为MonoBehaviour继承自Component。
                mInstance = Object.FindObjectOfType(typeof(HotUpdate)) as HotUpdate;

                if (mInstance == null)
                {
                    GameObject tempGame = new GameObject("HotUpdate");
                    mInstance = tempGame.AddComponent<HotUpdate>();
                }
            }
            return mInstance;
        }


        //++++++++++++++++++++     分界线     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



        XmlNodeList mNewXnl;//新版本文件

        /// <summary>
        /// 下载版本文件
        /// </summary>
        public IEnumerator VersionFile(string varURl,SelectUpdate varSelectUpdate)
        {
            while (true)
            {
                WWW tempVersion = new WWW(varURl);
                yield return tempVersion;
                if (tempVersion.error != null)
                {
                    tempVersion.Dispose();
                    tempVersion = null;
                    yield return new WaitForSeconds(1);
                }
                else
                {
                    XmlDocument tempXmlDoc = new XmlDocument();
                    StringReader tempSR = new StringReader(tempVersion.text);
                    Debug.Log(tempVersion.text);
                    tempXmlDoc.LoadXml(tempSR.ReadToEnd());
                    mNewXnl = tempXmlDoc.GetElementsByTagName("file");
                    tempSR.Close();
                    tempSR.Dispose();
                    tempVersion.Dispose();
                    tempVersion = null;
                    Project.CreateDirectory(ProjectPath.GetPersistent + "/Resources");
                    GetUpdateFile(ProjectPath.GetPersistent + "/Resources", varSelectUpdate);
                    break;
                }
            }
        }

        /// <summary>
        /// 需要更新的文件
        /// </summary>
        List<XmlNode> mUpdateFile;

        /// <summary>
        /// 获取更新资源
        /// </summary>
        public void GetUpdateFile(string varPath, SelectUpdate varSelectUpdate)
        {
            DirectoryInfo tempDirInfo = new DirectoryInfo(varPath);//资源文件夹
            //获取文件夹下所有文件(包括子文件夹下)
            FileInfo[] tempFileInfoList = tempDirInfo.GetFiles("*", SearchOption.AllDirectories);
            Dictionary<string, FileInfo> templocalFileDic = new Dictionary<string, FileInfo>();
            foreach (FileInfo v in tempFileInfoList)
            {
                string tempFileName = v.FullName.Remove(0, varPath.Length + 1);
                Debug.Log(tempFileName);
                if (!templocalFileDic.ContainsKey(tempFileName))
                {
                    templocalFileDic.Add(tempFileName, v);
                }
            }
            mUpdateFile = new List<XmlNode>();
            string tempFile = "";//文件名
            string tempOldVersion = "";//本地版本号
            string tempNewVersion = "";//服务器版本号
            for (int i = 0; i < mNewXnl.Count; i++)
            {
                tempFile = mNewXnl[i].Attributes["name"].InnerText;
                if (!templocalFileDic.ContainsKey(tempFile))
                {
                    if (varSelectUpdate == SelectUpdate.ExistingResources) continue;
                    mUpdateFile.Add(mNewXnl[i]);//本地不存在
                    Debug.Log("本地不存在");
                } 
                else
                {
                    tempOldVersion = GetLocalVersion(templocalFileDic[tempFile].FullName);
                    tempNewVersion = mNewXnl[i].Attributes["version"].InnerText;
                    if (tempOldVersion != tempNewVersion)
                    {
                        mUpdateFile.Add(mNewXnl[i]);//本地存在但版本不同
                        Debug.Log("本地存在但版本不同");
                    }
                }
            }

            StartCoroutine(UpdateFile());
        }

        /// <summary>
        /// 获取本地文件版本
        /// </summary>
        public string GetLocalVersion(string varPath)
        {
            Debug.Log("本地文件版本：" + varPath);
            string tempRoot = Path.GetDirectoryName(varPath);
            Debug.Log("本地文件版本：" + tempRoot);
            XmlDocument tempXmlDoc = new XmlDocument();
            if (File.Exists(tempRoot + "/Version.xml"))
                tempXmlDoc.Load(tempRoot + "/Version.xml");

            foreach (XmlElement v in tempXmlDoc.SelectSingleNode("root").ChildNodes)
            {
                if (varPath.IndexOf(v.Attributes["name"].InnerText) != -1)
                {
                    Debug.Log("本地文件版本：" + v.GetAttribute("version"));
                    return v.GetAttribute("version");
                }
            }

            return "";
        }

        /// <summary>
        /// 更新文件
        /// </summary>
        public IEnumerator UpdateFile()
        {
            //更新版本文件
            XmlDocument tempXmlD = new XmlDocument();
            XmlElement tempXmlE = tempXmlD.CreateElement("root");
            tempXmlD.AppendChild(tempXmlE);
            foreach (XmlNode v in mNewXnl)
            {
                tempXmlD.DocumentElement.AppendChild(tempXmlD.ImportNode(v, true));
            }
            tempXmlD.Save(ProjectPath.GetPersistent + "/Resources/Version.xml");

            //更新资源
            for (int i = 0; i < mUpdateFile.Count; i++)
            {
                string tempPath = "http://39.108.137.135/" + mUpdateFile[i].Attributes["path"].InnerText + mUpdateFile[i].Attributes["name"].InnerText;//服务器文件地址

                int tempSize = int.Parse(mUpdateFile[i].Attributes["size"].InnerText);//文件大小
                //下载文件
                WWW tempWWW = new WWW(tempPath);

                //广播进度
                while (!tempWWW.isDone)
                {
                    float tempProgress = tempWWW.progress * (100f / (float)mUpdateFile.Count) + (i * (100f / (float)mUpdateFile.Count));
                    EventManager.Instance.BroadcastEvent(EventEnum.DownloadProgress, (int)tempProgress);
                    yield return 0.2f;
                }

                yield return tempWWW;

                if (tempWWW.bytesDownloaded == tempSize)
                {
                    StartCoroutine(Project.SaveFile(tempWWW.bytes, ProjectPath.GetPersistent + "/Resources/" + mUpdateFile[i].Attributes["name"].InnerText));
                }
            }

            EventManager.Instance.BroadcastEvent(EventEnum.DownloadProgress, "100");
        }
    }
}
