using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Epitome.Utility.Load
{
    public delegate void LoadJsonFinish(object data);


    public static class Loading
    {
#if UNITY_2017
        public static IEnumerator LoadJson<T>(string path, LoadJsonFinish finish)
        {
            WWW www = new WWW(path);
            yield return www;
            while (www.isDone == false)
            {
                yield return new WaitForEndOfFrame();
            }
            string skipBom = Encoding.UTF8.GetString(www.bytes, 3, www.bytes.Length - 3);
            finish(JsonMapper.ToObject<T>(skipBom));
        }

# elif UNITY_2018
        public static IEnumerator LoadJson(string path, LoadJsonFinish finish)
        {
            UnityWebRequest uwr = new UnityWebRequest(path);

            yield return uwr.SendWebRequest();

            string skipBom = Encoding.UTF8.GetString(www.downloadHandler.data, 3, www.downloadHandler.data.Length - 3);
            finish(JsonMapper.ToObject<T>(skipBom));
        }


        public static IEnumerator Send(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                UnityEngine.Debug.Log(request.error);
            }
            else
            {
                UnityEngine.Debug.Log(request.downloadHandler.text);
            }
        }

        public static IEnumerator GetAssetBundle(string url)
        {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(path);
            yield return request.SendWebRequest();

            if (request.isDone)
            {
                AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request);
                if (ab)
                {
                    Instantiate(ab.LoadAsset("Cube"));
                }
            }
            request.Dispose();//记得使用完要释放呀 
        }

        public static IEnumerator GetTexture1()
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture("http://www.my-server.com/image.png");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            }
            www.Dispose();//记得使用完要释放呀
        }


        public static IEnumerator GetTexture2()
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture("http://www.my-server.com/image.png");
            yield return www.SendWebRequest();

            Texture myTexture = DownloadHandlerTexture.GetContent(www);
        }

#endif

        private static IEnumerator Load(string url)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            }
            www.Dispose();//记得使用完要释放呀	
        }
    }
}
