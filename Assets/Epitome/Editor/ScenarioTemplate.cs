/* $Header:   Assets/Epitome/Editor/ScenarioTemplate.cs   1.0   2020/06/18 Thursday PM 05:36:51   Ji Qiu .Yang   2017.1.1f1  $ */
/***********************************************************************************************
 ***              C O N F I D E N T I A L  ---  E P I T O M E  S T U D I O S                 ***
 ***********************************************************************************************
 *                                                                                             *
 *                 Project Name : Epitome                                                      *
 *                                                                                             *
 *                    File Name : ScenarioTemplate.cs                                          *
 *                                                                                             *
 *                   Programmer : Ji Qiu .Yang                                                 *
 *                                                                                             *
 *                   Start Date : 2020/06/18                                                   *
 *                                                                                             *
 *                  Last Update : 2020/06/18                                                   *
 *                                                                                             *
 *---------------------------------------------------------------------------------------------*
 * Functions:                                                                                  *
 * - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Epitome
{
    /**********************************************************************************************
    **  C# script template class. Customizable template style. When creating a new C# script, the 
    **  original content is overwritten by a custom template.
    */
    public class ScenarioTemplate : UnityEditor.AssetModificationProcessor
    {
        private static readonly string copyright = "E P I T O M E  S T U D I O S";

        private static readonly string authorName = "Ji Qiu .Yang";

        private static readonly string nameSpace = "Epitome";

        private static readonly string headerCode =
            "/* $Header:   #FILEPATH#   #VERSION#   #CREATETIME#   #CREATORNAME#   #UnityVersion#  $ */\r\n";

        private static readonly string authorCode =
              "/***********************************************************************************************\r\n"
            + " ***              C O N F I D E N T I A L  ---  #COPYRIGHT#***\r\n"
            + " ***********************************************************************************************\r\n"
            + " *                                                                                             *\r\n"
            + " *                 Project Name : #PROJECTNAME#*\r\n"
            + " *                                                                                             *\r\n"
            + " *                    File Name : #FILENAME#*\r\n"
            + " *                                                                                             *\r\n"
            + " *                   Programmer : #AUTHORNAME#*\r\n"
            + " *                                                                                             *\r\n"
            + " *                   Start Date : #CREATEDATE#*\r\n"
            + " *                                                                                             *\r\n"
            + " *                  Last Update : #CREATEDATE#*\r\n"
            + " *                                                                                             *\r\n"
            + " *---------------------------------------------------------------------------------------------*\r\n"
            + " * Functions:                                                                                  *\r\n"
            + " * - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */\r\n";

        public static readonly string usingCode =
              "using UnityEngine;\r\n"
            + "using System.Collections;\r\n"
            + "using System.Collections.Generic;\r\n"
            + "\r\n";

        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");
            if (path.EndsWith(".cs"))
            {
                string allText = "";
                allText += File.ReadAllText(path);
                string scriptName = GetClassName(allText);
                if (scriptName != "")
                {
                    CreateClass(path, scriptName);
                    AssetDatabase.Refresh();
                }
            }
        }

        //创建新的类 
        public static void CreateClass(string path, string className)
        {
            var sb = new ScriptBuilder();
            if (nameSpace != null)
            {
                sb.WriteLine("namespace #NAMESPACE#");
                sb.WriteCurlyBrackets();
                sb.Indent++;
            }

            sb.WriteLine("public class #SCRIPTNAME# : MonoBehaviour");
            sb.WriteCurlyBrackets();
            sb.Indent++;

            var allText = headerCode + authorCode + usingCode + sb.ToString();

            // 替换Header数据
            allText = allText.Replace("#FILEPATH#", path);
            allText = allText.Replace("#VERSION#", "1.0");
            allText = allText.Replace("#CREATETIME#", DateTime.Now.ToString("yyyy/MM/dd dddd tt hh:mm:ss"));
            allText = allText.Replace("#CREATORNAME#", authorName);
            allText = allText.Replace("#UnityVersion#", Application.unityVersion);
            // 替换文件注释
            allText = allText.Replace("#COPYRIGHT#", string.Format("{0,-45}", copyright));
            allText = allText.Replace("#PROJECTNAME#", string.Format("{0,-61}", PlayerSettings.productName));
            allText = allText.Replace("#FILENAME#", string.Format("{0,-61}", className + path.Substring(path.LastIndexOf("."))));
            allText = allText.Replace("#AUTHORNAME#", string.Format("{0,-61}", authorName));
            allText = allText.Replace("#CREATEDATE#", string.Format("{0,-61}", DateTime.Now.ToString("yyyy/MM/dd")));
            // 替换代码数据
            allText = allText.Replace("#SCRIPTNAME#", className);
            allText = allText.Replace("#NAMESPACE#", nameSpace);

            File.WriteAllText(path, allText, Encoding.UTF8);
        }

        //首字母改成大写
        public static string FirstLetterUppercase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            if (str.Length == 1)
                return str.ToUpper();

            var first = str[0];
            var rest = str.Substring(1);
            return first.ToString().ToUpper() + rest;
        }

        //获取unity自动创建的脚本类名
        public static string GetClassName(string allText)
        {
            var patterm = "public class ([A-Za-z0-9_]+)\\s*:\\s*MonoBehaviour {\\s*\\/\\/ Use this for initialization\\s*void Start \\(\\) {\\s*}\\s*\\/\\/ Update is called once per frame\\s*void Update \\(\\) {\\s*}\\s*}";
            var match = Regex.Match(allText, patterm);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return "";
        }
    }
}
