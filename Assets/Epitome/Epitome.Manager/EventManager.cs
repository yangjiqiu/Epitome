/*----------------------------------------------------------------
 * 文件名：EventManager
 * 文件功能描述：事件管理器
----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Epitome.Manager
{
    /// <summary>
    /// 声明委托
    /// </summary>
    public delegate void EventDelegate(object varObject);

    /// <summary>
    /// 事件管理器.
    /// </summary>
    public class EventManager
    {
        static EventManager mInstance;

        public static EventManager GetSingleton() { if (mInstance == null) { mInstance = new EventManager(); } return mInstance; }

        //++++++++++++++++++++     分界线     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 枚举事件集合
        /// </summary>
        Dictionary<EventEnum, List<EventDelegate>> mEventEnumList = new Dictionary<EventEnum, List<EventDelegate>>();

        /// <summary>
        /// 自定义事件集合
        /// </summary>
        Dictionary<string, List<EventDelegate>> mEventStrList = new Dictionary<string, List<EventDelegate>>();

        /// <summary>
        /// 枚举注册事件
        /// </summary>
        public void RegisterEvent(EventEnum varEventEnum, EventDelegate varDele)
        {
            List<EventDelegate> tempDeleList;
            mEventEnumList.TryGetValue(varEventEnum, out tempDeleList);
            if (tempDeleList == null)
            {
                tempDeleList = new List<EventDelegate>();
                mEventEnumList.Add(varEventEnum, tempDeleList);
            }
            tempDeleList.Add(varDele);
        }

        /// <summary>
        /// 自定义注册事件
        /// </summary>
        public void RegisterEvent(string varStr, EventDelegate varDele)
        {
            List<EventDelegate> tempDeleList;
            mEventStrList.TryGetValue(varStr, out tempDeleList);
            if (tempDeleList == null)
            {
                tempDeleList = new List<EventDelegate>();
                mEventStrList.Add(varStr, tempDeleList);
            }
            tempDeleList.Add(varDele);
        }

        /// <summary>
        /// 枚举广播事件
        /// </summary>
        public void BroadcastEvent(EventEnum varEventEnum, object varParameter)
        {
            List<EventDelegate> tempDeleList;
            mEventEnumList.TryGetValue(varEventEnum, out tempDeleList);
            if (tempDeleList == null) return;
            foreach (var v in tempDeleList) v(varParameter);
        }

        /// <summary>
        /// 自定义广播事件
        /// </summary>
        public void BroadcastEvent(string varStr, object varParameter)
        {
            List<EventDelegate> tempDeleList;
            mEventStrList.TryGetValue(varStr, out tempDeleList);
            if (tempDeleList == null) return;
            foreach (var v in tempDeleList) v(varParameter);
        }

        /// <summary>
        /// 枚举解注册事件
        /// </summary>
        public void UnRegisterEvent(EventEnum varEventEnum)
        {
            List<EventDelegate> tempDeleList;
            mEventEnumList.TryGetValue(varEventEnum, out tempDeleList);
            if (tempDeleList != null) mEventEnumList.Remove(varEventEnum);
        }

        /// <summary>
        /// 自定义解注册事件
        /// </summary>
        public void UnRegisterEvent(string varStr)
        {
            List<EventDelegate> tempDeleList;
            mEventStrList.TryGetValue(varStr, out tempDeleList);
            if (tempDeleList != null) mEventStrList.Remove(varStr);
        }
    }
}