using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Epitome.LogSystem
{
    public class FileAppender : AsyncTask<FileAppender>, ILogAppender
    {
        protected const string TIME_FORMATER = "yyyy.MM.dd HH:mm:ss,fff";

        string logRootPath;

        // 10M
        private int maxFileSize = 10 * 1024 * 1024;

        private StreamWriter streamWriter;

        private FileStream fileStream;

        private string logFilePath;
        protected virtual string LogFilePath
        {
            get { return logFilePath; }
            set { logFilePath = value; }
        }

        private List<LogData> writeList;

        private List<LogData> waitList;

        private object lockObj;

        private bool stopFlag;

        private int fileCount;

        private void Awake()
        {
#if UNITY_EDITOR
            logRootPath = Application.dataPath + "/Log";
#elif UNITY_STANDALONE_WIN
            logRootPath = Application.dataPath + "/Log";
#elif UNITY_STANDALONE_OSX
            logRootPath = Application.dataPath + "/Log";
#else
            logRootPath = Application.persistentDataPath + "/Log";
#endif
            fileCount = 0;

            LogFilePath = Path.Combine(logRootPath, string.Format("{0}_{1}.log", DateTime.Now.ToString("yyyyMMdd"), fileCount));

            if (File.Exists(LogFilePath))
            {
                fileStream = new FileStream(LogFilePath, FileMode.Append);
            }
            else
            {
                if (!Directory.Exists(logRootPath))
                    Directory.CreateDirectory(logRootPath);
                fileStream = new FileStream(LogFilePath, FileMode.Create);
            }
            streamWriter = new StreamWriter(fileStream);
            streamWriter.AutoFlush = true;

            writeList = new List<LogData>();
            waitList = new List<LogData>();
            lockObj = new object();
            stopFlag = false;
        }

        public void Log(LogData logData)
        {
            lock (lockObj)
            {
                waitList.Add(logData);
                Monitor.PulseAll(lockObj);
            }
        }

        public override void Close()
        {
            this.stopFlag = true;
            if (null != this.fileStream)
            {
                this.fileStream.Close();
            }
        }

        public override void Run()
        {
            Loom.RunAsync(() => {
                while (!stopFlag)
                {
                    lock (lockObj)
                    {
                        if (waitList.Count == 0)
                        {
                            Monitor.Wait(lockObj);
                        }
                        this.writeList.AddRange(this.waitList);
                        this.waitList.Clear();
                    }
                    for (int i = 0; i < writeList.Count; i++)
                    {
                        LogData logData = writeList[i];

                        Loom.QueueOnMainThread((object sd) =>
                        {
                            this.streamWriter.WriteLine(String.Format("[{0}] {1,-5}:{2}\r\n{3}", logData.logTime.ToString(TIME_FORMATER), logData.logLevel, logData.logMessage, logData.logBasicData));

                            if (logData.logTrack != null)
                            {
                                this.streamWriter.WriteLine(logData.logTrack + "\n");
                            }
                        }, 2);

                        HandleTriggerEvent();
                    }
                }
            });
        }

        private void HandleTriggerEvent()
        {
            if (this.fileStream.Length >= maxFileSize)
            {
                fileCount += 1;
                LogFilePath = Path.Combine(logRootPath, string.Format("{0}_{1}.log", DateTime.Now.ToString("yyyyMMdd"), fileCount));

                this.fileStream = new FileStream(LogFilePath, FileMode.Create);
                this.streamWriter = new StreamWriter(this.fileStream);
                this.streamWriter.AutoFlush = true;
            }
        }
    }
}