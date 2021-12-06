using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Epitome;

public class MusicManager : MonoSingleton<MusicManager> 
{
    public MusicDataItem accompanyData;

    private void Start()
    {
        DontDestroyOnLoad();
    }
}
