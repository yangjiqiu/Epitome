/* $Header:   Assets/Examples/Epitome.UIFrame/Scripts/FunctionModule/CaraSynthesis/CardUnit.cs   1.0   2021/10/30 Saturday PM 04:23:50   Ji Qiu .Yang   2017.1.1f1  $ */
/***********************************************************************************************
 ***              C O N F I D E N T I A L  ---  E P I T O M E  S T U D I O S                 ***
 ***********************************************************************************************
 *                                                                                             *
 *                 Project Name : Epitome                                                      *
 *                                                                                             *
 *                    File Name : CardUnit.cs                                                  *
 *                                                                                             *
 *                   Programmer : Ji Qiu .Yang                                                 *
 *                                                                                             *
 *                   Start Date : 2021/10/30                                                   *
 *                                                                                             *
 *                  Last Update : 2021/10/30                                                   *
 *                                                                                             *
 *---------------------------------------------------------------------------------------------*
 * Functions:                                                                                  *
 * - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Epitome.Utility;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Epitome;

/// <summary>卡牌单位</summary>
public class CardUnit : UIEventListener, ICardUnit
{
    public CardData cardData { get; private set; }

    // ========== 组件

    private Image background;
    private Image buddha;
    private Image border;
    private Image select;

    private ScrollRect scrollRect;

    // ========== 变量

    private Task monitoringTimeTask;
    private Task testOperationTask;
    private float time;

    protected void Awake()
    {
        // 监听卡牌单位上的点击事件
        OnMouseDown += MouseDown;
        OnMouseUp += MouseUp;
        OnMouseExit += MouseExit;

        monitoringTimeTask = new Task(MonitoringTime());
        monitoringTimeTask.Pause();

        testOperationTask = new Task(TestOperation());
        testOperationTask.Pause();

        scrollRect = transform.parent.parent.parent.GetComponent<ScrollRect>();
    }

    private void MouseDown(GameObject varGO, PointerEventData varData)
    {
        isClick = true;

        // 显示选中框
        SelectCard(true);

        // 开始按下时间检测
        time = 0;
        isMonitoringTime = true;
        monitoringTimeTask.UnPause();
    }

    private void MouseUp(GameObject varGO, PointerEventData varData)
    {
        // 结束时间检测
        isMonitoringTime = false;
        isClick = false;
    }

    private void MouseExit(GameObject varGO, PointerEventData varData)
    {
        if (Input.GetMouseButton(0) && CardDataManage.Instance.selectCardUnit == (ICardUnit)this)
        {
            // 拖出卡牌单位效果
            CardDataManage.Instance.cardUnit.SetCardData(cardData);
            CardDataManage.Instance.cardUnit.SetCardActive(true);

            CardDataManage.Instance.followCardUnit = this;

            // 限制滑动
            scrollRect.vertical = false;

            // 虚化卡牌单位
            CardUnitEmptiness(0.5f);

            testOperationTask.UnPause();
        }
    }

    /// <summary>卡牌单位虚化</summary>
    private void CardUnitEmptiness(float value = 1)
    {
        background.color = new Color(background.color.r, background.color.b, background.color.g, value);
        buddha.color = new Color(buddha.color.r, buddha.color.b, buddha.color.g, value);
        border.color = new Color(border.color.r, border.color.b, border.color.g, value);
        select.color = new Color(border.color.r, border.color.b, border.color.g, value);
    }

    /// <summary>监测操作</summary>
    /// <returns></returns>
    private IEnumerator TestOperation()
    {
        while (true)
        {
            yield return null;
           
            if (Input.GetMouseButtonUp(0))
            {
                // 关闭效果
                CardDataManage.Instance.cardUnit.SetCardActive(false);
                CardDataManage.Instance.followCardUnit = null;

                // 恢复限制
                scrollRect.vertical = true;

                // 恢复虚化卡牌单位
                CardUnitEmptiness();

                testOperationTask.Pause();
            }
        }
    }

    private static bool isClick;

    private bool isMonitoringTime;

    /// <summary>监听时间</summary>
    /// <returns></returns>
    private IEnumerator MonitoringTime()
    {
        while (true)
        {
            yield return null;

            // 检测按下时间
            if (isMonitoringTime)
            {
                time += Time.deltaTime;
                if (time >= 1.5f)
                {
                    time = 0;
                    isMonitoringTime = false;

                    // 打开卡牌详情面板
                    UIControl.OpenUI(new UIPanelType[] { UIPanelType.CardDetailsPanel });
                }
            }

            if (!isClick && Input.GetMouseButtonDown(0))
            {
                // 取消选中状态
                CardDataManage.Instance.CancelSelectCard();
                monitoringTimeTask.Pause();
            }
        }
    }

    public void SetCardActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SelectCard(bool value)
    {
        if (value)
        {
            // 清除旧选中状态
            CardDataManage.Instance.CancelSelectCard();
            select.gameObject.SetActive(value);
            CardDataManage.Instance.selectCardUnit = this;
        }
        else
        {
            select.gameObject.SetActive(value);
            CardDataManage.Instance.selectCardUnit = null;
        }

        // 刷新合成卡槽提示
        CardDataManage.Instance.SynthesisCardSlotsPutHint(value);

    }

    public void SetCardData(CardData data)
    {
        cardData = data;

        if (cardData == null) return;

        // 获取组件设置图片资源
        background = transform.Find("background").GetComponent<Image>();
        buddha = transform.Find("buddha").GetComponent<Image>();
        border = transform.Find("border").GetComponent<Image>();
        select = transform.Find("select").GetComponent<Image>();

        // 根据卡牌品种设置卡牌单位背景图
        Sprite sprite = CardDataManage.Instance.backgroundSprites[data.cardVariety == CardVariety.OrdinaryCard ? 0 : 1];
        background.sprite = sprite;

        // 根据信息设置头像、边框
        buddha.sprite = CardDataManage.Instance.buddhaSprites[data.cardGrade == "A" ? 0 : data.cardGrade == "B" ? 1 : data.cardGrade == "C" ? 2 : 3];
        border.sprite = CardDataManage.Instance.borderSprites[data.cardGrade == "A" ? 0 : data.cardGrade == "B" ? 1 : data.cardGrade == "C" ? 2 : 3];

        // 显示
        SetCardActive(true);
    }
}