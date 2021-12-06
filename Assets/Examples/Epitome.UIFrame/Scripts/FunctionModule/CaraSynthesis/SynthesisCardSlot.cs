/* $Header:   Assets/Examples/Epitome.UIFrame/Scripts/FunctionModule/CaraSynthesis/SynthesisCardSlot.cs   1.0   2021/10/31 Sunday PM 08:42:32   Ji Qiu .Yang   2017.1.1f1  $ */
/***********************************************************************************************
 ***              C O N F I D E N T I A L  ---  E P I T O M E  S T U D I O S                 ***
 ***********************************************************************************************
 *                                                                                             *
 *                 Project Name : Epitome                                                      *
 *                                                                                             *
 *                    File Name : SynthesisCardSlot.cs                                         *
 *                                                                                             *
 *                   Programmer : Ji Qiu .Yang                                                 *
 *                                                                                             *
 *                   Start Date : 2021/10/31                                                   *
 *                                                                                             *
 *                  Last Update : 2021/10/31                                                   *
 *                                                                                             *
 *---------------------------------------------------------------------------------------------*
 * Functions:                                                                                  *
 * - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Epitome;
using Epitome.Utility;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>合成卡槽接口</summary>
public interface ISynthesisCardSlot : ICardSlot
{
    /// <summary>可放置提示</summary>
    void PutHint(bool value);
}

/// <summary>合成卡槽</summary>
public class SynthesisCardSlot : UIEventListener, ISynthesisCardSlot
{
    public Vector3 Pos { get { return transform.position; } }
    private Task task;

    private ICardUnit cardUnit;

    private GameObject select;
    private GameObject countermand;

    private Button countermandBut;

    protected void Awake()
    {
        // 监听卡牌单位上的点击事件
        OnMouseDown += MouseDown;
        OnMouseExit += MouseExit;
        OnMouseEnter += MouseEnter;

        task = new Task(MonitorOperation());
        task.Pause();

        cardUnit = GetComponentInChildren<ICardUnit>(true);

        select = transform.Find("select").gameObject;
        countermand = transform.Find("countermand").gameObject;

        countermandBut = countermand.GetComponent<Button>();

        // 取消按钮
        countermandBut.onClick.AddListener(()=>{
            CountermandSynthesisCardSlot();
        });
    }

    public void PutCard(CardData data) { }

    public void ClearCard()
    {        
        // 影藏按钮
        countermand.SetActive(false);
        cardUnit.SetCardActive(false);
        cardUnit.SetCardData(null);
    }

    public void PutHint(bool value)
    {
        if (value && cardUnit.cardData == null) select.SetActive(value);
        else if (!value) select.SetActive(value);
    }

    private IEnumerator MonitorOperation()
    {
        while (true)
        {
            yield return null;

            if (Input.GetMouseButtonUp(0))
            {
                if (cardUnit.cardData == null)
                {
                    // 拖动置入
                    PutSynthesisCardSlot();
                }

                task.Pause();
            }
        }
    }

    /// <summary>置入合成卡槽</summary>
    private void PutSynthesisCardSlot()
    {
        // 放下卡牌单位 
        cardUnit.SetCardData(CardDataManage.Instance.selectCardUnit.cardData);

        // 清除卡包数据
        switch (CardDataManage.Instance.selectCardUnit.cardData.cardVariety)
        {
            case CardVariety.OrdinaryCard:
                CardDataManage.Instance.haveOrdinaryCard.Remove(CardDataManage.Instance.selectCardUnit.cardData);
                break;
            case CardVariety.UncommonCard:
                CardDataManage.Instance.haveUncommonCard.Remove(CardDataManage.Instance.selectCardUnit.cardData);
                break;
        }

        // 添加合成卡槽数据
        CardDataManage.Instance.synthesisCard.Add(CardDataManage.Instance.selectCardUnit.cardData);

        // 取消卡包中选中状态
        CardDataManage.Instance.CancelSelectCard();

        // 清除数据
        CardDataManage.Instance.followCardUnit = null;

        // 刷新卡包
        CardDataManage.Instance.cardRefreshMessage.Send();

        // 显示按钮
        countermand.SetActive(true);
    }

    /// <summary>取消合成卡槽</summary>
    private void CountermandSynthesisCardSlot()
    {
        // 添加卡包数据
        switch (cardUnit.cardData.cardVariety)
        {
            case CardVariety.OrdinaryCard:
                CardDataManage.Instance.haveOrdinaryCard.Add(cardUnit.cardData);
                break;
            case CardVariety.UncommonCard:
                CardDataManage.Instance.haveUncommonCard.Add(cardUnit.cardData);
                break;
        }

        // 清除合成卡槽数据
        CardDataManage.Instance.synthesisCard.Remove(cardUnit.cardData);

        // 清除卡牌
        ClearCard();

        // 刷新卡包
        CardDataManage.Instance.cardRefreshMessage.Send();
    }

    private void MouseDown(GameObject varGO, PointerEventData varData)
    {
        if (CardDataManage.Instance.selectCardUnit != null && cardUnit.cardData == null)
        {
            // 点击置入
            PutSynthesisCardSlot();
        }
    }

    private void MouseEnter(GameObject varGO, PointerEventData varData)
    {
        if (CardDataManage.Instance.followCardUnit != null) task.UnPause();
    }

    private void MouseExit(GameObject varGO, PointerEventData varData)
    {
        if (!task.Paused) task.Pause();
    }
}
