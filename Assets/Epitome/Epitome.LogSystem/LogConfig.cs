using UnityEngine;
using Epitome;
using Epitome.LogSystem;

public class LogConfig : MonoSingleton<LogConfig> 
{
    private void Awake()
    {
        DontDestroyOnLoad();

        // 启用日志
        Log.EnableLog(true);

        // 设置日志级别
        Log.LogLevel(LogLevel.ALL);

        // 设置日志输出
        Log.LoadAppenders(AppenderType.MobileGUI);

        Log.Trace("开启日志系统");

        // 注册Unity日志的监听
        Log.RegisterLogMessage();

        Debug.Log("注册Unity日志的监听");
    }

    protected override void OnDestroy()
    {
        // 注销Unity日志的监听
        Log.UnRegisterLogMessage();
        base.OnDestroy();
    }
}