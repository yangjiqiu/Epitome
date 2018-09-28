/*----------------------------------------------------------------
 * 文件名：Data
 * 文件功能描述：数据
----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Web;//浏览器与服务器通信
using System.Text;//字符编码
using System.Linq;
using System.IO;
using System;
using LitJson;

namespace Epitome.Utility
{
    public class Data
    {
        static Data mInstance;

        public static Data GetSingleton() { if (mInstance == null) { mInstance = new Data(); } return mInstance; }

        //++++++++++++++++++++     分界线     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 添加包头.
        /// </summary>
        public byte[] AddHeader(byte[] varByte)
        {
            byte[] tempCount = StringTurnBytes(varByte.Length.ToString());
            byte[] tempByte = StringTurnBytes(tempCount.Length.ToString());
            return MergeBytes(MergeBytes(tempByte, tempCount), varByte);
        }

        //++++++++++++++++++++     拆装箱     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 装箱.
        /// </summary>
        public object Packing<Type>(Type varType) { return (object)varType; }

        /// <summary>
		/// 拆箱.
		/// </summary>
		public Type Unboxing<Type>(object varObj) { return (Type)varObj; }

        //++++++++++++++++++++     合并数据     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 合并字节数组.
        /// </summary>
        public byte[] MergeBytes(byte[] varByte1,byte[] varByte2)
        {
            byte[] tempByte = new byte[varByte1.Length+ varByte2.Length];
            Array.Copy(varByte1, 0, tempByte, 0, varByte1.Length);
            Array.Copy(varByte2, varByte1.Length, tempByte, 0, varByte2.Length);
            return tempByte;
        }

        //++++++++++++++++++++     编解码     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        // 编码.
        string Encode(string varStr, Encoding varEncoding) { return HttpUtility.UrlEncode(varStr, varEncoding); }

        // 解码.
        string Decode(string varStr, Encoding varEncoding) { return HttpUtility.UrlDecode(varStr, varEncoding); }

        /// <summary>
        /// 编码UTF8.
        /// </summary>
        public string EncodeUTF8(string varStr) { return Encode(varStr, Encoding.UTF8); }

        /// <summary>
        /// 解码UTF8.
        /// </summary>
        public string DecodeUTF8(string varStr) { return Decode(varStr, Encoding.UTF8); }

        //++++++++++++++++++++     类型转换     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 字符串转字节(UTF8).
        /// </summary>
        public byte[] StringTurnBytes(string varStr) { return Encoding.UTF8.GetBytes(varStr); }

        /// <summary>
        /// 字节转字符串.
        /// </summary>
        public string BytesTurnString(byte[] varBytes) { return Encoding.Default.GetString(varBytes); }

        /// <summary>
        /// 数组转集合.
        /// </summary>
        public List<T> ArrayTurnList<T>(T[] varArray)
        {
            List<T> tempList = new List<T>();
            foreach (T v in varArray) { tempList.Add(v); }
            return tempList;
        }

        /// <summary>
        /// 数组转集合.
        /// </summary>
        public T[] ListTurnArray<T>(List<T> varList)
        {
            T[] tempArray = new T[varList.Count];
            for (int i = 0; i < tempArray.Length; i++) { tempArray[i] = varList[i]; }
            return tempArray;
        }

        /// <summary>
        /// byte[2]转float[1].
        /// </summary>
        public float[] BytesTurnFloat(byte[] varBytes)
        {
            float[] tempFloat = new float[varBytes.Length / 2];
            for (int i = 0; i < tempFloat.Length; i++)
            {
                Byte[] tempByteArr = new Byte[2];
                Array.Copy(varBytes, i * 2, tempByteArr, 0, 2);

                int tempRescaleFactor = 32767;
                tempFloat[i] = BitConverter.ToInt16(tempByteArr, tempByteArr.Length)/ tempRescaleFactor;
            }
            return tempFloat;
        }

        /// <summary>
        /// float[1]转byte[2].
        /// </summary>
        public byte[] FloatTurnBytes(float[] varFloat)
        {
            Int16[] tempInt16 = new Int16[varFloat.Length];
            Byte[] tempBytes = new Byte[varFloat.Length * 2];
            int tempRescaleFactor = 32767;
            for (int i = 0; i < varFloat.Length; i++)
            {
                tempInt16[i] = (short)(varFloat[i] * tempRescaleFactor);
                Byte[] tempByteArr = new Byte[2];
                tempByteArr = BitConverter.GetBytes(tempInt16[i]);
                tempByteArr.CopyTo(tempBytes, i * 2);
            }
            return tempBytes;
        }

        //++++++++++++++++++++     数据解析     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Json转字典.
        /// </summary>
        public Dictionary<string, string> JsonTurnDict(string varStr)
        {
            Dictionary<string, string> tempDict = new Dictionary<string, string>();

            JsonData tempJsonData = JsonMapper.ToObject(varStr);

            foreach (KeyValuePair<string, JsonData> v in tempJsonData.Inst_Object)
            {
                if (v.Value.IsArray)
                    tempDict.Add(this.DecodeUTF8(v.Key), this.DecodeUTF8(v.Value.ToJson()));
                else
                    tempDict.Add(this.DecodeUTF8(v.Key), this.DecodeUTF8(v.Value.ToString()));
            }

            return tempDict;
        }

        /// <summary>
        /// Json转list.
        /// </summary>
        public List<string> JsonTurnList(string varStr)
        {
            List<string> tempList = new List<string>();

            JsonData tempJsonData = JsonMapper.ToObject(varStr);

            for (int i = 0; i < tempJsonData.Count; i++)
            {
                tempList.Add(tempJsonData[i].ToString());
            }

            return tempList;
        }

        //++++++++++++++++++++     数据编码     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Json数据编码.
        /// </summary>
        public JsonData JsonEncode(JsonData varData)
        {
            JsonData tempJsonData = new JsonData();
            if (varData.IsObject)
            {
                foreach (KeyValuePair<string, JsonData> v in varData.Inst_Object)
                {
                    if (v.Value.IsString)
                        tempJsonData[EncodeUTF8(v.Key)] = EncodeUTF8(v.Value.ToString());
                    else
                        tempJsonData[EncodeUTF8(v.Key)] = JsonEncode(v.Value);
                }
                return tempJsonData;
            }
            else if (varData.IsArray)
            {
                foreach (JsonData v in varData)
                {
                    if (v.IsString)
                        tempJsonData[tempJsonData.Count] = EncodeUTF8(varData.ToString());
                    else
                        tempJsonData[tempJsonData.Count] = JsonEncode(v);
                }
                return tempJsonData;
            }
            return new JsonData();
        }

        //++++++++++++++++++++     获取组件     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 获取组件
        /// </summary>
        public T GetInterface<T>(GameObject inObj) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                return null;
            }
            var tmps = inObj.GetComponents<Component>().OfType<T>();
            if (tmps.Count() == 0)
            {
                return null;
            }
            return tmps.First();
        }

        //++++++++++++++++++++     获取时间     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 获取当前时间
        /// </summary>
        public string GetCurrentTime(string[] varInsert)
        {
            //Debug.Log(Data.GetSingleton().GetCurrentTime(new string[] { "", "", "_", "", "" }));
            return System.DateTime.Now.ToString(string.Format("yyyy{0}MM{1}dd{2}HH{3}mm{4}ss", varInsert));
        }
    }
}