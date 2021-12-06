using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System;
using Epitome;

/// <summary>卡牌显示视图</summary>
public class CardShowView : BaseView
{
    /// <summary>放入卡包</summary>
    public Button placeBut;

    protected override void Awake()
    {
        placeBut.onClick.AddListener(PlaceCardPackage);
    }

    protected override void OnDestroy()
    {

    }

    /// <summary>放入卡包</summary>
    private void PlaceCardPackage()
    {
        // 关闭面板
        UIControl.CloseUI(UIPanelType.CardShowPanel);

        // 打开飞入特效面板
        UIControl.OpenUI(UIPanelType.CardFlyIntoPanel);
    }
}