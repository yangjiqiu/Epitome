using System.Collections;
using System.Collections.Generic;

namespace Epitome
{
    public class Message : IEnumerable<KeyValuePair<string,object>>
    {
        private Dictionary<string, object> dataDict = null;

        public string Name { get; private set; }
        public object Sender { get; private set; }
        public object Content { get; private set; }

        public object this[string key]
        {
            get
            {
                if (dataDict == null || !dataDict.ContainsKey(key))
                {
                    return null;
                }

                return dataDict[key];
            }

            set
            {
                if (dataDict == null)
                {
                    dataDict = new Dictionary<string, object>();
                }

                if (dataDict.ContainsKey(key))
                {
                    dataDict[key] = value;
                }
                else
                {
                    dataDict.Add(key, value);
                }
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            if (dataDict == null) yield break;

            foreach (KeyValuePair<string, object> kvp in dataDict)
            {
                yield return kvp;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dataDict.GetEnumerator();
        }

        public Message(string name, object sender)
        {
            Name = name;
            Sender = sender;
            Content = null;
        }

        public Message(string name, object sender,object content)
        {
            Name = name;
            Sender = sender;
            Content = content;
        }

        public Message(string name, object sender, object content, params object[] paramDict)
        {
            Name = name;
            Sender = sender;
            Content = content;

            if (paramDict.GetType() == typeof(Dictionary<string, object>))
            {
                foreach (object param in paramDict)
                {
                    foreach (KeyValuePair<string, object> kvp in param as Dictionary<string, object>)
                    {
                        this[kvp.Key] = kvp.Value;
                    }
                }
            }
        }

        public Message(Message message)
        {
            Name = message.Name;
            Sender = message.Sender;
            Content = message.Content;

            foreach (KeyValuePair<string, object> kvp in message.dataDict)
            {
                this[kvp.Key] = kvp.Value;
            }
        }

        public void Add(string key,object value)
        {
            this[key] = value;
        }

        public void Remove(string key)
        {
            dataDict.Remove(key);
        }

        public void Send()
        {
            MessageCenter.Instance.SendMessage(this);
        }
    }
}