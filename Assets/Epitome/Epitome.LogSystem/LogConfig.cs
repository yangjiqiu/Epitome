using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Epitome;
using Epitome.LogSystem;

public class LogConfig : MonoSingleton<LogConfig> 
{
    private void Awake()
    {
        DontDestroyOnLoad();

        Log.EnableLog(true);
        Log.LogLevel(LogLevel.ALL);
        Log.LoadAppenders(MobileGUIAppender.Instance);

        Log.Debug("qweqweqwe");
        Log.Error("123123123132");
        Log.Debug("qweqweqwe");
        Log.Error("123123123132");
        Log.Debug("qweqweqwe");
        Log.Error("123123123132");

        Log.Debug(Screen.width);
        Log.Debug(Screen.height);
    }
}
