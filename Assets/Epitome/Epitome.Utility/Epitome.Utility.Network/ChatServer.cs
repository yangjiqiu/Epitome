/*----------------------------------------------------------------
 * 文件名：ChatServer
 * 文件功能描述：聊天服务器
----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Epitome.Utility.Network
{
    public class ChatServer
    {
        static ChatServer mInstance;

        public static ChatServer GetSingleton() { if (mInstance == null) { mInstance = new ChatServer(); } return mInstance; }

        ChatServer()
        {
            mNewSocket = new NewSocket("119.29.39.186",123,System.Net.Sockets.ProtocolType.Udp);
        }

        NewSocket mNewSocket;

        //++++++++++++++++++++     聊天     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 世界喊话
        /// </summary>
        public void Shouting(string varID,byte[] varData)
        {
            //玩家ID  聊天信息
            byte[] tempID = Data.StringTurnBytes(varID);
            byte[] tempData = Data.MergeBytes(tempID, varData);
            mNewSocket.UDP_SendTo(Data.AddHeader(tempData));
        }

        /// <summary>
        /// 频道
        /// </summary>
        public void Channel()
        {
            //玩家ID 频道ID  聊天信息
        }

        /// <summary>
        /// 私聊
        /// </summary>
        public void Whisper()
        {
            //玩家ID 聊天对象ID  聊天信息
        }
    }
}
