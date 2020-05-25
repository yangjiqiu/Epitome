using System.Collections.Generic;
using UnityEngine;

namespace Epitome
{
    public delegate void MessageEvent(Message message);

    public class MessageCenter : Singleton<MessageCenter>
    {
        private MessageCenter() { }

        private Dictionary<string, List<MessageEvent>> messageEventDict = null;
        private Dictionary<object, Dictionary<string, List<MessageEvent>>> objectEventDict = null;

        public override void OnSingletonInit()
        {
            messageEventDict = new Dictionary<string, List<MessageEvent>>();
            objectEventDict = new Dictionary<object, Dictionary<string, List<MessageEvent>>>(); 
        }

        public void AddListener(string messageName, MessageEvent messageEvent)
        {
            List<MessageEvent> list = null;

            if (messageEventDict.ContainsKey(messageName))
            {
                list = messageEventDict[messageName];
                list.Add(messageEvent);
                messageEventDict[messageName] = list;
            }
            else
            {
                list = new List<MessageEvent>();
                list.Add(messageEvent);
                messageEventDict.Add(messageName, list);
            }
        }

        public void RemoveListener(string messageName, MessageEvent messageEvent)
        {
            List<MessageEvent> list = null;

            if (messageEventDict.ContainsKey(messageName))
            {
                list = messageEventDict[messageName];
                if (list.Contains(messageEvent))
                {
                    list.Remove(messageEvent);
                }

                if (list.Count <= 0)
                {
                    messageEventDict.Remove(messageName);
                }
                else
                {
                    messageEventDict[messageName] = list;
                }
            }
        }

        public void AddListener(object Sender, string messageName, MessageEvent messageEvent)
        {
            Dictionary<string, List<MessageEvent>> dic = null;

            if (objectEventDict.TryGetValue(Sender,out dic))
            {
                dic = objectEventDict[Sender];

                List<MessageEvent> list = null;

                if (dic.TryGetValue(messageName,out list))
                {
                    list = dic[messageName];
                    list.Add(messageEvent);
                    dic[messageName] = list;
                }
                else
                {
                    list = new List<MessageEvent>();
                    list.Add(messageEvent);
                    dic.Add(messageName, list);
                }

                objectEventDict[Sender] = dic;
            }
            else
            {
                dic = new Dictionary<string, List<MessageEvent>>();
                dic.Add(messageName, new List<MessageEvent>() { messageEvent });
                objectEventDict.Add(Sender, dic);
            }

            AddListener(messageName, messageEvent);
        }

        public void RemoveSenderAllListener(object Sender)
        {
            Dictionary<string, List<MessageEvent>> dic = null;

            if (objectEventDict.TryGetValue(Sender, out dic))
            {
                foreach (KeyValuePair<string, List<MessageEvent>> item in dic)
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        RemoveListener(item.Key, item.Value[i]);
                    }
                }

                objectEventDict.Remove(Sender);
            }
        }

        public void RemoveNameAllListener(string messageName)
        {
            messageEventDict.Remove(messageName);
        }

        public void RemoveAllListener()
        {
            messageEventDict.Clear();
            objectEventDict.Clear();
        }

        public void SendMessage(Message message)
        {
            DoMessageDispatcher(message);
        }

        public void SendMessage(string name,object sender)
        {
            SendMessage(new Message(name, sender));
        }

        public void SendMessage(string name, object sender,object content)
        {
            SendMessage(new Message(name, sender, content));
        }

        public void SendMessage(string name, object sender, object content,params object[] paramDict)
        {
            SendMessage(new Message(name, sender, content, paramDict));
        }

        /// <summary>
        /// Do message dispatcher.
        /// </summary>
        /// <param name="message"></param>
        public void DoMessageDispatcher(Message message)
        {
            if (messageEventDict == null || !messageEventDict.ContainsKey(message.Name)) return;

            List<MessageEvent> list = messageEventDict[message.Name];

            for (int i = 0; i < list.Count; i++)
            {
                MessageEvent messageEvent = list[i];

                if (null != messageEvent)
                {
                    messageEvent(message);
                }
            }
        }
    }
}

