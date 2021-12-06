using UnityEngine;
using Epitome.UIFrame;
using Epitome;

/// <summary>UI控制</summary>
public class UIControl
{
    // 预加载UI
    public static void PreloadUI(UIPanelType type, string prefabPayh, GameObject fatherNode)
    {
        UIManager.Instance.PreloadUI(type.ToString(), prefabPayh, fatherNode);
    }
    // 打开UI
    public static void OpenUI(UIPanelType type)
    {
        UIManager.Instance.OpenUI(type.ToString());
    }
    public static void OpenUI(UIPanelType[] type)
    {
        UIManager.Instance.OpenUI(type.ToStringArray());
    }
    public static void OpenUI(string name)
    {
        UIManager.Instance.OpenUI(name);
    }
    // 打开UI传入参数
    public static void OpenUI(UIPanelType type, params object[] data)
    {
        UIManager.Instance.OpenUI(type.ToString(), data);
    }
    public static void OpenUI(string name, params object[] data)
    {
        UIManager.Instance.OpenUI(name, data);
    }
    // 打开UI并关闭其他UI
    public void OpenUICloseOthers(UIPanelType type)
    {
        UIManager.Instance.OpenUICloseOthers(type.ToString());
    }
    // 打开UI并关闭其他UI传入参数
    public void OpenUICloseOthers(UIPanelType type, params object[] data)
    {
        UIManager.Instance.OpenUICloseOthers(type.ToString(), data);
    }
    // 给UI传入参数
    public static void SetUIParams(UIPanelType type, params object[] data)
    {
        UIManager.Instance.GetUI<UIBase>(type.ToString()).SetUIParams(data);
    }
    // 关闭UI
    public static void CloseUI(UIPanelType type)
    {
        UIManager.Instance.CloseUI(type.ToString());
    }
    // 关闭UI
    public static void CloseUI(string name)
    {
        UIManager.Instance.CloseUI(name);
    }

    // 操作相同UI类型 tag 加载不同类型时给定的标识符

    public static void OpenSameKindUI(UIPanelType type, string tag)
    {
        UIManager.Instance.OpenSameKindUI(type.ToString(), tag, null);
    }

    public static void OpenSameKindUI(UIPanelType type, string tag, params object[] data)
    {
        UIManager.Instance.OpenSameKindUI(type.ToString(), tag, data);
    }

    public static void CloseSameKindUI(UIPanelType type, string tag)
    {
        UIManager.Instance.CloseSameKindUI(type.ToString(), tag);
    }
    /// <summary> 关闭所有UI（通过UI框架打开的） </summary>
    public static void CloseAllUI()
    {
        UIManager.Instance.CloseUIAll();
    }
}