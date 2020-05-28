using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Epitome
{
	public static class EditorExtension
	{
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
    }
}
