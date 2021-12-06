/* $Header:   Assets/Examples/Epitome.UIFrame/Scripts/FunctionModule/CaraSynthesis/CardData.cs   1.0   2021/10/30 Saturday PM 05:29:48   Ji Qiu .Yang   2017.1.1f1  $ */
/***********************************************************************************************
 ***              C O N F I D E N T I A L  ---  E P I T O M E  S T U D I O S                 ***
 ***********************************************************************************************
 *                                                                                             *
 *                 Project Name : Epitome                                                      *
 *                                                                                             *
 *                    File Name : CardData.cs                                                  *
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

/// <summary>卡牌品种</summary>
public enum CardVariety
{
    No,

    /// <summary>普通卡牌</summary>
    OrdinaryCard,

    /// <summary>稀有卡牌</summary>
    UncommonCard,
}

/// <summary>卡牌属性</summary>
public class CardAttribute
{
    /// <summary>进攻值</summary>
    public int attackValue;

    /// <summary>防御值</summary>
    public int defenseValue;

    /// <summary>闪避值</summary>
    public int evasionValue;

    /// <summary>暴击值</summary>
    public int critValue;

    public CardAttribute(int attack, int defense, int evasion, int crit)
    {
        attackValue = attack;
        defenseValue = defense;
        evasionValue = evasion;
        critValue = crit;
    }
}

/// <summary>卡牌技能</summary>
public class CardSkill
{
    /// <summary>技能名字</summary>
    public string skillName;

    /// <summary>技能等级</summary>
    public int skillGrade;

    /// <summary>技能描述</summary>
    public string skillDescription;

    public CardSkill(string name, int grade, string description)
    {
        skillName = name;
        skillGrade = grade;
        skillDescription = description;
    }
}

/// <summary>卡牌数据</summary>
public class CardData
{
    /// <summary>卡牌名字</summary>
    public string cardName;

    /// <summary>卡牌品种</summary>
    public CardVariety cardVariety;

    /// <summary>卡牌等级</summary>
    public string cardGrade;

    /// <summary>卡牌描述</summary>
    public string cardDescription;

    /// <summary>卡牌属性</summary>
    public CardAttribute cardAttribute;

    /// <summary>卡牌技能</summary>
    public List<CardSkill> cardSkills;

    public CardData(string name, CardVariety variety, string grade, string description, CardAttribute attribute, List<CardSkill> skills)
    {
        cardName = name;
        cardVariety = variety;
        cardGrade = grade;
        cardDescription = description;
        cardAttribute = attribute;
        cardSkills = skills;
    }
}