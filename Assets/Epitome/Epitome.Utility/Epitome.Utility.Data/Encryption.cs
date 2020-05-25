/*----------------------------------------------------------------
 * 文件名：Encryption
 * 文件功能描述：加密
----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encryption
{
    static Encryption mInstance;

    public static Encryption GetSingleton() { if (mInstance == null) { mInstance = new Encryption(); } return mInstance; }

    //++++++++++++++++++++     数字加密     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    Dictionary<int, List<string>> mDigitalLetter;

    /// <summary>
    /// 数字转字母
    /// </summary>
    public string DigitalLetter(int varInt)
    {
        if (mDigitalLetter == null)
            DigitalLetterChange();

        List<string> tempList;

        mDigitalLetter.TryGetValue(varInt, out tempList);

        return tempList[Random.Range(0, tempList.Count)];
    }

    //++++++++++++++++++++     数字解密     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    /// <summary>
    /// 字母解数字
    /// </summary>
    public int LetterDecryption(string varStr)
    {
        if (mDigitalLetter == null)
            DigitalLetterChange();

        foreach (KeyValuePair<int, List<string>> v in mDigitalLetter)
        {
            foreach (string vv in v.Value)
            {
                if(vv == varStr)
                    return v.Key;
            }
        }

        return -1;
    }

    //++++++++++++++++++++     密码本     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //字母替换数字
    void DigitalLetterChange()
    {
        mDigitalLetter = new Dictionary<int, List<string>>();

        Dictionary<char, int> mDictX = new Dictionary<char, int>();
        mDictX.Add('A', 0); mDictX.Add('B', 1); mDictX.Add('C', 2);
        mDictX.Add('D', 3); mDictX.Add('E', 4); mDictX.Add('F', 5);
        mDictX.Add('G', 6); mDictX.Add('H', 7); mDictX.Add('I', 8);
        mDictX.Add('J', 9);

        Dictionary<char, int> mDictY = new Dictionary<char, int>();
        mDictY.Add('K', 0); mDictY.Add('L', 1); mDictY.Add('M', 2);
        mDictY.Add('N', 3); mDictY.Add('O', 4); mDictY.Add('P', 5);
        mDictY.Add('Q', 6); mDictY.Add('R', 7); mDictY.Add('S', 8);
        mDictY.Add('T', 9);

        foreach (KeyValuePair<char, int> v in mDictX)
        {
            foreach (KeyValuePair<char, int> vv in mDictY)
            {
                int tempInt = v.Value + vv.Value;

                if (tempInt > 9)
                    tempInt = int.Parse(tempInt.ToString().Substring(1));

                List<string> tempStr;

                mDigitalLetter.TryGetValue(tempInt, out tempStr);

                if (tempStr == null)
                    tempStr = new List<string>();

                tempStr.Add(v.Key.ToString() + vv.Key.ToString());

                mDigitalLetter[tempInt] = tempStr;
            }
        }
    }
}
