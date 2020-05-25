using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

namespace Epitome
{
    public enum CursorType
    {
        /// <summary>标准图标和小沙漏（后台运行）</summary>
        Appstarting,
        /// <summary>标准的箭头</summary>
        Arrow, 
        /// <summary>十字光标</summary>
        Cross, 
        /// <summary>手的形状</summary>
        Hand, 
        /// <summary>标准的箭头和问号（帮助）</summary>
        Help, 
        /// <summary>工字光标（输入）</summary>
        Ibeam, 
        /// <summary>禁止圈</summary>
        No, 
        /// <summary>四向箭头：四个方位的箭头分别指向北、南、东、西</summary>
        SizeAll,
        /// <summary>双箭头：分别指向东北和西南</summary>
        SizeNESW,
        /// <summary>双箭头：分别指向北和南（上下双向箭头）</summary>
        SizeNS, 
        /// <summary>双箭头：分别指向西北和东南</summary>
        SizeNWSE,
        /// <summary>双箭头：分别指向西和东（左右双向箭头）</summary>
        SizeWE, 
        /// <summary>垂直箭头</summary>
        UpArrow,
        /// <summary>沙漏（等待）</summary>
        Wait,
    }

    public static class Cursors
    {
        public static void SetCursor(CursorType type)
        {
            CursorsControl.Instance.SetCursor(type);
        }

        public static void SetCursor(CursorType type, Vector2 hotspot, CursorMode cursorMode = CursorMode.Auto)
        {
            CursorsControl.Instance.SetCursor(type);
        }

        public static void SetCursor(string name)
        {
            CursorsControl.Instance.SetCursor(name);
        }

        public static void SetCursor(string name, Vector2 hotspot, CursorMode cursorMode = CursorMode.Auto)
        {
            CursorsControl.Instance.SetCursor(name, hotspot, cursorMode);
        }
    }

    public class CursorsControl : MonoSingleton<CursorsControl>
    {
        private CursorAsset cursorAsset;

        public Dictionary<string, Texture2D> Original;
        public Dictionary<string, Texture2D> Custom;

        private string oldCursor;
        private Texture2D currentCursor;

        private CursorsControl()
        {
            Original = new Dictionary<string, Texture2D>();
            Custom = new Dictionary<string, Texture2D>();
        }

        private void Awake()
        {
            DontDestroyOnLoad(); // 保证实例不会被释放
            LoadCursorAsset();
        }

        private void LoadCursorAsset()
        {
            cursorAsset = Resources.Load<CursorAsset>("CursorAsset");

            if (cursorAsset == null)
            {
                throw new Exception(string.Format("Under the path \"{0}\". Resources \"CursorAsset.asset\" is empty!", ProjectPath.FileDirectory(ProjectPath.RelativePath(typeof(Cursors)))));
            }
            else
            {
                if (cursorAsset.Original != null && cursorAsset.Original.Length >= 1)
                {
                    Texture2D[] original = cursorAsset.Original;

                    for (int i = 0; i < original.Length; i++)
                    {
                        Original.Add(original[i].name, original[i]);
                    }
                }
                if (cursorAsset.Custom != null && cursorAsset.Custom.Length >= 1)
                {
                    Texture2D[] custom = cursorAsset.Custom;

                    for (int i = 0; i < custom.Length; i++)
                    {
                        Custom.Add(custom[i].name, custom[i]);
                    }
                }
            }
        }

        private Vector2 GetHotspot(CursorType cursorType)
        {
            Vector2 hotspot = Vector2.zero;

            switch (cursorType)
            {
                case CursorType.Arrow:
                    hotspot = Vector2.zero;
                    break;
                case CursorType.SizeNS:
                    hotspot = new Vector2(currentCursor.width / 2, currentCursor.height / 2);
                    break;
            }

            return hotspot;
        }

        public void SetCursor(CursorType type)
        {
            SetCursor(type, GetHotspot(type), CursorMode.Auto);
        }

        public void SetCursor(CursorType type, Vector2 hotspot, CursorMode cursorMode)
        {
            if (!Original.ContainsKey(type.ToString())) return;
            if (oldCursor == type.ToString()) return; else oldCursor = type.ToString();
            currentCursor = Original.GetValue(type.ToString());
            Cursor.SetCursor(currentCursor, hotspot, cursorMode);
        }

        public void SetCursor(string name)
        {
            SetCursor(name, Vector2.zero, CursorMode.Auto);
        }

        public void SetCursor(string name, Vector2 hotspot, CursorMode cursorMode)
        {
            if (!Custom.ContainsKey(name)) return;
            if (oldCursor == name) return; else oldCursor = name;
            currentCursor = Custom.GetValue(name);
            Cursor.SetCursor(currentCursor, hotspot, cursorMode);
        }

        //private bool changeFlag = false;

        //private void OnGUI()
        //{
        //    if (GUI.Button(Rect(10, 10, 100, 50), "hand"))
        //    {
        //        changeFlag = true;
        //        Screen.showCursor = false;
        //    }
        //    if (GUI.Button(Rect(120, 10, 100, 50), "arrow"))
        //    {
        //        changeFlag = false;
        //        Screen.showCursor = true;
        //    }
        //    if (changeFlag)
        //    {
        //        var mousePos = Input.mousePosition;
        //        GUI.DrawTexture(Rect(mousePos.x, Screen.height - mousePos.y, cursorTexture.width, cursorTexture.height), cursorTexture);
        //    }
        //}
    }
}
