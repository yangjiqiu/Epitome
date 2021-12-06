using Epitome.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicItem : MonoBehaviour 
{
    MusicDataItem musicData;

    Text Number;
    Text MusicName;
    Text Duration;

    private void Awake()
    {
        Number = transform.Find("Number").GetComponent<Text>();
        MusicName = transform.Find("MusicInfo/MusicName").GetComponent<Text>();
        Duration = transform.Find("MusicInfo/Duration").GetComponent<Text>();
    }

    public void SetMusicDataItem(MusicDataItem data)
    {
        musicData = data;

        Number.text = musicData.ID;
        MusicName.text = musicData.MusicName;
        Duration.text = musicData.Duration;
    }

    public void JumpRecording()
    {
        MusicManager.Instance.accompanyData = musicData;
        Project.LoadScene("AudioRecord");
    }
}
