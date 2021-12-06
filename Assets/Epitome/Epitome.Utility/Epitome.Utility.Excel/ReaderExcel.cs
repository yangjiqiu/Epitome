using OfficeOpenXml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Epitome
{
    public static class ReaderExcel
	{
        public static void Reader(string filePath)
        {
            using (ExcelPackage package = new ExcelPackage(new FileStream(filePath, FileMode.Open)))
            {
                for (int i = 1; i <= package.Workbook.Worksheets.Count; ++i)
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets[i];
                    for (int j = sheet.Dimension.Start.Column, k = sheet.Dimension.End.Column; j <= k; j++)
                    {
                        for (int m = sheet.Dimension.Start.Row, n = sheet.Dimension.End.Row; m <= n; m++)
                        {
                            string str = sheet.GetValue(m, j).ToString();
                            if (str != null)
                            {
                                // do something
                                Debug.Log(str);
                            }
                        }
                    }
                }
            }
        }
	}
}
