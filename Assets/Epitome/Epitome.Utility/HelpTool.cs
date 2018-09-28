/*----------------------------------------------------------------
 * 文件名：HelpTool
 * 文件功能描述：帮助工具
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
    /// 帮助工具
    /// </summary>
    public class HelpTool : MonoBehaviour
    {

#if UNITY_EDITOR
        /// <summary>
        /// 生成Resources资源目录
        /// </summary>
        [MenuItem("HelpTool/Create/Directory/Resources")]
        private static void CreateResourcesDirectory()
        {
            string tempPath = Application.dataPath + "/";
            Project.GetSingleton().CreateDirectory(tempPath + "Resources");

            tempPath = tempPath + "Resources";
            string[] tempStr = new string[] { "Anim", "Atlas", "Audio", "Effect", "Material", "Prefab", "Scene", "Script"  };
            Project.GetSingleton().CreateMultipleDirectories(tempPath,tempStr);
        }

        /// <summary>
        /// 生成XML文件
        /// </summary>
        [MenuItem("HelpTool/Create/File/XML")]
        private static void CreateXMLFile() { CreatePHPFile("xml"); }

        /// <summary>
        /// 生成JSON文件
        /// </summary>
        [MenuItem("HelpTool/Create/File/JSON")]
        private static void CreateJSONFile() { CreatePHPFile("json"); }

        /// <summary>
        /// 生成PHP文件
        /// </summary>
        [MenuItem("HelpTool/Create/File/PHP")]
        private static void CreatePHPFile() { CreatePHPFile("php"); }

        /// <summary>
        /// 生成空白文件
        /// </summary>
        private static void CreatePHPFile(string varSuffix)
        {
            List<string> tempListStr = GetSelectionPath();
            foreach (string v in tempListStr)
                Project.GetSingleton().CreateFile(v + "/New File." + varSuffix);
        }

        /// <summary>
        /// 获取Project面板选择路径
        /// </summary>
        private static List<string> GetSelectionPath()
        {
            Object[] tempObj = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

            List<string> tempListStr=new List<string> ();

            foreach (Object v in tempObj)
            {
                string tempStr = Application.dataPath;
                foreach (string vv in AssetDatabase.GetAssetPath(v).Split('/'))
                {
                    if (vv != "Assets")
                        tempStr = tempStr + "/" + vv;
                }
                tempListStr.Add(tempStr);
                Debug.Log(tempStr);
            }

            return tempListStr;
        }

        /// <summary>
        /// 打包AssetBundle资源
        /// </summary>
        [MenuItem("HelpTool/Pack/AssetBundle")]
        private static void PackAssetBundle() {Packaged.GetSingleton().PackAssetBundle();  }
#endif
    }
}
