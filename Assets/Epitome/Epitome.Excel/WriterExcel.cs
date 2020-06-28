using OfficeOpenXml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Epitome
{
    public static class WriterExcel
    {
        public static void Writer(string filePath)
        {
            Writer(new FileStream(filePath, FileMode.Open));
        }

        public static void Writer(FileStream fileStream)
        {
            Writer(new ExcelPackage(fileStream));
        }

        public static void Writer(ExcelPackage package)
        {
            using (package)
            {
                //ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("我的Excel");
                //worksheet.Cells[1, 1].Value = "序号";
                //worksheet.Cells[1, 2].Value = "姓名";
                //worksheet.Cells[1, 3].Value = "电话";
                //package.Save();
            }
        }

        public static void NewWorkSheet(ExcelPackage package, string sheetName,List<string> header,List<List<string>> tableData = null)
        {
            if (!ActionExcel.SheetNames(package).Contains(sheetName))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);

                for (int i = 0; i < header.Count; i++)
                {
                    worksheet.SetValue(1, i + 1, header[i]);
                }
                if (tableData != null)
                {
                    for (int i = 0; i < tableData.Count ; i++)
                    {
                        List<string> rowData = tableData[i];

                        for (int j = 0; j < rowData.Count; j++)
                        {
                            worksheet.SetValue(i + 2, j + 1, rowData[j]);
                        }
                    }
                }

                package.Save();
            }
        }
    }
}
