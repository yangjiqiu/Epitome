using OfficeOpenXml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Epitome.Utility.Excle
{
    public class Excle
    {
        //public void CreateExcel(string path,)
        //{
        //    string outPutDir = Application.dataPath + "\\" + "MyExcel.xls";
        //    FileInfo newFile = new FileInfo(outPutDir);
        //    if (newFile.Exists)
        //    {
        //        newFile.Delete();  // ensures we create a new workbook   
        //        Debug.Log("删除表");
        //        newFile = new FileInfo(outPutDir);
        //    }
        //    using (ExcelPackage package = new ExcelPackage(newFile))
        //    {
        //        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("我的Excel");
        //        worksheet.Cells[1, 1].Value = "序号";
        //        worksheet.Cells[1, 2].Value = "姓名";
        //        worksheet.Cells[1, 3].Value = "电话";
        //        package.Save();
        //        Debug.Log("导出Excel成功");
        //    }
        //}
    }
}

