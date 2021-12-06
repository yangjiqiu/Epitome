using Epitome;
using Epitome.UIFrame;
using System;
using UnityEngine;


/// <summary>卡牌详情</summary>
public class CardDetailsPanel : PopUpPanelBase
{
    public override string GetUIType()
    {
        return UIPanelType.CardSynthesisPanel.ToString();
    }

    private Message message;

    protected override void OnAwake()
    {
        UIMaskType = UIMaskType.Translucence;

        message = new Message("CardDetailsPanelOpen",this);

        base.OnAwake();
    }

    public override void Display()
    {
        base.Display();

        message.Send();
    }
}