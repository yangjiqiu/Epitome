/*----------------------------------------------------------------
 * 文件名：NewSocket
 * 文件功能描述：创建套接字
----------------------------------------------------------------*/
using System;
using System.Collections;
using UnityEngine;
using System.Net;//网络协议
using System.Net.Sockets;//Sockets网络应用程序
using System.Threading;//多线程编程
using Epitome.Manager;

namespace Epitome.Utility.Network
{
    public class NewSocket
	{
        //++++++++++++++++++++     定义变量     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 套接字.
        /// </summary>
        Socket mSocket;

        /// <summary>
        /// 广播事件标签
        /// </summary>
        string mSendDataEevent;

        /// <summary>
        /// 服务器端点.
        /// </summary>
        EndPoint mServerEndPoint;

		/// <summary>
		/// 客户端端点.
		/// </summary>
		EndPoint mClientEndPoint;

		/// <summary>
		/// 存放服务器数据流.
		/// </summary>
		byte[] mServerBytes;

		/// <summary>
		/// 当前数据流长度.
		/// </summary>
		int mCurveLength;

        //++++++++++++++++++++     初始化     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 初始化套接字默认TCP协议.
        /// </summary>
        public NewSocket(string varIP,int varPort,ProtocolType varType=ProtocolType.Tcp)
		{
            mSendDataEevent = varIP + varPort.ToString();

			mServerEndPoint = new IPEndPoint (IPAddress.Parse (varIP),varPort);

			mClientEndPoint = new IPEndPoint (IPAddress.Any,0);

			if (varType == ProtocolType.Tcp) 
			{
				mSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				//异步连接
				IAsyncResult tempResult = mSocket.BeginConnect (mServerEndPoint,new AsyncCallback(ConnectCallback),mSocket);
				//超时检测
				bool tempSuccess = tempResult.AsyncWaitHandle.WaitOne(5000,true);   

				if (!tempSuccess)     
				{     
					//超时     
					CloseSocket();     
					Debug.Log("连接超时");
				}
				else  
				{  
					//开启一个独立线程
					Thread tempThread = new Thread(new ThreadStart(TCP_Receive));     
					//指示该线程为后台线程,后台线程将会随着主线程的退出而退出
					tempThread.IsBackground = true;    
					//启动线程
					tempThread.Start();
				}     
			}
			else if (varType == ProtocolType.Udp)
			{
				mSocket = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				//开启一个独立线程
				Thread tempThread = new Thread(new ThreadStart(UDP_ReceiveFrom));
				//指示该线程为后台线程,后台线程将会随着主线程的退出而退出
				tempThread.IsBackground = true;    
				//启动线程
				tempThread.Start();
            }
		}

        /// <summary>
        /// 连接回调.
        /// </summary>
        void ConnectCallback(IAsyncResult varAR) { }

        //++++++++++++++++++++     Socket_TCP     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// TCP_异步发送数据.
        /// </summary>
        public void TCP_BeginSend(string varData)
		{
            byte[] tempByte= Data.StringTurnBytes (varData);
			mSocket.BeginSend (tempByte,0,tempByte.Length, SocketFlags.None,new AsyncCallback (TCP_EndSend),mSocket);
		}
        /// <summary>
        /// TCP_异步发送数据.
        /// </summary>
        public void TCP_BeginSend(byte[] varData)
        {
            mSocket.BeginSend(varData, 0, varData.Length, SocketFlags.None, new AsyncCallback(TCP_EndSend), mSocket);
        }

        void TCP_EndSend(IAsyncResult varAR)
		{
			Socket tempSocket = varAR.AsyncState as Socket;  
			tempSocket.EndSend (varAR);  
		}

		/// <summary>
		/// TCP_接收数据.
		/// </summary>
		void TCP_Receive()
		{
			while (true)  
			{  
				if (!mSocket.Connected)  
				{
                    //与服务器断开连接  
                    Thread.Sleep(10000000);
                }  
				try  
				{
                    if (mSocket.Available <= 0) continue;

                    //接受数据保存至bytes当中  
                    byte[] tempByte = new byte[4096];  
					//Receive方法中会一直等待服务端回发消息  
					int tempCount = mSocket.Receive(tempByte);  

					if (tempCount != 0)  
					{
                        //Debug.Log(Data.DecodeUTF8(System.Text.Encoding.Default.GetString(tempByte, 0, tempCount)));
                        StickyBagData(tempByte);
                    }
                }
                catch (Exception varException)
                {
                    Debug.LogWarning(mSendDataEevent + " : " + varException.Message);
                    Thread.Sleep(10000000);
                }
            }
		}

