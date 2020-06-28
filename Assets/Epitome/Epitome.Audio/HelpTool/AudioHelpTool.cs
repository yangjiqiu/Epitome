using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Epitome.Utility;
using System.IO;
using Epitome;
using System.Threading;
using System;
using OfficeOpenXml;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Epitome.Audio
{
    public static class AudioHelpTool
    {
#if UNITY_EDITOR
        /// <summary>
        /// MP3文件转WAV文件
        /// </summary>
        [MenuItem("Epitome/Audio/MP3TurnWAV")]
        [MenuItem("Assets/Epitome/Audio/MP3TurnWAV")]
        private static void MP3TurnWAV()
        {
            List<string> paths = EditorExtension.GetSelectionPath();

            string rootPath = ProjectPath.GetDataPath.Substring(0, ProjectPath.GetDataPath.Length - 6);

            for (int i = 0; i < paths.Count; i++)
            {
                if (Directory.Exists(paths[i]))
                {
                    DirectoryInfo info = new DirectoryInfo(rootPath + paths[i]);

                    string savePath = info.FullName + "_WAV";

                    Project.CreateDirectory(savePath);

                    Loom.RunAsync(() =>
                    {
                        List<string> strs = Project.DirectoryAllFileNames(info.FullName, new List<string>() { "mp3" });

                        AudioConverter.MP3TurnWAV(info.FullName, savePath, strs);
                    });
                }
                else if (File.Exists(paths[i]))
                {
                    FileInfo info = new FileInfo(rootPath + paths[i]);
                    string[] fileName = info.Name.Split('.');

                    if (fileName[fileName.Length - 1] == "mp3")
                    {
                        string savePath = string.Format("{0}/{1}.wav", info.Directory.FullName, fileName[0]);

                        AudioConverter.MP3TurnWAV(info.FullName, savePath);
                        AssetDatabase.Refresh();
                    }
                }
            }
        }

        private static Task ReadAudio;

        /// <summary>
        /// 生成音频表格
        /// </summary>
        [MenuItem("Epitome/Audio/GenerateAudioTable")]
        [MenuItem("Assets/Epitome/Audio/GenerateAudioTable")]
        private static void GenerateAudioTable()
        {
            List<string> paths = EditorExtension.GetSelectionPath();

            string rootPath = ProjectPath.GetDataPath.Substring(0, ProjectPath.GetDataPath.Length - 6);


            if (paths.Count > 0 && Directory.Exists(paths[0]))
            {
                DirectoryInfo info = new DirectoryInfo(rootPath + paths[0]);

                List<string> strs = Project.DirectoryAllFileFullName(info.FullName, new List<string>() { "wav", "aif", "ogg" });
                Debug.Log(strs.Count);
                if (ReadAudio != null)
                {
                    ReadAudio.Stop();
                    ReadAudio = null;
                }
                ReadAudio = new Task(AudioLoad.Load(strs.ToArray(), AudioProcessing));
            }
        }

        private static void AudioProcessing(List<AudioClip> clips, List<string> audioPaths)
        {
            List<string> header = new List<string>();
            header.Add("MusicName");
            header.Add("Duration");
            header.Add("Path");

            List<List<string>> tableData = new List<List<string>>();
            for (int i = 0; i < clips.Count; i++)
            {
                int minute = (int)clips[i].length / 60;
                int seconds = (int)clips[i].length % 60;

                List<string> rowData = new List<string>();
                rowData.Add(Path.GetFileNameWithoutExtension(audioPaths[i]));
                rowData.Add(string.Format("{0:D2}:{1:D2}", minute, seconds));

                string path = audioPaths[i].Substring(audioPaths[i].IndexOf("Assets"));
                path = path.Replace('\\', '/');

                rowData.Add(path);

                tableData.Add(rowData);
            }

            FileInfo info = new FileInfo(audioPaths[0]);

            string filePath = info.Directory.Parent.FullName + "/" + info.Directory.Name + ".xlsx";

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    WriterExcel.NewWorkSheet(package, "Music", header, tableData);
                }
            }

            if (ReadAudio != null)
            {
                ReadAudio.Stop();
                ReadAudio = null;
            }
        }
#endif
    }
}