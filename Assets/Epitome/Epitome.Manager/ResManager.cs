using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Object = UnityEngine.Object;

namespace Epitome
{
    public class AssetInfo
    {
        private Object _Object;
        public Type AssetType { get; set; }
        public string Path { get; set; }
        public int RefCount { get; set; }

        public bool IsLoaded { get { return _Object != null; } }

        public Object AssetObject
        {
            get
            {
                if (_Object == null)
                {
                    ResourcesLoad();
                }

                return _Object;
            }
        }

        public IEnumerator GetCorouttineObject(Action<Object> loaded)
        {
            while (true)
            {
                yield return null;

                if (_Object == null)
                {
                    ResourcesLoad();
                    yield return null;
                }

                if (loaded != null)
                {
                    loaded(_Object);
                }

                yield break;
            }
        }

        private void ResourcesLoad()
        {
            try {
                _Object = Resources.Load(Path);
                if (_Object == null)
                {
                    Debug.Log("Resources Load Failure! Path:" + Path);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        public IEnumerator GetAsyncObject(Action<Object> loaded)
        {
            return GetAsyncObject(loaded, null);
        }

        public IEnumerator GetAsyncObject(Action<Object> loaded, Action<float> progress)
        {
            if (_Object != null)
            {
                loaded(_Object);
                yield break;
            }

            ResourceRequest resRequest = Resources.LoadAsync(Path);

            while (resRequest.progress < 0.9)
            {
                if (progress != null)
                {
                    progress(resRequest.progress);
                }
                yield return null;
            }

            while (!resRequest.isDone)
            {
                if (progress != null)
                {
                    progress(resRequest.progress);
                }

                yield return null;
            }

            _Object = resRequest.asset;
            if (loaded != null)
            {
                loaded(_Object);
            }

            yield return resRequest;
        }
    }

    public class ResManager : Singleton<ResManager>
    {
        protected ResManager() { }

        private Dictionary<string, AssetInfo> assetInfoDict = null;

        public override void OnSingletonInit()
        {
            assetInfoDict = new Dictionary<string, AssetInfo>();

            base.OnSingletonInit();
        }

        public Object LoadInstance(string path)
        {
            Object obj = Load(path);
            return Instantiate(obj);
        }

        public void LoadCoroutineInstance(string path,Action<Object> loaded)
        {
            LoadCoroutine(path, (obj) => { Instantiate(obj, loaded); });
        }

        public void LoadAsyncInstance(string path, Action<Object> loaded)
        {
            LoadAsync(path, (obj) => { Instantiate(obj, loaded); });
        }

        public void LoadAsyncInstance(string path, Action<Object> loaded, Action<float> progress)
        {
            LoadAsync(path, (obj) => { Instantiate(obj, loaded); }, progress);
        }

        public Object Load(string path)
        {
            AssetInfo assetInfo = GetAssetInfo(path);
            
            if (assetInfo!=null)
            {
                return assetInfo.AssetObject;
            }
            Debug.Log("assetInfo null");
            return null;
        }

        public void LoadCoroutine(string path,Action<Object> loaded)
        {
            AssetInfo assetInfo = GetAssetInfo(path, loaded);
            if (assetInfo!=null)
            {
                new Task(assetInfo.GetCorouttineObject(loaded));
            }
        }

        public void LoadAsync(string path, Action<Object> loaded)
        {
            LoadAsync(path, loaded, null);
        }

        public void LoadAsync(string path,Action<Object> loaded,Action<float> progress)
        {
            AssetInfo assetInfo = GetAssetInfo(path, loaded);
            if (assetInfo != null)
            {
                new Task(assetInfo.GetAsyncObject(loaded, progress));
            }
        }

        public AssetInfo GetAssetInfo(string path)
        {
           return GetAssetInfo(path,null);
        }

        public AssetInfo GetAssetInfo(string path,Action<Object> loaded)
        {
            AssetInfo assetInfo = null;
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Error:null path name");

                if (loaded !=null)
                {
                    loaded(null);
                    return assetInfo;
                }
            }

            if (!assetInfoDict.TryGetValue(path,out assetInfo))
            {
                assetInfo = new AssetInfo();
                assetInfo.Path = path;
                assetInfoDict.Add(path, assetInfo);
            }

            assetInfo.RefCount++;

            return assetInfo;
        }

        public Object Instantiate(Object obj)
        {
            return Instantiate(obj,null);
        }

        public Object Instantiate(Object obj,Action<Object> loaded)
        {
            Object retObj = null;

            if (obj != null)
            {
                retObj = MonoBehaviour.Instantiate(obj);

                if (retObj != null)
                {
                    if (loaded != null)
                    {
                        loaded(retObj);
                        return null;
                    }

                    return retObj;
                }
                else
                {
                    Debug.LogError("Error:null instantiate retObj");
                }
            }
            else
            {
                Debug.LogError("Error:null Resources Load return obj");
            }

            return null;
        }
    }
}
