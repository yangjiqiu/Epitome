using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ParsingLyricsLRC : MonoBehaviour
{
    public string[] SortLyricArrayAndTimeArray(string[] lyricA, float[] timeA, out float[] timeArray)
    {
        for (int i = 0; i < timeA.Length - 1; i++)
        {
            for (int j = 0; j < timeA.Length - 1 - i; j++)
            {
                if (timeA[j] > timeA[j + 1])
                {
                    float temp = timeA[j];
                    timeA[j] = timeA[j + 1];
                    timeA[j + 1] = temp;

                    string tempLyric = lyricA[j];
                    lyricA[j] = lyricA[j + 1];
                    lyricA[j + 1] = tempLyric;
                }
            }
        }
        timeArray = timeA;
        return lyricA;
    }


    //获取时间数组和 歌词数组
    public string[] GetLyricArrayAndTimeArray(string lyricText, int length, int titleLength, out float[] timeA, out string[] titleA)
    {
        timeA = new float[length];
        string[] lyricArray = new string[length];
        titleA = new string[titleLength];
        int lyricIndex = 0;
        int titleIndex = 0;
        string[] lineArray = lyricText.Split('\n');//根据分隔出行

        for (int i = 0; i < lineArray.Length; i++)
        {
            string lineStr = lineArray[i];
            if (lineStr.Contains("ti") || lineStr.Contains("ar") || lineStr.Contains("al") || lineStr.Contains("by") || lineStr.Contains("offset"))
            {//标题
                string[] array = lineStr.Split('[', ':', ']');
                float f;
                if (!float.TryParse(array[array.Length - 2], out f) && array[array.Length - 2] != null)
                {
                    titleA[titleIndex] = array[array.Length - 2];
                    titleIndex++;
                }
            }
            else
            {//歌词
                string[] contentArray = lineStr.Split('[', ']');
                for (int j = contentArray.Length - 1; j >= 0; j--)
                {
                    string subStr = contentArray[j];
                    string newSubStr = subStr.Replace(":", "");
                    float temp;
                    if (float.TryParse(newSubStr, out temp))
                    {
                        string[] time = subStr.Split(':');
                        float min;
                        float.TryParse(time[0], out min);
                        float sec;
                        float.TryParse(time[1], out sec);
                        subStr = string.Format("{0}", (sec + 60 * min));
                    }

                    float num = 0f;
                    if (float.TryParse(subStr, out num))
                    {
                        timeA[lyricIndex] = num;
                        if (float.TryParse(contentArray[contentArray.Length - 1], out num))
                        {
                            lyricArray[lyricIndex] = "空测试";
                        }
                        else
                        {
                            lyricArray[lyricIndex] = contentArray[contentArray.Length - 1];
                        }
                        lyricIndex++;
                    }
                }
            }
        }

        return lyricArray;
    }


    //获取所有时间数组的长度 和标题的长度
    public int GetLyricArrayLength(string lyricText, out int titleLenght)
    {
        int index = 0;
        int titleL = 0;
        string[] lineArray = lyricText.Split('\n');//根据空格分隔出行
        foreach (string lineStr in lineArray)
        {
            if (lineStr.Contains("ti") || lineStr.Contains("ar") || lineStr.Contains("al") || lineStr.Contains("by") || lineStr.Contains("offset"))
            {//标题
                titleL++;
            }
            else
            {//歌词
                string[] contentArray = lineStr.Split('[', ']');
                foreach (string subStr in contentArray)
                {
                    string newStr = subStr.Replace(":", "");
                    float i = 0f;
                    if (float.TryParse(newStr, out i))
                    {
                        index++;
                    }
                }
            }
        }
        titleLenght = titleL;
        return index;
    }



    //2 修改使用list=================================================================
    //获取时间数组和 歌词数组
    public List<string> GetLyricListAndTimeList(string lyricText, out List<float> timeA, out List<string> titleA)
    {
        List<string> lyricArray = new List<string>();
        timeA = new List<float>();
        titleA = new List<string>();

        string[] lineArray = lyricText.Split('\n');//根据分隔出行

        for (int i = 0; i < lineArray.Length; i++)
        {
            string lineStr = lineArray[i];
            if (lineStr.Contains("ti") || lineStr.Contains("ar") || lineStr.Contains("al") || lineStr.Contains("by") || lineStr.Contains("offset"))
            {//标题
                string[] array = lineStr.Split('[', ':', ']');
                float f;
                if (!float.TryParse(array[array.Length - 2], out f) && array[array.Length - 2] != null)
                {
                    titleA.Add(array[array.Length - 2]);
                }
            }
            else
            {//歌词
                string[] contentArray = lineStr.Split('[', ']');
                for (int j = contentArray.Length - 1; j >= 0; j--)
                {
                    string subStr = contentArray[j];
                    string newSubStr = subStr.Replace(":", "");
                    float temp;
                    if (float.TryParse(newSubStr, out temp))
                    {
                        string[] time = subStr.Split(':');
                        float min;
                        float.TryParse(time[0], out min);
                        float sec;
                        float.TryParse(time[1], out sec);
                        subStr = string.Format("{0}", (sec + 60 * min));
                    }
                    float num = 0f;
                    if (float.TryParse(subStr, out num))
                    {
                        timeA.Add(num);
                        if (float.TryParse(contentArray[contentArray.Length - 1], out num))
                        {
                            lyricArray.Add("");
                        }
                        else
                        {
                            lyricArray.Add(contentArray[contentArray.Length - 1]);
                        }
                    }
                }
            }
        }

        return lyricArray;
    }

    public List<string> SortLyricListAndTimeList(List<string> lyricA, List<float> timeA, out List<float> timeArray)
    {
        for (int i = 0; i < timeA.Count - 1; i++)
        {
            for (int j = 0; j < timeA.Count - 1 - i; j++)
            {
                if (timeA[j] > timeA[j + 1])
                {
                    float temp = timeA[j];
                    timeA[j] = timeA[j + 1];
                    timeA[j + 1] = temp;

                    string tempLyric = lyricA[j];
                    lyricA[j] = lyricA[j + 1];
                    lyricA[j + 1] = tempLyric;
                }
            }
        }
        timeArray = timeA;
        return lyricA;
    }


    //读取lrc歌词文件
    public void GetLrcFile(string file)
    {
        StreamReader sr = new StreamReader(file, Encoding.Default);
        string str = sr.ReadToEnd();
        print(str);

    }
}
