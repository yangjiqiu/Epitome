﻿/*----------------------------------------------------------------
 * 文件名：Features
 * 文件功能描述：功能
----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Epitome.Utility
{
    /// <summary>
    /// 功能集合
    /// </summary>
    public class Features
    {
        static Features mInstance;

        public static Features GetSingleton() { if (mInstance == null) { mInstance = new Features(); } return mInstance; }
        
        //++++++++++++++++++++     分界线     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 物体激死激活.
        /// </summary>               
        public void Activation(GameObject varGame, bool varBool) { varGame.SetActive(varBool); }

        /// <summary>
        /// 物体状态.
        /// </summary>               
        public bool ObjectState(GameObject varGame) { return varGame.activeSelf; }

        /// <summary>
        /// 平面距离
        /// </summary>
        public float PlaneDistance(Vector2 varStart, Vector2 varEnd) { return Vector2.Distance(varStart, varEnd); }

        /// <summary>
        /// 空间距离
        /// </summary>
        public float SpaceDistance(Vector3 varStart, Vector3 varEnd) { return Vector3.Distance(varStart, varEnd); }

#if UNITY_EDITOR
        /// <summary>
        /// 添加标签
        /// </summary>
        public void AddTag(string varTag, GameObject varObj)
        {
            if (!IsHasTag(varTag))
            {
                SerializedObject tempTagManager = new SerializedObject(varObj);//序列化物体
                SerializedProperty tempIterator = tempTagManager.GetIterator();//序列化属性
                while (tempIterator.NextVisible(true))//下一属性的可见性
                {
                    if (tempIterator.name == "m_TagString")
                    {
                        tempIterator.stringValue = varTag;
                        tempTagManager.ApplyModifiedProperties();
                    }
                }
            }
        }

        /// <summary>
        /// 是否有标签
        /// </summary>
        public bool IsHasTag(string varTog)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
            {
                if (UnityEditorInternal.InternalEditorUtility.tags[i].Equals(varTog))
                    return true;
            }
            return false;
        }
#endif
    }
}