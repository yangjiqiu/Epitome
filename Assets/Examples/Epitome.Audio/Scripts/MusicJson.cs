using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicDataItem
{
	public string ID;
	/// <summary>
	/// 音乐名称
	/// </summary>
	public string MusicName;
	/// <summary>
	/// 时长
	/// </summary>
	public string Duration;
	/// <summary>
	/// 路径
	/// </summary>
	public string Path;
}

public class MusicData
{
	public List<MusicDataItem> MusicDataItems { get; set; }
}
