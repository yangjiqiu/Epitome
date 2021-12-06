using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Epitome.UIFrame
{
    public class UIManager : Singleton<UIManager>
    {
        private UIManager() { }

        class UIInfoData
        {
            public string UIType { get; private set; }

            // 当需要加载相同类型时使用
            public string UITag { get; private set; } 

            public string Path { get; private set; }

            public object[] UIParams { get; private set; }

            public UIInfoData(string uiType,string path,params object[] uiParams)
            {
                this.UIType = uiType;
                this.Path = path;
                this.UIParams = uiParams;
            }

            public UIInfoData(string uiType, string uiTag, string path, params object[] uiParams)
            {
                this.UIType = uiType;
                this.UITag = uiTag;
                this.Path = path;
                this.UIParams = uiParams;
            }
        }

        private Dictionary<string, string> UIPrefab_Paths = null;

        private Dictionary<string, GameObject> UIObject_FatherNodes = null;

        private Dictionary<string, GameObject> UIObject_Pool = null;

        private Dictionary<string, GameObject> UIObject_Pool_Idle = null;

        private Stack<UIInfoData> UIInfoStacks;

        // 可加载多个同类型UI
        private Dictionary<string, List<string>> UIType_Tags = null;

        public override void OnSingletonInit()
        {
            UIPrefab_Paths = new Dictionary<string, string>();
            UIObject_FatherNodes = new Dictionary<string, GameObject>();
            UIObject_Pool = new Dictionary<string, GameObject>();
            UIObject_Pool_Idle = new Dictionary<string, GameObject>();
            UIInfoStacks = new Stack<UIInfoData>();

            UIType_Tags = new Dictionary<string, List<string>>();

            base.OnSingletonInit();
        }

        public T GetUI<T>(string uiType) where T : UIBase
        {
            GameObject obj = GetUIObject(uiType);
            if (obj != null)
            {
                return obj.GetComponent<T>();
            }
            return null;
        }

        public GameObject GetUIObject(string uiType)
        {
            GameObject obj = null;
            if (!UIObject_Pool.TryGetValue(uiType, out obj))
            {
                throw new Exception("UIObjectPool TryGetValue Failure! uiType:" + uiType);
            }
            return obj;
        }

        public void PreloadUI(string uiType, string prefabPath)
        {
            PreloadUI(new string[] { uiType }, prefabPath);
        }
        public void PreloadUI(string[] uiTypes, string prefabPath)
        {
            PreloadUI(uiTypes, prefabPath, null);
        }
        public void PreloadUI(string uiType, string prefabPayh, GameObject fatherNode)
        {
            PreloadUI(new string[] { uiType }, prefabPayh, fatherNode);
        }
        public void PreloadUI(string[] uiTypes, string prefabPath, GameObject fatherNode)
        {
            for (int i = 0; i < uiTypes.Length; i++)
            {
                if (UIPrefab_Paths.ContainsKey(uiTypes[i])) continue;

                UIPrefab_Paths.Add(uiTypes[i], prefabPath);
                if (fatherNode != null)
                    UIObject_FatherNodes.Add(uiTypes[i], fatherNode);
            }
        }

        public void OpenUI(string uiType)
        {
            OpenUI(new string[] { uiType });
        }
        public void OpenUI(string[] uiTypes)
        {
            OpenUI(uiTypes, null);
        }
        public void OpenUI(string uiType, params object[] uiParams)
        {
            OpenUI(new string[] { uiType }, uiParams);
        }
        public void OpenUI(string[] uiTypes, params object[] uiParams)
        {
            OpenUI(false, uiTypes, uiParams);
        }
        public void OpenUICloseOthers(string uiType)
        {
            OpenUICloseOthers(new string[] { uiType });
        }
        public void OpenUICloseOthers(string[] uiTypes)
        {
            OpenUICloseOthers(uiTypes, null);
        }
        public void OpenUICloseOthers(string uiType, params object[] uiParams)
        {
            OpenUICloseOthers(new string[] { uiType }, uiParams);
        }
        public void OpenUICloseOthers(string[] uiTypes, params object[] uiParams)
        {
            OpenUI(true, uiTypes, uiParams);
        }

        public void OpenUI(bool isCloseOthers, string[] uiTypes, params object[] uiParams)
        {
            if (isCloseOthers)
            {
                CloseUIAll();
            }

            for (int i = 0; i < uiTypes.Length; i++)
            {
                string uiType = uiTypes[i];

                GameObject uiObj = null;

                if (!UIObject_Pool.TryGetValue(uiType, out uiObj))
                {
                    string path = null;
                    if (!UIPrefab_Paths.TryGetValue(uiType, out path))
                    {
                        throw new Exception(string.Format("Error:on {0} type ui path null", uiType));
                    }
                    else
                    {
                        UIInfoStacks.Push(new UIInfoData(uiType, path, uiParams));
                    }
                }
                else
                {
                    UIObject_Pool_Idle.TryGetValue(uiType, out uiObj);
                    if (uiObj == null) continue;

                    uiObj.SetActive(true);

                    UIBase ui = uiObj.GetComponent<UIBase>();

                    if (ui != null)
                    {
                        ui.SetUIWhenOpening(uiParams);
                        ui.Display();
                    }

                    UIObject_Pool_Idle.Remove(uiType);
                }
            }

            if (UIInfoStacks.Count > 0)
            {
                new Task(AsyncLoadData());
            }
        }

        public void OpenSameKindUI(string uiType, string tag, params object[] uiParams)
        {
            string type = string.Format("{0}_{1}", uiType, tag);

            GameObject uiObj = null;

            if (!UIObject_Pool.TryGetValue(type, out uiObj))
            {
                string path = null;
                if (!UIPrefab_Paths.TryGetValue(uiType, out path))
                {
                    throw new Exception(string.Format("Error:on {0} type ui path null", uiType));
                }
                else
                {
                    UIInfoStacks.Push(new UIInfoData(uiType, tag, path, uiParams));
                }
            }
            else
            {
                UIObject_Pool_Idle.TryGetValue(type, out uiObj);
                if (uiObj == null) return;

                uiObj.SetActive(true);

                UIBase ui = uiObj.GetComponent<UIBase>();

                if (ui != null)
                {
                    ui.SetUIWhenOpening(uiParams);
                    ui.Display();
                }

                UIObject_Pool_Idle.Remove(type);
            }

            if (UIInfoStacks.Count > 0)
            {
                new Task(AsyncLoadData());
            }
        }

        public void CloseSameKindUI(string uiType, string tag)
        {
            CloseUI(string.Format("{0}_{1}", uiType, tag));
        }

        private IEnumerator<int> AsyncLoadData()
        {
            UIInfoData uiInfoData = null;
            UnityEngine.Object prefabObj = null;
            GameObject uiObject = null;

            if (UIInfoStacks != null && UIInfoStacks.Count > 0)
            {
                do
                {
                    uiInfoData = UIInfoStacks.Pop();
                    prefabObj = ResManager.Instance.Load(uiInfoData.Path);

                    if (prefabObj != null)
                    {
                        uiObject = ResManager.Instance.Instantiate(prefabObj) as GameObject;

                        string name = "";
                        if (uiInfoData.UITag == null)
                        {
                            name = uiInfoData.UIType;
                        }
                        else
                        {
                            List<string> tags;

                            if (!UIType_Tags.TryGetValue(uiInfoData.UIType, out tags))
                                tags = new List<string>();

                            tags.Add(uiInfoData.UITag);
                            UIType_Tags.Add(uiInfoData.UIType, tags);

                            name = string.Format("{0}_{1}", uiInfoData.UIType, uiInfoData.UITag);
                        }

                        uiObject.name = name;

                        GameObject fatherNode = null;
                        UIObject_FatherNodes.TryGetValue(uiInfoData.UIType, out fatherNode);
                        if (fatherNode != null)
                        {
                            uiObject.transform.SetParent(fatherNode.transform, false);
                        }
                     
                        UIBase ui = uiObject.GetComponent<UIBase>();

                        if (ui == null)
                        {
                            throw new Exception("Error:On GetComponent<UIBase> in Instantiate GameObject is null!");
                        }

                        if (ui != null)
                        {
                            ui.SetUIWhenOpening(uiInfoData.UIParams);
                            ui.Display();
                        }

                        UIObject_Pool.Add(name, uiObject);
                    }
                } while (UIInfoStacks.Count > 0);
            }

            yield return 0;
        }



        public void CloseUI(string uiType)
        {
            GameObject uiObj = null;

            if (!UIObject_Pool.TryGetValue(uiType,out uiObj))
            {
                Debug.Log(string.Format("on {0} type UI GameObject is null", uiType.ToString()));
            }

            CloseUI(uiType, uiObj);
        }
        public void CloseUI(string[] uiTypes)
        {
            for (int i = 0; i < uiTypes.Length; i++)
            {
                CloseUI(uiTypes[i]);
            }
        }
        public void CloseUIAll()
        {
            List<string> keyList = new List<string>(UIObject_Pool.Keys.Except(UIObject_Pool_Idle.Keys));
            for (int i = 0; i < keyList.Count; i++)
            {
                Debug.Log("CloseUIAll:" + keyList[i]);
                GameObject uiObj = UIObject_Pool[keyList[i]];
                CloseUI(keyList[i], uiObj);
            }
        }

        public void CloseUI(string uiType,GameObject uiObj)
        {
            if (uiObj == null)
            {
                UIObject_Pool.Remove(uiType);
            }
            else
            {
                UIBase ui = uiObj.GetComponent<UIBase>();
                if (ui != null)
                {
                    ui.Hiding();

                    if (!UIObject_Pool_Idle.ContainsKey(uiType))
                        UIObject_Pool_Idle.Add(uiType, ui.gameObject);
                }
                else
                {
                    GameObject.Destroy(uiObj);
                    UIObject_Pool.Remove(uiType);
                }
            }
        }

        public void CloseUIHandler(object sender,ObjectState newState,ObjectState oldState)
        {
            UIBase ui = sender as UIBase;
            switch (newState)
            {
                case ObjectState.Disabled:
                    Debug.Log("UIObject_Pool_Idle");
                    UIObject_Pool_Idle.Add(ui.gameObject.name, ui.gameObject);
                    ui.StateChanged -= CloseUIHandler;
                    break;
                case ObjectState.Closing:
                    UIObject_Pool.Remove(ui.GetUIType());
                    ui.StateChanged -= CloseUIHandler;
                    break;
            }
        }

        private void OverflowClearing()
        {
            if (UIObject_Pool_Idle.Count >= 10)
            {
                for (int i = 10; i < UIObject_Pool_Idle.Keys.Count; i++)
                {
                    string key = UIObject_Pool_Idle.ElementAt(i).Key;

                    UIBase ui = UIObject_Pool_Idle[key].GetComponent<UIBase>();
                    if (ui != null)
                    {
                        ui.StateChanged += CloseUIHandler;
                        ui.Release();
                    }
                    else
                    {
                        GameObject.Destroy(ui);
                        UIObject_Pool.Remove(key);
                        UIObject_Pool_Idle.Remove(key);
                    }
                }
            }
        }
    }
}