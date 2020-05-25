using UnityEngine;
using Epitome.Utility;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Epitome.HelpTool
{
    public class Create
    {
#if UNITY_EDITOR
        /// <summary>
        /// 创建Resources资源目录，包括子资源目录
        /// </summary>
        [MenuItem("Epitome/Create/Directory/Resources")]
        private static void CreateResourcesDirectory()
        {
            string path = Application.dataPath + "/Resources";
            Project.CreateDirectory(path);
            string[] subfile = new string[] { "Anims", "Atlas", "Audios", "Effects", "Materials", "Prefabs", "Scenes", "Scripts" };
            Project.CreateMultipleDirectories(path, subfile);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成XML文件
        /// </summary>
        [MenuItem("Epitome/Create/File/XML")]
        private static void CreateXMLFile() { CreateBlankFile("xml"); }

        /// <summary>
        /// 生成JSON文件
        /// </summary>
        [MenuItem("Epitome/Create/File/JSON")]
        private static void CreateJSONFile() { CreateBlankFile("json"); }

        /// <summary>
        /// 生成PHP文件
        /// </summary>
        [MenuItem("Epitome/Create/File/PHP")]
        private static void CreatePHPFile() { CreateBlankFile("php"); }

        private static void CreateBlankFile(string suffix)
        {
            List<string> paths = GetSelectionPath();
            for (int i = 0; i < paths.Count; i++)
            {
                Project.CreateFile(paths[i] + "/New File." + suffix);
            }
        }

        /// <summary>
        /// 获取Project面板选择路径
        /// </summary>
        private static List<string> GetSelectionPath()
        {
            Object[] obj = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

            List<string> selectPath = new List<string>();

            string path = Application.dataPath;

            for (int i = 0; i < obj.Length; i++)
            {
                selectPath.Add(AssetDatabase.GetAssetPath(obj[i]));
            }

            return selectPath;
        }

        /// <summary>
        /// 打包AssetBundle资源
        /// </summary>
        [MenuItem("HelpTool/Pack/AssetBundle")]
        private static void PackAssetBundle() { Packaged.Instance.PackAssetBundle(); }
#endif
    }
}

