using OfficeOpenXml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
# endif

namespace Epitome
{
    public static class ActionExcel
    {
        public static ExcelPackage OpenExcel(string filePath)
        {
            return new ExcelPackage(new FileStream(filePath, FileMode.Open));
        }

        public static List<string> SheetNames(ExcelPackage package)
        {
            List<string> sheetNames = new List<string>();

            ExcelWorkbook Wworkbook = package.Workbook;

            foreach (var worksheet in Wworkbook.Worksheets)
            {
                sheetNames.Add(worksheet.Name);
            }
            return sheetNames;
        }


        public static StreamWriter Create(string outPutPath)
        {
            StreamWriter stream = null;

            if (!File.Exists(outPutPath))
            {
                stream = File.CreateText(outPutPath);
#if UNITY_EDITOR
                AssetDatabase.Refresh();
# endif
            }
            return stream;
        }

    }
}
