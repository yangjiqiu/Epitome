/*----------------------------------------------------------------
 * 文件名：Hierarchy
 * 文件功能描述：层级
----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Epitome.Utility
{
    /// <summary>
    /// 层级面板
    /// </summary>
    public class Hierarchy
    {
        static Hierarchy mInstance;

        public static Hierarchy GetSingleton() { if (mInstance == null) { mInstance = new Hierarchy(); } return mInstance; }

        /// <summary>
        /// 查找子物体
        /// </summary>
        public Transform[] FindSubObjects(Transform varTrans, string varName, int varHierarchy = 0)
        {
            List<Transform> tempTrans = new List<Transform>();

            foreach (Transform v in varTrans)
            {
                if (varName == v.name)
                    tempTrans.Add(v);
            }

            foreach (Transform v in varTrans)
            {
                Transform[] tempReturn = FindSubObjects(v, varName, varHierarchy);
                foreach (Transform vv in tempReturn)
                {
                    tempTrans.Add(vv);
                }
            }

            return tempTrans.ToArray();
        }

        /// <summary>
        /// 获取子物体组件
        /// </summary>
        public T[] GetSubObjectsComponent<T>(Transform varTran) where T : Component
        {
            List<T> tempTrans = new List<T>();

            if (varTran.GetComponent<T>() != null)
                tempTrans.Add(varTran.GetComponent<T>());

            if (varTran.childCount > 0)
            {
                foreach (Transform v in varTran.transform)
                {
                    foreach (T vv in GetSubObjectsComponent<T>(v))
                        tempTrans.Add(vv);
                }
            }

            return tempTrans.ToArray();
        }

#if NGUI
        /// <summary>  
        /// 别忘了 调用 NGUITools.NormalizePanelDepths() 这样depth就不会无限往上增加！！！  
        /// </summary>  
        /// <returns></returns>  
        public int MaxPanelDepth()
        {
            NGUITools.NormalizePanelDepths();
            /**********高级写法****************/
            int depth = 0;
            UIPanel[] list = NGUITools.FindActive<UIPanel>();
            if (list.Length > 0)
            {
                System.Array.Sort(list, UIPanel.CompareFunc);
                for (int i=1;i< list.Length-1;i++ )
                {
                    if (list[list.Length - i].depth != 100)
                    {
                        depth = list[list.Length - i].depth;
                        break;
                    }
                }
            }
            return depth;
            /**********高级写法****************/
        }
        /// <summary>  
        /// 改变GameObject的uipanel的Depth 如果子对象还有uipanel 继续累加 保证层次正确  
        /// 例子：UITools.ChangeGameObjectDepths(go,UITools.MaxPanelDepth());  
        /// </summary>  
        public void ChangeGameObjectDepths(GameObject go, int depth)
        {
            if (!go.GetComponent<UIPanel>())
            {
                return;
            }
            int lastDepth = go.GetComponent<UIPanel>().depth;
            int cha = depth - lastDepth;
            go.GetComponent<UIPanel>().depth = depth;
            UIPanel[] list = Hierarchy.GetSingleton().GetSubObjectsComponent<UIPanel>(go.transform);
            int size = list.Length;
            if (size > 1)
            {
                for (int i = 1; i < list.Length; i++)
                {
                    list[i].GetComponent<UIPanel>().depth += cha;
                }
            }
        }
#endif
    }
}
