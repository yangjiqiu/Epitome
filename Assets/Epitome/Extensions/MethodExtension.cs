using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using UnityEngine;

namespace Epitome
{
    public static class MethodExtension
    {
        public static void SetSpeed(this Animation self, float speed)
        {
            self[self.clip.name].speed = speed;
        }

        // 编码.
        public static string Encode(this string self, Encoding encoding) { return HttpUtility.UrlEncode(self, encoding); }

        // 解码.
        public static string Decode(this string self, Encoding encoding) { return HttpUtility.UrlDecode(self, encoding); }

        /// <summary>
        /// 编码UTF8.
        /// </summary>
        public static string EncodeUTF8(this string self) { return Encode(self, Encoding.UTF8); }

        /// <summary>
        /// 解码UTF8.
        /// </summary>
        public static string DecodeUTF8(this string self) { return Decode(self, Encoding.UTF8); }

        /// <summary>
        /// Returns the distance between a and b.
        /// </summary>
        public static float Distance(Vector2 a, Vector2 b) { return Vector2.Distance(a, b); }

        /// <summary>
        /// Returns the distance between a and b.
        /// </summary>
        public static float Distance(Vector3 a, Vector3 b) { return Vector3.Distance(a, b); }

        public static TInterface GetInterfaceComponent<TInterface>(this Component thisComponent) where TInterface : class
        {
            return thisComponent.GetComponent(typeof(TInterface)) as TInterface;
        }

        /// <summary>
        /// Vector3 to vector2
        /// </summary>
        public static Vector2 ToVector2(this Vector3 vector3) { return new Vector2(vector3.x, vector3.y); }

        /// <summary>
        /// Vector2 to vector3
        /// </summary>
        public static Vector3 ToVector3(this Vector2 vector2) { return new Vector3(vector2.x, vector2.y, 0); }

        /// <summary>
        /// Get dictionary value
        /// </summary>
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            TValue value = default(TValue);
            dict.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// Int to enum
        /// </summary>
        public static T ToEnum<T>(this int value) { return (T)Enum.ToObject(typeof(T), value); }

        /// <summary>
        /// String to enum
        /// </summary>
        public static T ToEnum<T>(this string str) { return (T)Enum.Parse(typeof(T), str); }

        /// <summary>
        /// String array To enum array
        /// </summary>
        public static T[] ToEnumArray<T>(this string[] strArray)
        {
            T[] enumArray = new T[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                enumArray[i] = strArray[i].ToEnum<T>();
            }
            return enumArray;
        }

        /// <summary>
        /// String list to enum list
        /// </summary>
        public static List<T> ToEnumList<T>(this List<string> strList)
        {
            List<T> enumList = new List<T>();

            for (int i = 0; i < strList.Count; i++)
                enumList.Add(strList[i].ToEnum<T>());

            return enumList;
        }

        /// <summary>
        /// Int array to enum array
        /// </summary>
        public static T[] ToEnumArray<T>(this int[] intArray)
        {
            T[] enumArray = new T[intArray.Length];

            for (int i = 0; i < intArray.Length; i++)
                enumArray[i] = intArray[i].ToEnum<T>();

            return enumArray;
        }

        /// <summary>
        /// Int array to string array
        /// </summary>
        public static string[] ToStringArray(this int[] intArray)
        {
            string[] strArray = new string[intArray.Length];

            for (int i = 0; i < intArray.Length; i++)
                strArray[i] = intArray[i].ToString();

            return strArray;
        }


        /// <summary>
        /// String array to int array
        /// </summary>
        public static int[] ToIntArray(this string[] strArray)
        {
            int[] intArray = new int[strArray.Length];

            for (int i = 0; i < strArray.Length; i++)
                intArray[i] = int.Parse(strArray[i]);

            return intArray;
        }

        /// <summary>
        /// String array to int array
        /// </summary>
        public static string[] ToStringArray<T>(this T[] enumArray)
        {
            string[] strArray = new string[enumArray.Length];

            for (int i = 0; i < enumArray.Length; i++)
                strArray[i] = enumArray[i].ToString();

            return strArray;
        }

        /// <summary>
        /// 数组转集合.
        /// </summary>
        public static List<T> ToList<T>(this T[] self)
        {
            List<T> list = new List<T>();

            for (int i = 0; i < self.Length; i++)
                list.Add(self[i]);

            return list;
        }

        // ==================
        public static Vector2 ToVector2XY(this Vector3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }

        //public static Tvalue GetValue<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> varDict, Tkey varKey)
        //{
        //    Tvalue tempValue = default(Tvalue);
        //    varDict.TryGetValue(varKey, out tempValue);
        //    return tempValue;
        //}

        //public static T StringToEnum<T>(this string str)
        //{
        //    return (T)Enum.Parse(typeof(T), str);
        //}

        //public static T[] StringsToEnums<T>(this string[] strings)
        //{
        //    T[] EnumTypes = new T[strings.Length];
        //    for (int i = 0; i < strings.Length; i++)
        //    {
        //        EnumTypes[i] = strings[i].StringToEnum<T>();

        //    }
        //    return EnumTypes;
        //}

        //public static List<T> StringToEnumList<T>(this List<string> strings)
        //{
        //    List<T> EnumTypes = new List<T>();
        //    for (int i = 0; i < strings.Count; i++)
        //    {
        //        EnumTypes.Add(strings[i].StringToEnum<T>());
        //    }
        //    return EnumTypes;
        //}

        //public static T IntToEnum<T>(this int Num)
        //{
        //    return (T)Enum.ToObject(typeof(T), Num);
        //}

        //public static T[] ToEnums<T>(this int[] Nums)
        //{
        //    T[] EnumTypes = new T[Nums.Length];
        //    for (int i = 0; i < Nums.Length; i++)
        //    {
        //        EnumTypes[i] = IntToEnum<T>(Nums[i]);
        //    }
        //    return EnumTypes;
        //}
    }
}