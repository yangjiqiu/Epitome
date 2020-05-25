/* =========================
 * 描 述：关键字近似搜索
 * 作 者：杨继求
 * 创建时间：2019/03/25 15:59:37
 * ========================= */
using System;
using System.Linq;

public static class SearchHelper
{
    public static string[] Search(string param, string[] datas)
    {
        if (string.IsNullOrEmpty(param))
            return new string[0];

        string[] words = param.Split(new char[] { ' ', '　' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string word in words)
        {
            int maxDist = (word.Length - 1) / 2;

            var q = from str in datas
                    where word.Length <= str.Length
                        && Enumerable.Range(0, maxDist + 1)
                        .Any(dist =>
                        {
                            return Enumerable.Range(0, Math.Max(str.Length - word.Length - dist + 1, 0))
                                .Any(f =>
                                {
                                    return Distance(word, str.Substring(f, word.Length + dist)) <= maxDist;
                                });
                        })
                    orderby str
                    select str;
            datas = q.ToArray();
        }

        return datas;
    }

    static int Distance(string str1, string str2)
    {
        int n = str1.Length;
        int m = str2.Length;
        int[,] C = new int[n + 1, m + 1];
        int i, j, x, y, z;
        for (i = 0; i <= n; i++)
            C[i, 0] = i;
        for (i = 1; i <= m; i++)
            C[0, i] = i;
        for (i = 0; i < n; i++)
            for (j = 0; j < m; j++)
            {
                x = C[i, j + 1] + 1;
                y = C[i + 1, j] + 1;
                if (str1[i] == str2[j])
                    z = C[i, j];
                else
                    z = C[i, j] + 1;
                C[i + 1, j + 1] = Math.Min(Math.Min(x, y), z);
            }
        return C[n, m];
    }
}
