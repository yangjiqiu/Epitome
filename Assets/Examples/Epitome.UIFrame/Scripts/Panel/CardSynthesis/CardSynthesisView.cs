using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System;
using Epitome;

/// <summary>卡牌合成视图</summary>
public class CardSynthesisView : BaseView
{
    /// <summary>普通卡牌</summary>
    public Toggle ordinaryTog;
    
    /// <summary>稀有卡牌</summary>
    public Toggle uncommonTog;

    /// <summary>刷新按钮</summary>
    public Button refreshBut;

    /// <summary>合成按钮</summary>
    public Button synthesisBut;

    /// <summary>卡槽样本</summary>
    public GameObject cardSlotPrototype;

    // 滑动区域 && 滑动物体
    private RectTransform GridParentTran;
    private RectTransform GridTran;

    /// <summary>合成卡槽</summary>
    private List<ISynthesisCardSlot> synthesisCardSlots;

    protected override void Awake()
    {
        ordinaryTog.onValueChanged.AddListener((bool value)=> { CardSwitch(CardVariety.OrdinaryCard, value); });
        uncommonTog.onValueChanged.AddListener((bool value) => { CardSwitch(CardVariety.UncommonCard, value); });
        refreshBut.onClick.AddListener(CardRefresh);
        synthesisBut.onClick.AddListener(CardSynthesis);

        GridTran = cardSlotPrototype.transform.parent.GetComponent<RectTransform>();
        GridParentTran = GridTran.transform.parent.GetComponent<RectTransform>();

        // 创建六行五列卡槽

        CardDataManage.Instance.cardSlots = new List<ICardSlot>();

        for (int j = 0; j < 6; j++)
        {
            for (int i = 0; i < 5; i++)
            {
                Transform tran = Instantiate(cardSlotPrototype).transform;
                tran.name = string.Format("CardSlot{0}_{1}", j, i);
                tran.parent = cardSlotPrototype.transform.parent;
                tran.gameObject.SetActive(tran);
                CardDataManage.Instance.cardSlots.Add(tran.GetComponent<CardSlot>());
            }
        }

        // 获取合成卡槽
        synthesisCardSlots = transform.Find("Underpart/Right").GetComponentsInChildren<ISynthesisCardSlot>().ToList();
        CardDataManage.Instance.synthesisCardSlots = synthesisCardSlots;

        MessageCenter.Instance.AddListener("CardRefresh", CardRefresh);

        CardDataManage.Instance.ordinaryTog = ordinaryTog.gameObject;
    }

    private void Start()
    {
        // 初始刷新
        CardSwitch(CardVariety.OrdinaryCard, true);
    }

    protected override void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener("CardRefresh", CardRefresh);
    }

    /// <summary>切换卡牌</summary>
    private void CardSwitch(CardVariety variety, bool value)
    {
        switch (variety)
        {
            case CardVariety.OrdinaryCard:
                CardDataManage.Instance.currentCardVariety = variety;
                CardRefresh();
                break;
            case CardVariety.UncommonCard:
                CardDataManage.Instance.currentCardVariety = variety;
                CardRefresh();
                break;
        }
    }

    /// <summary>卡牌刷新</summary>
    private void CardRefresh(object obj = null)
    {
        CardRefresh();
    }

    /// <summary>卡牌刷新</summary>
    private void CardRefresh()
    {
        // 回到顶端
        Vector3 localPos = GridTran.localPosition;
        localPos.y = -GridTran.rect.height / 2 + GridParentTran.rect.height / 2;
        GridTran.localPosition = localPos;

        // 清除选中状态
        CardDataManage.Instance.CancelSelectCard();

        // 清除卡槽数据
        foreach (var item in CardDataManage.Instance.cardSlots) item.ClearCard();

        // 获取已有卡牌数据
        List<CardData> cardList = new List<CardData>();
        switch (CardDataManage.Instance.currentCardVariety)
        {
            case CardVariety.OrdinaryCard:
                cardList = CardDataManage.Instance.haveOrdinaryCard;
                break;
            case CardVariety.UncommonCard:
                cardList = CardDataManage.Instance.haveUncommonCard;
                break;
        }

        // 按照品质生成卡牌
        for (int i = 0; i < cardList.Count ; i++) CardDataManage.Instance.cardSlots[i].PutCard(cardList[i]);
    }

    /// <summary>卡牌合成</summary>
    private void CardSynthesis()
    {
        // 合成卡槽数量为3
        if (CardDataManage.Instance.synthesisCard.Count == 3)
        {
            // 打开新卡牌合成效果面板
            UIControl.OpenUI(new UIPanelType[] { UIPanelType.CardShowPanel });

            // 设置新卡牌S数据 普通S卡
            CardDataManage.Instance.newCardData = CardDataManage.Instance.OrdinaryCardData["S"][0];

            // 清除卡槽
            foreach (var item in synthesisCardSlots) item.ClearCard();

            // 合成卡槽数据置空
            CardDataManage.Instance.synthesisCard = new List<CardData>();
        }
    }
}