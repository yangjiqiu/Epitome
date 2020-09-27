/* $Header:   Assets/Epitome/Epitome.LogSystem/OnGUIScaler.cs   1.0   2020/06/29 Monday AM 11:07:38   Ji Qiu .Yang   2017.1.1f1  $ */
/***********************************************************************************************
 ***              C O N F I D E N T I A L  ---  E P I T O M E  S T U D I O S                 ***
 ***********************************************************************************************
 *                                                                                             *
 *                 Project Name : Epitome                                                      *
 *                                                                                             *
 *                    File Name : OnGUIScaler.cs                                               *
 *                                                                                             *
 *                   Programmer : Ji Qiu .Yang                                                 *
 *                                                                                             *
 *                   Start Date : 2020/06/29                                                   *
 *                                                                                             *
 *                  Last Update : 2020/06/29                                                   *
 *                                                                                             *
 *---------------------------------------------------------------------------------------------*
 * Functions:                                                                                  *
 * - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Epitome
{
    public class OnGUIScaler : MonoBehaviour
    {
        [SerializeField]
        private Vector2 ReferenceResolution;
        [SerializeField]
        [Range(0, 1)]
        private float Match;

    }
}
