using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LyricView : MonoBehaviour {

    protected Lyric mLyric = new Lyric();
    protected AudioSource mAudioSource;

	// Use this for initialization
	void Start () {
        string lyricPath = GetTestLyricPath();

        mAudioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        mLyric.Load(lyricPath);
        // just for debug
        mLyric.PrintInfo();

        mAudioSource.Play();
    }
	
	// Update is called once per frame
	void Update () {
        // update show lyric
        UpdateLyric();

        // update music slider
        UnityEngine.UI.Slider slider = GameObject.Find("Canvas/Slider").GetComponent<UnityEngine.UI.Slider>();
        slider.value = mAudioSource.time / mAudioSource.clip.length;
    }

    protected void UpdateLyric()
    {
        // test code
        // get current music play timestamp
        Int64 timestamp = GetCurrentTimestamp();
        // search current lyric
        LyricItem currentItem = mLyric.SearchCurrentItem(timestamp);
        string text = "";
        string uiTextLyricName = "Canvas/TextLyric";
        string uiTextTimeName = "Canvas/TextTime";
        UnityEngine.UI.Text uiTextLyric = GameObject.Find(uiTextLyricName).GetComponent<UnityEngine.UI.Text>();
        if (null != uiTextLyric)
        {
            // show lyrics from index (currentItem.mIndex - showLyricSize) to (currentItem.mIndex + showLyricSize)
            List<LyricItem> items = mLyric.GetItems();
            int showLyricSize = 3;
            foreach (LyricItem item in items)
            {
                if (item == currentItem)
                {
                    // if current lyric, highlight text with color (R, G, B)
                    text += Lyric.WrapStringWithColorTag(item.mText, 255, 0, 0) + System.Environment.NewLine;
                }
                else if ((null == currentItem && item.mIndex < showLyricSize) 
                    || (null != currentItem && item.mIndex >= currentItem.mIndex - showLyricSize 
                    && item.mIndex <= currentItem.mIndex + showLyricSize))
                {
                    text += item.mText + System.Environment.NewLine;
                }
            }
            uiTextLyric.text = text;
        }
        else
        {
            Debug.LogError("GetComponent " + uiTextLyricName + " failed");
        }
        UnityEngine.UI.Text uiTextTime = GameObject.Find(uiTextTimeName).GetComponent<UnityEngine.UI.Text>();
        if (null != uiTextTime)
        {
            // show timestamp
            uiTextTime.text = "time: " + Lyric.TimestampToString(timestamp);
        }
        else
        {
            Debug.LogError("GetComponent " + uiTextTimeName + " failed");
        }
    }

    protected Int64 GetCurrentTimestamp()
    {
        return (Int64)(mAudioSource.time * 1000.0f);
    }

    public void OnSliderChanged()
    {
        // set the audio play position
        UnityEngine.UI.Slider slider = GameObject.Find("Canvas/Slider").GetComponent<UnityEngine.UI.Slider>();
        float value = Mathf.Clamp(slider.value, 0.0f, 1.0f);
        mAudioSource.time = value * mAudioSource.clip.length;

        // update show lyric
        UpdateLyric();
    }

    protected string GetTestLyricPath()
    {
        string fileName = "test.lrc";
        // test code
        // copy lrc file from streamingAssetsPath to persistentDataPath use www class for android
#if (UNITY_ANDROID) && !UNITY_EDITOR
        string srcPath = Application.streamingAssetsPath + System.IO.Path.AltDirectorySeparatorChar + fileName;
        string destPath = Application.persistentDataPath + System.IO.Path.AltDirectorySeparatorChar + "lyrics" + System.IO.Path.AltDirectorySeparatorChar + fileName;

        if (System.IO.File.Exists(destPath))
        {
            System.IO.File.Delete(destPath);
        }   

        using (WWW www = new WWW (srcPath))
        {
            while (!www.isDone) { }

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogWarning (www.error);
                return String.Empty;
            }
                
            //create Directory
            String dirPath = System.IO.Path.GetDirectoryName (destPath);
            if (!System.IO.Directory.Exists (dirPath))
            {
                System.IO.Directory.CreateDirectory (dirPath);
            }
            System.IO.File.WriteAllBytes (destPath, www.bytes);
        }
        return destPath;
#endif

        // just return streamingAssetsPath + fileName on other platform
        string dir = Application.streamingAssetsPath + System.IO.Path.DirectorySeparatorChar;
        return dir + fileName;
    }
}
