/*----------------------------------------------------------------
 * 文件名：ClickEvent
 * 文件功能描述：点击事件
----------------------------------------------------------------*/
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;


namespace Epitome.Utility
{
    /// <summary>
    /// 点击事件
    /// </summary>
    public class UIEventListener : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
    {
        public delegate void UIClickEvent(GameObject varGO, PointerEventData varData);


        public event UIClickEvent OnMouseEnter;

        public event UIClickEvent OnMouseDown;

        public event UIClickEvent OnMouseClick;

        public event UIClickEvent OnMouseUp;

        public event UIClickEvent OnMouseExit;

        /// <summary>
        /// UGUI 进入事件
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData) {
            if (OnMouseEnter != null)
                OnMouseEnter(this.gameObject, eventData);
        }

        /// <summary>
        /// UGUI 按下事件
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)  {
            if (OnMouseDown != null)
                OnMouseDown(this.gameObject, eventData);
        }

        /// <summary>
        /// UGUI 点击事件
        /// </summary>
        public void OnPointerClick(PointerEventData eventData) {
            if (OnMouseClick != null)
                OnMouseClick(this.gameObject, eventData);
        }

        /// <summary>
        /// UGUI 抬起事件
        /// </summary>
        public void OnPointerUp(PointerEventData eventData) {
            if (OnMouseUp != null)
                OnMouseUp(this.gameObject, eventData);
        }

        /// <summary>
        /// UGUI 滑出事件
        /// </summary>
        public void OnPointerExit(PointerEventData eventData) {
            if (OnMouseExit != null)
                OnMouseExit(this.gameObject, eventData);
        }

        /// <summary>
        /// NGUI按钮 按下抬起事件
        /// </summary>
        //void OnPress(bool tempIsPressed) { Debug.Log(tempIsPressed); if (tempIsPressed) ButtonDown(); else ButtonUp(); }
    }
}