using Epitome;
using Epitome.LogSystem;
using Epitome.Utility.Load;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// 音频目录显示
/// </summary>
public class AudioDirectoryDisplay : MonoBehaviour 
{
	MusicData musicData = new MusicData();

	public Transform Item;

	public Text text;

	private void Awake()
	{

#if UNITY_EDITOR
		string filepath = Application.streamingAssetsPath + "/MusicLibrary_WAV.json";
#elif UNITY_IPHONE
        //没有试过 
        string filepath = Application.dataPath + "/Raw";
#elif UNITY_ANDROID
        string filepath = "jar:file://" + Application.dataPath + "!/assets/" + "MusicLibrary_WAV.json";
#endif

		//new Task(Loading.LoadJson<List<MusicDataItem>>(filepath, LoadJsonFinish));
		StartCoroutine(Loading.LoadJson<List<MusicDataItem>>(filepath, LoadJsonFinish));
		//using (StreamReader streamreader = new StreamReader("jar:file://" + Application.dataPath + "!/assets/" + "MusicLibrary_WAV.json"))
		//{
		//	JsonReader reader = new JsonReader(streamreader);
		//	musicData.MusicDataItems = JsonMapper.ToObject<List<MusicDataItem>>(reader);
		//	text.text += musicData.MusicDataItems.Count.ToString() + "\n";
		//}
		//text.text += (musicData.MusicDataItems.Count);
		//text.text += "123123123" + "\n";

	}

	private void LoadJsonFinish(object obj)
	{
		musicData.MusicDataItems = obj as List<MusicDataItem>;

		duqu();
	}

	private void duqu()
	{
		for (int i = 0; i < musicData.MusicDataItems.Count; i++)
		{
			MusicDataItem dataItem = musicData.MusicDataItems[i];
			dataItem.ID = (i + 1).ToString();
			Transform trans = Instantiate(Item);
			trans.gameObject.SetActive(true);
			trans.parent = Item.parent;

			MusicItem item = trans.GetComponent<MusicItem>();
			item.name = string.Format("{0:D2}", dataItem.ID);
			item.SetMusicDataItem(dataItem);
		}
	}
}
