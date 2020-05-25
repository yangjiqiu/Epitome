using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Epitome.Manager.Window
{
    public enum UnityLayer
    {
        ShowUILayer = 5,
        HideUILayer = 8,
    }

    public abstract class View : IView
    {
        protected GameObject mView = null;

        public virtual void Init(GameObject varView) { this.mView = varView; }

        /// <summary>
        /// 显示
        /// </summary>
        public void Show() { mView.layer = GetLayer(UnityLayer.ShowUILayer); }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide() { mView.layer = GetLayer(UnityLayer.HideUILayer); }

        int GetLayer(UnityLayer varUnityLayer) { return (int)varUnityLayer; }
    }
}