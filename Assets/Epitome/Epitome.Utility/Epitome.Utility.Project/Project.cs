using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;//读写文件和数据流的类型
using System;

namespace Epitome.Utility
{
    using Manager;
    using System.Collections.Generic;

    public static class Project
    {
        public static void CreateFile(string path)
        {
            if (!FileExists(path))
            {
                File.CreateText(path);
#if UNITY_EDITOR
                AssetDatabase.Refresh();
# endif
            }
        }
            
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
#if UNITY_EDITOR
                AssetDatabase.Refresh();
# endif
            }
        }
        
        public static void CreateMultipleDirectories(string path, string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                CreateDirectory(string.Format("{0}/{1}", path, names[i]));
            }
        }

        //=================检索资源======================

        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public static List<string> DirectoryAllFileNames(string path)
        {
            if (DirectoryExists(path))
            {
                List<string> strs = new List<string>();
                DirectoryInfo direInfo = new DirectoryInfo(path);
                FileInfo[] files = direInfo.GetFiles("*", SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta")) continue;
                    strs.Add(files[i].Name);
                }
                return strs;
            }
            return null;
        }

        public static List<string> DirectoryAllFileNames(string path, List<string> suffixs)
        {
            if (DirectoryExists(path))
            {
                List<string> strs = new List<string>();
                DirectoryInfo direInfo = new DirectoryInfo(path);
                FileInfo[] files = direInfo.GetFiles("*", SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta")) continue;
                    string[] fileName = files[i].Name.Split('.');
                    if (suffixs.Contains(fileName[fileName.Length - 1]))
                        strs.Add(files[i].Name);
                }
                return strs;
            }
            return null;
        }

        public static List<string> DirectoryAllFileFullName(string path, List<string> suffixs)
        {
            if (DirectoryExists(path))
            {
                List<string> strs = new List<string>();
                DirectoryInfo direInfo = new DirectoryInfo(path);
                FileInfo[] files = direInfo.GetFiles("*", SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta")) continue;
                    string[] fileName = files[i].Name.Split('.');
                    if (suffixs.Contains(fileName[fileName.Length - 1]))
                        strs.Add(files[i].FullName);
                }
                return strs;
            }
            return null;
        }

        public static FileInfo[] DirectoryAllFileInfo(string path, List<string> suffixs)
        {
            List<FileInfo> fileInfos = new List<FileInfo>();
            if (DirectoryExists(path))
            {
                DirectoryInfo direInfo = new DirectoryInfo(path);
                FileInfo[] files = direInfo.GetFiles("*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta")) continue;
                    string[] fileName = files[i].Name.Split('.');
                    if (suffixs.Contains(fileName[fileName.Length - 1]))
                        fileInfos.Add(files[i]);
                }
                return fileInfos.ToArray();
            }
            return null;
        }

        public static ArrayList ReadFile(string path, string name)
        {
            StreamReader SR = null;
            try
            {
                SR = File.OpenText(path + "//" + name);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
            string line;
            ArrayList arrlist = new ArrayList();
            while ((line = SR.ReadLine()) != null)
            {
                arrlist.Add(line);
            }
            //关闭流
            SR.Close();
            //销毁流
            SR.Dispose();
            //将数组链表容器返回
            return arrlist;
        }

        public static IEnumerator SaveFile(byte[] byteFile, string path)
        {
            DeleteFiles(path);

            FileStream fileStream = new FileStream(path, FileMode.CreateNew);

            BinaryWriter binaryWriter = new BinaryWriter(fileStream);

            binaryWriter.Write(byteFile);

            binaryWriter.Close();
            fileStream.Close();

            yield return null;

#if UNITY_EDITOR
            AssetDatabase.Refresh(); //刷新
#endif
        }


        public static void SaveImage(Texture2D Image, string path, string name)
        {
            byte[] bytes = Image.EncodeToPNG();
            Project.CreateDirectory(path);
            File.WriteAllBytes(string.Format("{0}/{1}/{2}", ProjectPath.GetPersistent, path, name), bytes);
        }

        public static void DeleteFiles(string path)
        {
            FileInfo file = new FileInfo(path);
            if (file != null)
                DeleteFiles(file);
        }

        public static void DeleteFiles(FileInfo file)
        {
            if (file != null)
                file.Delete();
        }

        public static void DeleteDirectory(string path)
        {
            DirectoryInfo dire = new DirectoryInfo(path);
            if (dire != null)
                DeleteDirectory(dire);
        }

        public static void DeleteDirectory(DirectoryInfo dire)
        {
            if (dire == null || (!dire.Exists))
                return;

            DirectoryInfo[] dires = dire.GetDirectories();
            if (dires != null)
            {
                for (int i = 0; i < dires.Length; i++)
                    DeleteDirectory(dires[i]);
                dires = null;
            }

            FileInfo[] files = dire.GetFiles();
            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                    DeleteFiles(files[i]);
                files = null;
            }

            dire.Delete();
        }

        public static void LoadScene(string name)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(name);
        }

        public static void LoadScene(int index)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(index);
        }
    }
}
