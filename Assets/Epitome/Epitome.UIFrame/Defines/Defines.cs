using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Epitome.UIFrame
{
    public delegate void StateChangedEvent(object sender, ObjectState newState, ObjectState oldState);

    public enum ObjectState
    {
        None,

        Initial,

        Loading,

        Ready,

        Disabled,

        Closing
    }

    public enum UIMaskType
    {
        Lucency,            //完全透明，不能穿透
        Translucence,       //半透明，不能穿透
        ImPenetrable,       //低透明，不能穿透
        Pentrate            //可以穿透
    }

    public enum UINodeType
    {
        NormalNode,
        FixedNode,
        PopUpNode
    }

    public class Defines
    {
        public const string ROOTNODE = "UIFrame_RootNode";

        public const string MASKPANEL = "UIMaskPanel";

#if UNITY_EDITOR
        [MenuItem("Epitome/UIFrame/Create/NodeDirectory")]
        private static void CreateNodeDirectory()
        {
            Transform SelectedObject = Selection.activeGameObject.transform;

            Transform uiTrans = new GameObject(ROOTNODE).AddComponent<RectTransform>();
            uiTrans.SetParent(SelectedObject, false);

            SetUI((RectTransform)uiTrans);

            List<UINodeType> subfiles = new List<UINodeType>();

            foreach (UINodeType item in Enum.GetValues(typeof(UINodeType)))
            {
                subfiles.Add(item);
            }

            UINodeType[] subfile = subfiles.ToArray();

            for (int i = 0; i < subfile.Length; i++)
            {
                Transform nodeTrans = new GameObject(subfile[i].ToString()).AddComponent<RectTransform>();
                nodeTrans.SetParent(uiTrans, false);

                SetUI((RectTransform)nodeTrans);

                if (subfile[i] == UINodeType.PopUpNode)
                {
                    Transform maskTrans = new GameObject(MASKPANEL).AddComponent<RectTransform>();
                    maskTrans.SetParent(nodeTrans, false);

                    Image mask = maskTrans.gameObject.AddComponent<Image>();
                    mask.color = Color.black;

                    SetUI((RectTransform)maskTrans);

                    maskTrans.gameObject.SetActive(false);
                }
            }

            AssetDatabase.Refresh();
        }

        private static void SetUI(RectTransform rectTrans)
        {
            rectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            rectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            rectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
            rectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
        }
#endif
    }
}
