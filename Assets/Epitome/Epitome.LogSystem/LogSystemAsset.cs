#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Epitome
{
    public class LogSystemAsset : ScriptableObject
    {
        [SerializeField]
        public Texture2D[] WindowIcons;

        [SerializeField]
        public Texture2D[] LogLevelIcons;

        [SerializeField]
        public Texture2D LogCountIcon;

#if UNITY_EDITOR
        [MenuItem("Epitome/Create/LogSystemAsset")]
        static void CreateAsset()
        {
            var exampleAsset = CreateInstance<LogSystemAsset>();
            AssetDatabase.CreateAsset(exampleAsset, string.Format("{0}/{1}", ProjectPath.FileDirectory(ProjectPath.RelativePath(typeof(LogSystemAsset))), "LogSystemAsset.asset"));
            AssetDatabase.Refresh();
        }
#endif
    }
}