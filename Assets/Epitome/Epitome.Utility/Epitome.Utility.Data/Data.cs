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
    public static class Data
    {
        //++++++++++++++++++++     分界线     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 添加包头.
        /// </summary>
        public static byte[] AddHeader(byte[] varByte)
        {
            byte[] tempCount = StringTurnBytes(varByte.Length.ToString());
            byte[] tempByte = StringTurnBytes(tempCount.Length.ToString());
            return MergeBytes(MergeBytes(tempByte, tempCount), varByte);
        }

        //++++++++++++++++++++     拆装箱     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 装箱.
        /// </summary>
        public static object Packing<Type>(Type varType) { return (object)varType; }

        /// <summary>
		/// 拆箱.
		/// </summary>
		public static Type Unboxing<Type>(object varObj) { return (Type)varObj; }

        //++++++++++++++++++++     合并数据     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 合并字节数组.
        /// </summary>
        public static byte[] MergeBytes(byte[] varByte1,byte[] varByte2)
        {
            byte[] tempByte = new byte[varByte1.Length+ varByte2.Length];
            Array.Copy(varByte1, 0, tempByte, 0, varByte1.Length);
            Array.Copy(varByte2, varByte1.Length, tempByte, 0, varByte2.Length);
            return tempByte;
        }

        /// <summary>
        /// 合并数组.
        /// </summary>
        public static T[] MergeArray<T>(T[] array1, T[] array2)
        {
            T[] result = new T[array1.Length + array2.Length];
            array1.CopyTo(result, 0);
            array2.CopyTo(result, array1.Length);
            return result;
        }

        //++++++++++++++++++++     编解码     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //++++++++++++++++++++     类型转换     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 字符串转字节(UTF8).
        /// </summary>
        public static byte[] StringTurnBytes(string varStr) { return Encoding.UTF8.GetBytes(varStr); }

        /// <summary>
        /// 字节转字符串.
        /// </summary>
        public static string BytesTurnString(byte[] varBytes) { return Encoding.Default.GetString(varBytes); }

        /// <summary>
        /// byte[2]转float[1].
        /// </summary>
        public static float[] BytesTurnFloat(byte[] varBytes)
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
        public static byte[] FloatTurnBytes(float[] varFloat)
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
        public static Dictionary<string, string> JsonTurnDict(string varStr)
        {
            Dictionary<string, string> tempDict = new Dictionary<string, string>();

            JsonData tempJsonData = JsonMapper.ToObject(varStr);

            foreach (KeyValuePair<string, JsonData> v in tempJsonData.Inst_Object)
            {
                if (v.Value.IsArray)
                    tempDict.Add(v.Key.DecodeUTF8(), v.Value.ToJson().DecodeUTF8());
                else
                    tempDict.Add(v.Key.DecodeUTF8(), v.Value.ToString().DecodeUTF8());
            }

            return tempDict;
        }

        /// <summary>
        /// Json转list.
        /// </summary>
        public static List<string> JsonTurnList(string varStr)
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
        public static JsonData JsonEncode(JsonData varData)
        {
            JsonData tempJsonData = new JsonData();
            if (varData.IsObject)
            {
                foreach (KeyValuePair<string, JsonData> v in varData.Inst_Object)
                {
                    if (v.Value.IsString)
                        tempJsonData[v.Key.EncodeUTF8()] = v.Value.ToString().EncodeUTF8();
                    else
                        tempJsonData[v.Key.EncodeUTF8()] = JsonEncode(v.Value);
                }
                return tempJsonData;
            }
            else if (varData.IsArray)
            {
                foreach (JsonData v in varData)
                {
                    if (v.IsString)
                        tempJsonData[tempJsonData.Count] = varData.ToString().EncodeUTF8();
                    else
                        tempJsonData[tempJsonData.Count] = JsonEncode(v);
                }
                return tempJsonData;
            }
            return new JsonData();
        }

        //++++++++++++++++++++     获取组件     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //++++++++++++++++++++     获取时间     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 获取当前时间
        /// </summary>
        public static string GetCurrentTime(string[] varInsert)
        {
            //Debug.Log(Data.GetCurrentTime(new string[] { "", "", "_", "", "" }));
            return System.DateTime.Now.ToString(string.Format("yyyy{0}MM{1}dd{2}HH{3}mm{4}ss", varInsert));
        }
    }
}