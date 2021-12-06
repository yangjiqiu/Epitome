using Epitome.Utility;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Epitome.ToolsMenu
{
    /// <summary>创建文件</summary>
    public class CreateFile
    {
#if UNITY_EDITOR
        /// <summary>生成XML文件</summary>
        [MenuItem("Epitome/Create/File/XML")]
        [MenuItem("Assets/Epitome/Create/File/XML")]
        private static void CreateXMLFile() { CreateBlankFile("xml"); }

        /// <summary>生成XLS文件</summary>
        [MenuItem("Epitome/Create/File/XLS")]
        [MenuItem("Assets/Epitome/Create/File/XLS")]
        private static void CreateXLSFile() { CreateBlankFile("xls"); }

        /// <summary>生成XLSX文件</summary>
        [MenuItem("Epitome/Create/File/XLSX")]
        [MenuItem("Assets/Epitome/Create/File/XLSX")]
        private static void CreateXLSXFile() { CreateBlankFile("xlsx"); }

        /// <summary>生成JSON文件</summary>
        [MenuItem("Epitome/Create/File/JSON")]
        [MenuItem("Assets/Epitome/Create/File/JSON")]
        private static void CreateJSONFile() { CreateBlankFile("json"); }

        /// <summary>生成PHP文件</summary>
        [MenuItem("Epitome/Create/File/PHP")]
        [MenuItem("Assets/Epitome/Create/File/PHP")]
        private static void CreatePHPFile() { CreateBlankFile("php"); }

        private static void CreateBlankFile(string suffix)
        {
            List<string> paths = EditorExtension.GetSelectionPath();
            for (int i = 0; i < paths.Count; i++)
            {
                Project.CreateFile(paths[i] + "/New File." + suffix);
            }

            AssetDatabase.Refresh();
        }
#endif
    }
}