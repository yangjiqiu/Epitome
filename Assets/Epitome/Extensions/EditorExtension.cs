using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Epitome
{
	public static class EditorExtension
	{
#if UNITY_EDITOR
        /// <summary>
        /// 获取Project面板选择路径
        /// </summary>
        public static List<string> GetSelectionPath()
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
#endif
    }
}
