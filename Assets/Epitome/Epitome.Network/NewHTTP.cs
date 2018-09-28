/*----------------------------------------------------------------
 * 文件名：NewHTTP
 * 文件功能描述：超文本传输协议
----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Epitome.Network
{
    //===============命名空间=========================
    using Manager;
    //===============命名空间=========================

    public class NewHTTP : MonoBehaviour
	{
		static NewHTTP mInstance;
		public static NewHTTP GetSingleton()
		{
			if(mInstance==null)
			{
				//尝试寻找该类的实例。此处不能用GameObject.Find，因为MonoBehaviour继承自Component。
				mInstance = Object.FindObjectOfType(typeof(NewHTTP)) as NewHTTP;

				if (mInstance == null)
				{                                       
					GameObject tempGame = new GameObject("NewHTTP");
					//防止被销毁
					DontDestroyOnLoad(tempGame);
					mInstance = tempGame.AddComponent<NewHTTP>();
				}
			}
			return mInstance;
		}

        //++++++++++++++++++++     信息输入输出     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        /// <summary>
        /// HTTP请求输入数据.
        /// </summary>
        public void HTTP_Request(object varObj)
		{
            StartCoroutine(WWWConnectionServer (varObj));
		}
			
		/// <summary>
		/// HTTP响应返回数据.
		/// </summary>
		void HTTP_Respond(string varEvent, WWW varWWW)
		{
			EventManager.GetSingleton ().BroadcastEvent (varEvent, varWWW as object);
		}

        //++++++++++++++++++++     连接服务器     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// HTTP连接服务器.
        /// </summary>
        IEnumerator WWWConnectionServer(object varObj)
		{
            List<object> tempObj = varObj as List<object>;

            WWW tempWWW;

            if (tempObj[1].ToString() == HTTPType.HTTP_Url.ToString())
                tempWWW = new WWW(tempObj[2].ToString());
            if (tempObj[1].ToString() == HTTPType.HTTP_byte.ToString())
                tempWWW = new WWW(tempObj[2].ToString(), tempObj[3] as byte[]);
            else if (tempObj[1].ToString() == HTTPType.HTTP_WWWForm.ToString())
                tempWWW = new WWW(tempObj[2].ToString(), tempObj[3] as WWWForm);
            else if (tempObj[1].ToString() == HTTPType.HTTP_Dictionary.ToString())
                tempWWW = new WWW(tempObj[2].ToString(), tempObj[3] as byte[], tempObj[4] as Dictionary<string, string>);
            else
                tempWWW = new WWW("");

            yield return tempWWW;
			HTTP_Respond (tempObj[0].ToString(), tempWWW);
		}
	}

    /// <summary>
	/// HTTP类型.
	/// </summary>
	public enum HTTPType
    {
        /// <summary>
        /// HTTP 参数（string varUrl）.
        /// </summary>
        HTTP_Url,
        /// <summary>
        /// HTTP 参数（string varUrl,WWWForm varWWWForm）.
        /// </summary>
        HTTP_WWWForm,
        /// <summary>
        /// HTTP 参数（string varUrl,byte[] varByte）.
        /// </summary>
        HTTP_byte,
        /// <summary>
        /// HTTP 参数（string varUrl,byte[] varByte,Dictionary<string,string> varDictionary）.
        /// </summary>
        HTTP_Dictionary,
    }
}