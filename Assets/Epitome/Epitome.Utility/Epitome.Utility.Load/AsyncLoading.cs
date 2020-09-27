using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;


namespace Epitome
{
	public static class AsyncLoading 
	{
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">要下载的文件的网络路径</param>
        /// <param name="savePath">保存路径</param>

        public static void BeginDownloadSmallPackage(string url, string savePath)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ResPackageDownloadProgress);

                client.DownloadFileAsync(new System.Uri(url), savePath);
            }
        }

        private static void ResPackageDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            // 异步操作放到Unity主线程上运行
            Loom.QueueOnMainThread((param) =>
            {
                if (e.ProgressPercentage >= 100 && e.BytesReceived == e.TotalBytesToReceive)
                {
                    Debug.Log("下载增量包成功");
                }
                else
                {
                    Debug.Log("进度条代码放在这里");
                }
            });
        }
	}
}
