using Epitome;

/// <summary>基础模型</summary>
public class BaseModel<T> : Epitome.Singleton<T> where T : Epitome.Singleton<T>
{
    protected Message message;
}
