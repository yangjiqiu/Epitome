/* $Header:   Assets/Examples/Epitome.UIFrame/Scripts/FunctionModule/CaraSynthesis/CardSlot.cs   1.0   2021/10/30 Saturday PM 04:22:09   Ji Qiu .Yang   2017.1.1f1  $ */
/***********************************************************************************************
 ***              C O N F I D E N T I A L  ---  E P I T O M E  S T U D I O S                 ***
 ***********************************************************************************************
 *                                                                                             *
 *                 Project Name : Epitome                                                      *
 *                                                                                             *
 *                    File Name : CardSlot.cs                                                  *
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

/// <summary>卡槽接口</summary>
public interface ICardSlot
{
    /// <summary>放置卡牌</summary>
    /// <param name="data"></param>
    void PutCard(CardData data);

    /// <summary>清除卡牌</summary>
    void ClearCard();

    Vector3 Pos { get; }
}

/// <summary>卡槽</summary>
public class CardSlot : MonoBehaviour, ICardSlot
{
    ICardUnit cardUnit;
    public Vector3 Pos { get { return transform.position; } }
    protected void Awake()
    {
        cardUnit = GetComponentInChildren<ICardUnit>(true);
    }

    public void PutCard(CardData data)
    {
        cardUnit.SetCardData(data);
    }

    public void ClearCard()
    {
        cardUnit.SetCardData(null);
        cardUnit.SetCardActive(false);
    }
}