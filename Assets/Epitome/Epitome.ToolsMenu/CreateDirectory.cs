using Epitome.Utility;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Epitome.ToolsMenu
{
    /// <summary>创建目录</summary>
    public class CreateDirectory
    {
        private static string[] subfile = new string[] { "Anims", "Atlas", "Audios", "Effects", "Materials", "Prefabs", "Scenes", "Scripts" };

#if UNITY_EDITOR
        /// <summary>创建Resources资源目录，包括子资源目录</summary>
        [MenuItem("Epitome/Create/Directory/Resources")]
        [MenuItem("Assets/Epitome/Create/Directory/Resources")]
        private static void CreateResourcesDirectory()
        {
            List<string> paths = EditorExtension.GetSelectionPath();
            for (int i = 0; i < paths.Count; i++)
            {
                CreateResourcesDirectory(paths[i]);
            }
        }

        private static void CreateResourcesDirectory(string dataPath)
        {
            string path = dataPath + "/Resources";
            Project.CreateDirectory(path);
            Project.CreateMultipleDirectories(path, subfile);

            AssetDatabase.Refresh();
        }
#endif
    }
}