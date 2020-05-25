using Epitome.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Epitome
{
    public static class ProjectPath
    {
        public static string GetDataPath { get { return Application.dataPath; } }
        public static string GetPersistent { get { return Application.persistentDataPath; } }
        public static string GetStreamingAssets { get { return Application.streamingAssetsPath; } }
        public static string GetTemporaryCache { get { return Application.temporaryCachePath; } }

        public static string[] AllFilePaths(string path)
        {
            string[] filePaths = new string[] { };
            if (Project.FileExists(path))
            {
                DirectoryInfo direInfo = new DirectoryInfo(path);
                FileInfo[] files = direInfo.GetFiles("*", SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta")) continue;
                    filePaths[filePaths.Length] = files[i].DirectoryName;
                }
                return filePaths;
            }
            return null;
        }

        public static string[] AllFilePaths(string path, string suffixs)
        {
            return Directory.GetFiles(path, "*." + suffixs);
        }

        public static string[] AllFilePaths(string path, string[] suffixs)
        {
            string[] filePaths = new string[] { };
            if (Project.FileExists(path))
            {
                DirectoryInfo direInfo = new DirectoryInfo(path);
                FileInfo[] files = direInfo.GetFiles("*", SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta")) continue;
                    string[] fileName = files[i].Name.Split('.');
                    if (suffixs.Contains(fileName[fileName.Length - 1]))
                        filePaths[filePaths.Length] = files[i].DirectoryName;
                }
                return filePaths;
            }
            return null;
        }

        public static string[] AllIamgePaths(string path)
        {
            string[] filePaths = new string[] { };
            string type = "BMP|JPG|GIF|PNG";
            string[] imageType = type.Split('|');
            for (int i = 0; i < imageType.Length; i++)
            {
                string[] dirs = AllFilePaths(path, imageType[i]);
                for (int j = 0; j < dirs.Length; j++)
                    filePaths[filePaths.Length] = dirs[j];
            }
            return filePaths;
        }

        public static string RelativePath(MonoBehaviour script) 
        {
            string path = "";
#if UNITY_EDITOR
            MonoScript monoScript = MonoScript.FromMonoBehaviour(script);
            path = AssetDatabase.GetAssetPath(monoScript);
#endif
            return path;
        }

        public static string RelativePath(ScriptableObject script)
        {
            string path = "";
#if UNITY_EDITOR
            MonoScript monoScript = MonoScript.FromScriptableObject(script); //更新 使用UnityEditor API 
            path = AssetDatabase.GetAssetPath(monoScript);
#endif
            return path;
        }

        public static string RelativePath(Type script)
        {
            string path = "";
#if UNITY_EDITOR
            path = AssetDatabase.FindAssets("t:Script")
                .Where(v => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(v)) == script.Name)
                .Select(id => AssetDatabase.GUIDToAssetPath(id))
                .FirstOrDefault()
                .ToString();
#endif
            return path;
        }

        //public static string ParentDirectory(Type script)
        //{
        //    //return Path.;
        //}

        public static string FullParentDirectory(Type script)
        {
            return FullParentDirectory(RelativePath(script));
        }

        public static string FullParentDirectory(string path)
        {
            return Directory.GetParent(path).FullName.Replace('\\', '/');
        }

        public static string FileDirectory(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public static string DirectoryName(string path)
        {
            return Path.GetDirectoryName(path).Replace('\\', '/');
        }

        public static string CurrentDirectory { get { return Directory.GetCurrentDirectory(); } }

        public static string BaseDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }


        //public static string CurrentDirectory()
        //{
        //    return Environment.CurrentDirectory;
        //}

        //public static string ParentDirectory()
        //{

        //}

        //        // 获取程序的基目录。
        //        System.AppDomain.CurrentDomain.BaseDirectory

        //        // 获取模块的完整路径。
        //        System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName

        //        // 获取和设置当前目录(该进程从中启动的目录)的完全限定目录。
        //        System.Environment.CurrentDirectory

        //        // 获取应用程序的当前工作目录。
        //        System.IO.Directory.GetCurrentDirectory()

        //        // 获取和设置包括该应用程序的目录的名称。
        //        System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase

        //        // 获取启动了应用程序的可执行文件的路径。
        //        System.Windows.Forms.Application.StartupPath

        //        // 获取启动了应用程序的可执行文件的路径及文件名
        //        System.Windows.Forms.Application.ExecutablePath
    }
}
