using Epitome.UIFrame;
using System;
using UnityEngine;
using Epitome;
using System.Collections;

/// <summary>卡牌飞入动画面板</summary>
public class CardFlyIntoPanel : PopUpPanelBase
{
    public Transform card;

    public override string GetUIType()
    {
        return UIPanelType.CardFlyIntoPanel.ToString();
    }

    Task task;

    private float time;
    private float speedTime = 1;

    private Vector3 currentPos;
    private Vector3 targetPos;
    protected override void OnAwake()
    {
        UIMaskType = UIMaskType.Translucence;

        task = new Task(DisplayAnimation());
        task.Pause();

        base.OnAwake();
    }

    public override void Display()
    {
        base.Display();

        // 显示卡牌放入动画特效
        time = 0;

        if(currentPos==Vector3.zero) currentPos = card.position;

        switch (CardDataManage.Instance.currentCardVariety)
        {
            case CardVariety.OrdinaryCard:
                targetPos = CardDataManage.Instance.cardSlots[CardDataManage.Instance.haveOrdinaryCard.Count].Pos;
                break;
            case CardVariety.UncommonCard:
                targetPos = CardDataManage.Instance.ordinaryTog.transform.position;
                break;

        }
        task.UnPause();
    }

    private IEnumerator DisplayAnimation()
    {
        while (true)
        {
            yield return null;

            time += Time.deltaTime;
            card.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time / speedTime);
            card.position = Vector3.Lerp(currentPos, targetPos, time / speedTime);
            card.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0,0,45f), time / speedTime));

            if (time / speedTime >= 1)
            {
                // 关闭面板
                UIControl.CloseUI(UIPanelType.CardFlyIntoPanel);

                // 添加卡包数据
                switch (CardDataManage.Instance.newCardData.cardVariety)
                {
                    case CardVariety.OrdinaryCard:
                        CardDataManage.Instance.haveOrdinaryCard.Add(CardDataManage.Instance.newCardData);
                        break;
                    case CardVariety.UncommonCard:
                        CardDataManage.Instance.haveUncommonCard.Add(CardDataManage.Instance.newCardData);
                        break;
                }

                CardDataManage.Instance.newCardData = null;

                CardDataManage.Instance.cardRefreshMessage.Send();

                task.Pause();
            }
        }
    }
}