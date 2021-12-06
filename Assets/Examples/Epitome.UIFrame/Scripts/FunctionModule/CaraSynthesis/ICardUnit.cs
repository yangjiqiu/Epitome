/* $Header:   Assets/Examples/Epitome.UIFrame/Scripts/FunctionModule/CaraSynthesis/ICardUnit.cs   1.0   2021/10/30 Saturday PM 05:29:03   Ji Qiu .Yang   2017.1.1f1  $ */
/***********************************************************************************************
 ***              C O N F I D E N T I A L  ---  E P I T O M E  S T U D I O S                 ***
 ***********************************************************************************************
 *                                                                                             *
 *                 Project Name : Epitome                                                      *
 *                                                                                             *
 *                    File Name : ICardUnit.cs                                                 *
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

/// <summary>卡牌单位接口</summary>
public interface ICardUnit
{
    CardData cardData { get; }

    void SetCardActive(bool value);

    void SetCardData(CardData data);

    void SelectCard(bool value);
}