        //++++++++++++++++++++     Socket_UDP     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// UDP_数据发送.
        /// </summary>
        public void UDP_SendTo(string varData)
		{
			byte[] tempByte = Data.StringTurnBytes (varData);
            if (mSocket != null)
                mSocket.SendTo(tempByte, tempByte.Length, SocketFlags.None, mServerEndPoint);
        }

        /// <summary>
        /// UDP_数据发送.
        /// </summary>
        public void UDP_SendTo(byte[] varData)
        {
            Debug.Log(varData.Length);
            if (mSocket != null)
                mSocket.SendTo(varData, varData.Length, SocketFlags.None, mServerEndPoint);
        }

        /// <summary>
        /// UDP_数据接收.
        /// </summary>
        void UDP_ReceiveFrom()
		{
            try
            {
                if (mSocket == null)
                {
                    return;
                }

                while (true)
                {
                    try
                    {
                        if (mSocket.Available <= 0) continue;

                        var tempVar = new byte[4096];
                        var tempCount = mSocket.ReceiveFrom(tempVar, ref mClientEndPoint);

                        if (tempCount != 0)
                        {
                            Debug.Log(System.Text.Encoding.Default.GetString(tempVar, 0, tempCount).DecodeUTF8());
                            StickyBagData(tempVar);
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (SocketException varSocketException)
                    {
                        Debug.LogWarning(mSendDataEevent + " : " + varSocketException.Message);
                    }
                    catch (Exception varException)
                    {
                        Debug.LogWarning(mSendDataEevent + " : " + varException.Message);
                        Thread.Sleep(10000000);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Debug.LogWarning("发送/接收线程退出");
            }
            catch (Exception varException)
            {
                Debug.LogWarning(mSendDataEevent + " : " + varException.Message);
            }
        }

        //++++++++++++++++++++     粘包.广播数据     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 数据粘包.
        /// </summary>
        void StickyBagData(byte[] varByte)
		{
            try
            {
                if (mServerBytes == null)
                {
                    //获取包头长度
                    byte[] tempBaotouLength = new byte[1];
                    Array.Copy(varByte, 0, tempBaotouLength, 0, 1);
                    int tempBaotou = int.Parse(Data.BytesTurnString(tempBaotouLength));
                    //获取消息体长度
                    byte[] tempPacketLength = new byte[tempBaotou];
                    Array.Copy(varByte, 1, tempPacketLength, 0, tempBaotou);
                    int tempPacket = int.Parse(Data.BytesTurnString(tempPacketLength));
                    //获取消息体
                    mServerBytes = new byte[tempPacket];
                    if (1 + tempBaotou + tempPacket >= varByte.Length)
                        Array.Copy(varByte, 1 + tempBaotou, mServerBytes, 0, varByte.Length - (1 + tempBaotou));
                    else
                        Array.Copy(varByte, 1 + tempBaotou, mServerBytes, 0, tempPacket);
                    //当前数据流长度
                    mCurveLength = varByte.Length - (1 + tempBaotou);
                }
                else
                {
                    Array.Copy(varByte, 0, mServerBytes, mCurveLength, (mServerBytes.Length - mCurveLength < varByte.Length) ? mServerBytes.Length - mCurveLength : varByte.Length);
                    //当前数据流长度
                    mCurveLength += varByte.Length;
                }

                if (mServerBytes.Length <= mCurveLength) {
                    string tempStr = Data.BytesTurnString(mServerBytes);
                    mServerBytes = null;
                    mCurveLength = 0;
                    SendData(tempStr);
                }
            }
            catch (Exception varException)
            {
                Debug.LogWarning(mSendDataEevent + " : " + varException.Message);
            }
        }

        /// <summary>
        /// 广播数据.
        /// </summary>
        void SendData(object varObj)
        {
            try
            {
                EventManager.Instance.BroadcastEvent(mSendDataEevent, varObj);
            }
            catch (Exception varException)
            {
                Debug.LogWarning(mSendDataEevent + " : "+ varException.Message);
            }
        }

        //++++++++++++++++++++     结束Socket     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 关闭Socket.
        /// </summary>
        public void CloseSocket()
		{
			if (mSocket != null || mSocket.Connected)
			{
				//禁用这个套接字上发送和接收
				mSocket.Shutdown (SocketShutdown.Both);
				//关闭 Socket 连接和释放所有关联资源
				mSocket.Close();	
				//置空
				mSocket = null;
				Debug.Log ("CloseSocket");
			}
		}
	}
}
