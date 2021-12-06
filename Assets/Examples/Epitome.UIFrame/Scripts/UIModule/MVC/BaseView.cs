using UnityEngine;
using Epitome;

/// <summary>基础视图</summary>
public abstract class BaseView : MonoBehaviour
{
    protected abstract void Awake();

    protected abstract void OnDestroy();

    protected Message message;

    public virtual void initialize() { }

    /// <summary>接收消息</summary>
    public virtual void ReceiveMessage(object varObj)
    {
        // 根据消息显示视图
    }

    /// <summary>发送消息</summary>
    public virtual void SendMessage()
    {
        // 根据消息显示视图
    }
}