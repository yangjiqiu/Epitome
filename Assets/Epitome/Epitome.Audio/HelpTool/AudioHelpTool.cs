using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Epitome.Utility;
using System.IO;
using Epitome;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Epitome
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
            for (int i = 0; i < paths.Count; i++)
            {
                if (Directory.Exists(paths[i]))
                {
                    string rootPath = ProjectPath.GetDataPath.Substring(0, ProjectPath.GetDataPath.Length - 6);

                    DirectoryInfo info = new DirectoryInfo(rootPath + paths[i]);

                    string savePath = info.FullName + "_WAV";

                    Project.CreateDirectory(savePath);

                    List<string> strs = Project.DirectoryAllFileNames(info.FullName, new List<string>() { "mp3" });

                    AudioConverter.MP3TurnWAV(info.FullName, savePath, strs);
                }
            }
        }

        /// <summary>
        /// 生成音频表格
        /// </summary>
        private static void GenerateAudioTable()
        {
            
        }
#endif
    }
}