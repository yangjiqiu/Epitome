using System;
using UnityEngine;
using Epitome.UIFrame;

namespace Epitome
{
    public class UIControl
    {
        // 预加载UI
        public static void PreloadUI(Enum enumType, string prefabPayh, GameObject fatherNode)
        {
            UIManager.Instance.PreloadUI(enumType.ToString(), prefabPayh, fatherNode);
        }
        // 打开UI
        public static void OpenUI(Enum enumType)
        {
            UIManager.Instance.OpenUI(enumType.ToString());
        }
        public static void OpenUI(params Enum[] enumTypes)
        {
            UIManager.Instance.OpenUI(enumTypes.ToStringArray());
        }
        public static void OpenUI(string nameType)
        {
            UIManager.Instance.OpenUI(nameType);
        }
        public static void OpenUI(params string[] nameTypes)
        {
            UIManager.Instance.OpenUI(nameTypes.ToStringArray());
        }
        // 打开UI传入参数
        public static void OpenUI(Enum enumType, params object[] data)
        {
            UIManager.Instance.OpenUI(enumType.ToString(), data);
        }
        public static void OpenUI(string nameType, params object[] data)
        {
            UIManager.Instance.OpenUI(nameType, data);
        }
        // 打开UI并关闭其他UI
        public void OpenUICloseOthers(Enum enumType)
        {
            UIManager.Instance.OpenUICloseOthers(enumType.ToString());
        }
        // 打开UI并关闭其他UI传入参数
        public void OpenUICloseOthers(Enum enumType, params object[] data)
        {
            UIManager.Instance.OpenUICloseOthers(enumType.ToString(), data);
        }
        // 给UI传入参数
        public static void SetUIParams(Enum enumType, params object[] data)
        {
            UIManager.Instance.GetUI<UIBase>(enumType.ToString()).SetUIParams(data);
        }
        // 关闭UI
        public static void CloseUI(Enum enumType)
        {
            UIManager.Instance.CloseUI(enumType.ToString());
        }
        // 关闭UI
        public static void CloseUI(string nameType)
        {
            UIManager.Instance.CloseUI(nameType);
        }

        // 操作相同UI类型 tag 加载不同类型时给定的标识符

        public static void OpenSameKindUI(Enum enumType, string tag)
        {
            UIManager.Instance.OpenSameKindUI(enumType.ToString(), tag, null);
        }

        public static void OpenSameKindUI(Enum enumType, string tag, params object[] data)
        {
            UIManager.Instance.OpenSameKindUI(enumType.ToString(), tag, data);
        }

        public static void CloseSameKindUI(Enum enumType, string tag)
        {
            UIManager.Instance.CloseSameKindUI(enumType.ToString(), tag);
        }

        /// <summary> 关闭所有UI（通过UI框架打开的） </summary>
        public static void CloseAllUI()
        {
            UIManager.Instance.CloseUIAll();
        }
    }
}