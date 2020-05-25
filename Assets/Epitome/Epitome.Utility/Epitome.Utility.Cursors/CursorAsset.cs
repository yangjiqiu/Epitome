#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Epitome
{
    public class CursorAsset : ScriptableObject
    {
        [SerializeField]
        public Texture2D[] Original;
        [SerializeField]
        public Texture2D[] Custom;
#if UNITY_EDITOR
        [MenuItem("Epitome/Create/CursorAsset")]
        static void CreateAsset()
        {
            var exampleAsset = CreateInstance<CursorAsset>();
            AssetDatabase.CreateAsset(exampleAsset, string.Format("{0}/{1}", ProjectPath.FileDirectory(ProjectPath.RelativePath(typeof(CursorAsset))), "CursorAsset.asset"));
            AssetDatabase.Refresh();
        }

        [MenuItem("Epitome/Load CursorAsset")]
        static void LoadExampleAsset()
        {
            //var exampleAsset =
            //AssetDatabase.LoadAssetAtPath<CursorAsset>(string.Format("{0}/{1}", ProjectPath.RelativePath(typeof(CursorAsset)), "CursorAsset.asset"));
        }
#endif
    }
}