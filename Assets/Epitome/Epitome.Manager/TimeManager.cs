/*----------------------------------------------------------------
 * 文件名：TimeManager
 * 文件功能描述：时间管理器
----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Epitome.Manager
{
    /// <summary>
    /// 时间管理器.
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        static TimeManager mInstance;

        public static TimeManager GetSingleton()
        {
            if (mInstance == null)
            {
                //尝试寻找该类的实例。此处不能用GameObject.Find，因为MonoBehaviour继承自Component。
                mInstance = Object.FindObjectOfType(typeof(TimeManager)) as TimeManager;

                if (mInstance == null)
                {
                    GameObject tempGame = new GameObject("TimeManager");
                    //防止被销毁
                    DontDestroyOnLoad(tempGame);
                    mInstance = tempGame.AddComponent<TimeManager>();
                }
            }
            return mInstance;
        }

        //++++++++++++++++++++     分界线     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        Coroutine mCoroutine;

        /// <summary>
        /// 时间集合
        /// </summary>
        Dictionary<float, List<object>> mTimeDict = new Dictionary<float, List<object>>();

        /// <summary>
        /// 注册时间事件
        /// </summary>
        public string RegisterTime(EventDelegate varDele, float varTime, object varObj = null)
        {
            float tempTime = Time.time + varTime;

            if (mCoroutine != null)
                StopCoroutine(mCoroutine);

            List<object> tempObjList;
            mTimeDict.TryGetValue(tempTime, out tempObjList);
            if (tempObjList == null)
            {
                tempObjList = new List<object>();
                tempObjList.Add(varObj);
            }
            else
                mTimeDict.Remove(tempTime);
            mTimeDict.Add(tempTime, tempObjList);

            mCoroutine = StartCoroutine(Timing());

            EventManager.GetSingleton().RegisterEvent("Time" + tempTime.ToString(), varDele);

            return tempTime.ToString();
        }

        void BroadcastTime(float varTime, object varObj = null)
        {
            //广播
            EventManager.GetSingleton().BroadcastEvent("Time" + varTime.ToString(), varObj);
            //解注册
            UnRegister(varTime.ToString());
        }

        void UnRegister(string varTime)
        {
            if (mTimeDict.ContainsKey(float.Parse(varTime)))//解注册
                EventManager.GetSingleton().UnRegisterEvent("Time" + varTime);
        }

        public void UnRegisterTime()
        {
            foreach (KeyValuePair<float, List<object>> v in mTimeDict)
            {
                UnRegister(v.Key.ToString());
            }
        }

        /// <summary>
        /// 定时
        /// </summary>
        IEnumerator Timing()
        {
            while (true)
            {
                if (mTimeDict.Count > 0)
                {
                    Dictionary<float, List<object>> tempList = new Dictionary<float, List<object>>();
                    foreach (KeyValuePair<float, List<object>> v in mTimeDict)
                    {
                        if (v.Key <= Time.time)
                        {
                            tempList.Add(v.Key, v.Value);
                        }
                    }

                    foreach (KeyValuePair<float, List<object>> v in tempList)
                    {
                        mTimeDict.Remove(v.Key);

                        if (v.Value.Count != 0)
                        {
                            for (int i = 0; i < v.Value.Count; i++)
                            {
                                BroadcastTime(v.Key, v.Value[i]);
                            }
                        }
                        else
                            BroadcastTime(v.Key);
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
