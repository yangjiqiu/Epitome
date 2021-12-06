/* $Header:   Assets/Examples/Epitome.UIFrame/Scripts/CardDataManage.cs   1.0   2021/10/30 Saturday PM 05:36:05   Ji Qiu .Yang   2017.1.1f1  $ */
/***********************************************************************************************
 ***              C O N F I D E N T I A L  ---  E P I T O M E  S T U D I O S                 ***
 ***********************************************************************************************
 *                                                                                             *
 *                 Project Name : Epitome                                                      *
 *                                                                                             *
 *                    File Name : CardDataManage.cs                                            *
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
using Epitome;

/// <summary>卡牌数据管理</summary>
public class CardDataManage : MonoSingleton<CardDataManage>
{
    /// <summary>普通卡牌数据库</summary>
    public Dictionary<string, List<CardData>> OrdinaryCardData;

    /// <summary>稀有卡牌包数据库</summary>
    public Dictionary<string, List<CardData>> UncommonCardData;

    /// <summary>普通卡牌</summary>
    public List<CardData> haveOrdinaryCard;

    /// <summary>稀有卡牌</summary>
    public List<CardData> haveUncommonCard;

    /// <summary>合成卡牌</summary>
    public List<CardData> synthesisCard;

    /// <summary>卡槽</summary>
    public List<ICardSlot> cardSlots;

    /// <summary>卡牌单位背景</summary>
    public Sprite[] backgroundSprites = new Sprite[] { };

    /// <summary>卡牌单位头像</summary>
    public Sprite[] buddhaSprites = new Sprite[] { };

    /// <summary>卡牌单位边框</summary>
    public Sprite[] borderSprites = new Sprite[] { };

    /// <summary>选中卡牌单位</summary>
    public ICardUnit selectCardUnit;

    /// <summary>跟随卡牌单位</summary>
    public ICardUnit followCardUnit;

    /// <summary>当前卡牌品种</summary>
    public CardVariety currentCardVariety;

    /// <summary>合成卡槽</summary>
    public List<ISynthesisCardSlot> synthesisCardSlots;

    /// <summary>卡牌单位拖出效果</summary>
    public ICardUnit cardUnit;

    /// <summary>新合成卡牌</summary>
    public CardData newCardData;

    /// <summary>卡包刷新信息</summary>
    public Message cardRefreshMessage;

    /// <summary>普通卡牌</summary>
    public GameObject ordinaryTog;

    protected void Awake()
    {
        InitCardAttributeData();

        // 类是地址传递，考虑到卡牌属性固定不会修改，暂且如此

        haveOrdinaryCard = new List<CardData>();
        haveOrdinaryCard.Add(OrdinaryCardData["A"][0]);
        haveOrdinaryCard.Add(OrdinaryCardData["B"][0]);
        haveOrdinaryCard.Add(OrdinaryCardData["B"][0]);
        haveOrdinaryCard.Add(OrdinaryCardData["C"][0]);
        haveOrdinaryCard.Add(OrdinaryCardData["C"][0]);

        haveUncommonCard = new List<CardData>();
        haveUncommonCard.Add(UncommonCardData["A"][0]);
        haveUncommonCard.Add(UncommonCardData["B"][0]);
        haveUncommonCard.Add(UncommonCardData["C"][0]);

        synthesisCard = new List<CardData>();

        cardRefreshMessage = new Message("CardRefresh", this);
    }

    /// <summary>合成卡槽放置提示</summary>
    public void SynthesisCardSlotsPutHint(bool value)
    {
        foreach (var item in synthesisCardSlots) item.PutHint(value);
    }

    /// <summary>取消选中状态</summary>
    public void CancelSelectCard()
    {
        if (selectCardUnit != null)
        {
            selectCardUnit.SelectCard(false);
            selectCardUnit = null;
        }
    }

    /// <summary>初始化卡牌属性数据</summary>
    private void InitCardAttributeData()
    {
        OrdinaryCardData = new Dictionary<string, List<CardData>>();
        OrdinaryCardData.Add("S", new List<CardData>());
        OrdinaryCardData.Add("A", new List<CardData>());
        OrdinaryCardData.Add("B", new List<CardData>());
        OrdinaryCardData.Add("C", new List<CardData>());
        OrdinaryCardData["S"].Add(new CardData("西施", CardVariety.OrdinaryCard, "S", "中国古代四大美女之一，沉鱼之美誉", new CardAttribute(4, 3, 5, 4),
            new List<CardSkill>() {
                new CardSkill("西施技能1",3, "西施技能1:中国古代四大美女之一，沉鱼之美誉"),
                new CardSkill("西施技能2", 3, "西施技能2:中国古代四大美女之一，沉鱼之美誉"),
                new CardSkill("西施技能3",3, "西施技能3:中国古代四大美女之一，沉鱼之美誉") }));
        OrdinaryCardData["A"].Add(new CardData("王昭君", CardVariety.OrdinaryCard, "A", "中国古代四大美女之一，落雁之美誉", new CardAttribute(3, 3, 4, 3),
            new List<CardSkill>()  {
                new CardSkill("王昭君技能1",3, "王昭君技能1:中国古代四大美女之一，落雁之美誉"),
                new CardSkill("王昭君技能2", 2, "王昭君技能2:中国古代四大美女之一，落雁之美誉"),
                new CardSkill("王昭君技能3",2, "王昭君技能3:中国古代四大美女之一，落雁之美誉") }));
        OrdinaryCardData["B"].Add(new CardData("貂蝉", CardVariety.OrdinaryCard, "B", "中国古代四大美女之一，闭月之美誉", new CardAttribute(2, 2, 4, 2),
            new List<CardSkill>()  {
                new CardSkill("貂蝉技能1",3, "貂蝉技能1:中国古代四大美女之一，闭月之美誉"),
                new CardSkill("貂蝉技能2", 2, "貂蝉技能2:中国古代四大美女之一，闭月之美誉"),
                new CardSkill("貂蝉技能3",1, "貂蝉技能3:中国古代四大美女之一，闭月之美誉") }));
        OrdinaryCardData["C"].Add(new CardData("杨贵妃", CardVariety.OrdinaryCard, "C", "中国古代四大美女之一，羞花之美誉", new CardAttribute(1, 2, 3, 3),
            new List<CardSkill>()  {
                new CardSkill("杨贵妃技能1",2, "杨贵妃技能1:中国古代四大美女之一，羞花之美誉"),
                new CardSkill("杨贵妃技能2", 2, "杨贵妃技能2:中国古代四大美女之一，羞花之美誉"),
                new CardSkill("杨贵妃技能3",1, "杨贵妃技能3:中国古代四大美女之一，羞花之美誉") }));


        UncommonCardData = new Dictionary<string, List<CardData>>();
        UncommonCardData.Add("S", new List<CardData>());
        UncommonCardData.Add("A", new List<CardData>());
        UncommonCardData.Add("B", new List<CardData>());
        UncommonCardData.Add("C", new List<CardData>());
        UncommonCardData["S"].Add(new CardData("唐僧", CardVariety.UncommonCard, "S", "又名唐三藏、唐御弟、金蝉子、唐玄奘", new CardAttribute(6, 2, 6, 6),
            new List<CardSkill>(){
                new CardSkill("唐僧技能1",3, "唐僧技能1:又名唐三藏、唐御弟、金蝉子、唐玄奘"),
                new CardSkill("唐僧技能2", 3, "唐僧技能2:又名唐三藏、唐御弟、金蝉子、唐玄奘"),
                new CardSkill("唐僧技能3",3, "唐僧技能3:又名唐三藏、唐御弟、金蝉子、唐玄奘") }));
        UncommonCardData["A"].Add(new CardData("孙悟空", CardVariety.UncommonCard, "A", "又名孙行者、心猿、金公、斗战胜佛", new CardAttribute(6, 2, 4, 6),
            new List<CardSkill>(){
                new CardSkill("孙悟空技能1",3, "孙悟空技能1:又名孙行者、心猿、金公、斗战胜佛"),
                new CardSkill("孙悟空技能2", 2, "孙悟空技能2:又名孙行者、心猿、金公、斗战胜佛"),
                new CardSkill("孙悟空技能3",2, "孙悟空技能3:又名孙行者、心猿、金公、斗战胜佛") }));
        UncommonCardData["B"].Add(new CardData("猪八戒", CardVariety.UncommonCard, "B", "又名猪刚鬣、天蓬元帅、净坛使者、二师兄、老猪", new CardAttribute(3, 5, 2, 1),
            new List<CardSkill>(){
                new CardSkill("猪八戒技能1",3, "猪八戒技能1:又名猪刚鬣、天蓬元帅、净坛使者、二师兄、老猪"),
                new CardSkill("猪八戒技能2", 2, "猪八戒技能2:又名猪刚鬣、天蓬元帅、净坛使者、二师兄、老猪"),
                new CardSkill("猪八戒技能3",1, "猪八戒技能3:又名猪刚鬣、天蓬元帅、净坛使者、二师兄、老猪") }));
        UncommonCardData["C"].Add(new CardData("沙僧", CardVariety.UncommonCard, "C", "又名沙僧、卷帘大将、金身罗汉、沙师弟、老沙、悟净", new CardAttribute(3, 3, 2, 1),
            new List<CardSkill>(){
                new CardSkill("沙僧技能1",2, "沙僧技能1:又名沙僧、卷帘大将、金身罗汉、沙师弟、老沙、悟净"),
                new CardSkill("沙僧技能2", 2, "沙僧技能2:又名沙僧、卷帘大将、金身罗汉、沙师弟、老沙、悟净"),
                new CardSkill("沙僧技能3",1, "沙僧技能3:又名沙僧、卷帘大将、金身罗汉、沙师弟、老沙、悟净") }));
    }
}