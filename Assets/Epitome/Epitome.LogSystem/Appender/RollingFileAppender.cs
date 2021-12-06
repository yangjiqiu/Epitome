using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading;

namespace Epitome.LogSystem
{
    /// <summary>滚动文件输出</summary>
    public class RollingFileAppender : AsyncTask<RollingFileAppender>, ILogAppender
    {
#if UNITY_EDITOR
        string logRootPath = Application.dataPath + "/Log";
#elif UNITY_STANDALONE_WIN
        string logRootPath = Application.dataPath + "/Log";
#elif UNITY_STANDALONE_OSX
        string logRootPath = Application.dataPath + "/Log";
#else
        string logRootPath = Application.persistentDataPath + "/Log";
#endif
        // 时间格式化
        protected const string TIME_FORMATER = "yyyy-MM-dd hh:mm:ss,fff";

        /// <summary>
        /// 文件最大值 10M
        /// </summary>
        private int maxFileSize = 10 * 1024 * 1024;
        // 文件Writer
        private StreamWriter streamWriter;
        // 文件流
        private FileStream fileStream;
        // 文件路径
        private string filePath;
        // 写日志队列
        private List<LogData> writeList;
        // 等待队列
        private List<LogData> waitList;
        // 锁
        private object lockObj;
        // 是否停止的标志
        private bool stopFlag;
        /// <summary>
        /// 构造函数
        /// </summary>
        public RollingFileAppender()
        {
            this.filePath = Path.Combine(logRootPath, "game.log");
            if (File.Exists(filePath))
            {
                this.fileStream = new FileStream(filePath, FileMode.Append);
                this.streamWriter = new StreamWriter(this.fileStream);
                this.streamWriter.AutoFlush = true;

            }
            else
            {
                if (!Directory.Exists(logRootPath))
                {
                    Directory.CreateDirectory(logRootPath);
                }
                this.fileStream = new FileStream(filePath, FileMode.Create);
                this.streamWriter = new StreamWriter(this.fileStream);
                this.streamWriter.AutoFlush = true;
            }
            this.writeList = new List<LogData>();
            this.waitList = new List<LogData>();
            this.lockObj = new object();
            this.stopFlag = false;
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="log">Log.</param>
        /// <param name="logData">Log data.</param>
        public void Log(LogData logData)
        {
            lock (lockObj)
            {
                waitList.Add(logData);
                Monitor.PulseAll(lockObj);
            }
        }
        /// <summary>
        /// 关闭执行
        /// </summary>
        public override void Close()
        {
            this.stopFlag = true;
            if (null != this.fileStream)
            {
                this.fileStream.Close();
            }
        }
        /// <summary>
        /// 开始运行
        /// </summary>
        public override void Run()
        {
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

                foreach (LogData data in writeList)
                {
                    // 写日志
                    this.streamWriter.WriteLine(String.Format("{0}#{1}#{2}", System.DateTime.Now.ToString(TIME_FORMATER), data.logBasicData, data.logMessage));

                    // 写堆栈
                    if (null != data.logTrack)
                    {
                        this.streamWriter.WriteLine(data.logTrack);
                    }

                    // 判断是否触发策略
                    HandleTriggerEvent();
                }
            }

        }
        /// <summary>
        /// 处理Trigger事件
        /// </summary>
        private void HandleTriggerEvent()
        {
            if (this.fileStream.Length >= maxFileSize)
            {
                // 文件超过大小，重头开始写
                this.streamWriter.Close();
                this.fileStream.Close();

                // 重新开始写
                this.fileStream = new FileStream(this.filePath, FileMode.Create);
                this.streamWriter = new StreamWriter(this.fileStream);
                this.streamWriter.AutoFlush = true;
            }
        }
    }
}