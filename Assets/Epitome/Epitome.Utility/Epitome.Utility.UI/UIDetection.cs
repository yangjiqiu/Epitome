using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Epitome;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Epitome.Utility.UI
{
	/// <summary>
	/// UI检测
	/// </summary>
	public class UIDetection : Singleton<UIDetection>
	{
        private UIDetection() { }

        /// <summary>
        /// 检测GUI射线检测对象
        /// </summary>
        /// <param name="graphicRaycaster">Canvas中的GraphicRaycaster组件</param>
        /// <returns></returns>
        public bool CheckGuiRaycastObjects(GraphicRaycaster graphicRaycaster)
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null || graphicRaycaster == null) return false;
            PointerEventData eventData = new PointerEventData(eventSystem);
            eventData.pressPosition = Input.mousePosition;
            eventData.position = Input.mousePosition;

            List<RaycastResult> resultList = new List<RaycastResult>();
            graphicRaycaster.Raycast(eventData, resultList);
            return resultList.Count > 0;
        }
    }
}
