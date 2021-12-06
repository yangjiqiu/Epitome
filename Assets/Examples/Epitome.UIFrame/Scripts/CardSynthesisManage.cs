/* $Header:   Assets/Examples/Epitome.UIFrame/Scripts/CardSynthesisManage.cs   1.0   2021/10/29 Friday PM 07:23:33   Ji Qiu .Yang   2017.1.1f1  $ */
/***********************************************************************************************
 ***              C O N F I D E N T I A L  ---  E P I T O M E  S T U D I O S                 ***
 ***********************************************************************************************
 *                                                                                             *
 *                 Project Name : Epitome                                                      *
 *                                                                                             *
 *                    File Name : CardSynthesisManage.cs                                       *
 *                                                                                             *
 *                   Programmer : Ji Qiu .Yang                                                 *
 *                                                                                             *
 *                   Start Date : 2021/10/29                                                   *
 *                                                                                             *
 *                  Last Update : 2021/10/29                                                   *
 *                                                                                             *
 *---------------------------------------------------------------------------------------------*
 * Functions:                                                                                  *
 * - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Epitome;
using Epitome.UIFrame;
using System;

/// <summary>卡牌合成管理</summary>
public class CardSynthesisManage : MonoSingleton<CardSynthesisManage>
{
    // 外部拖动动态创建的根目录
    public GameObject UIFrame_RootNode;

    // 层级节点
    private GameObject PopUpNode;
    private GameObject FixedNode;
    private GameObject NormalNode;

    // Resources资源文件夹下UI面板路径
    public string UIPath = "Prefabs/UIPanel";

    protected void Awake()
    {
        // 获取UI节点
        PopUpNode = UIFrame_RootNode.transform.Find(UINodeType.PopUpNode.ToString()).gameObject;
        FixedNode = UIFrame_RootNode.transform.Find(UINodeType.FixedNode.ToString()).gameObject;
        NormalNode = UIFrame_RootNode.transform.Find(UINodeType.NormalNode.ToString()).gameObject;

        UIMaskManager.Instance.OnSingletonInit();

        AddInterface();
    }

    /// <summary>初始化界面</summary>
    private void AddInterface()
    {
        // 预加载所有的UI面板信息
        foreach (UIPanelType item in Enum.GetValues(typeof(UIPanelType)))
        {
            switch (item)
            {
                case UIPanelType.CardSynthesisPanel:
                    UIControl.PreloadUI(item, string.Format("{0}/{1}", UIPath, item.ToString()), NormalNode);
                    break;
                case UIPanelType.CardShowPanel:
                case UIPanelType.CardDetailsPanel:
                case UIPanelType.CardFlyIntoPanel:
                    UIControl.PreloadUI(item, string.Format("{0}/{1}", UIPath, item.ToString()), PopUpNode);
                    break;
            }
        }

        // 打开UI面板   使用了栈，先进后出
        UIControl.OpenUI(UIPanelType.CardSynthesisPanel);
    }
}
