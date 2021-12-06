using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

/// <summary>
/// KSC歌词文件解析属性, 单例工具类 ( 解析解析解析解析解析解析解析解析解析!!!!!!重要的事情多说几遍 )
/// 1. 歌词部分标题信息用单例instance访问
/// 2. 正文信息部分使用KSCWord对象访问( 每句开始时间\结束时间\每句歌词文字的数组\每句歌词文件时间的数组 )
/// </summary>
public  class KSC : Singleton<KSC>
{
	/// <summary>
	/// 歌曲		歌名
	/// </summary>
	public string SongName { get; set; }

	/// <summary>
	/// 歌名字数		歌名字数
	/// </summary>
	public int WordCount{ get; set; }

	/// <summary>
	/// 歌名字数		歌名的拼音声母
	/// </summary>
	public string Pinyin{ get; set; }

	/// <summary>
	/// 歌名字数		歌曲语言种类
	/// </summary>
	public string LangClass{ get; set; }

	/// <summary>
	/// 歌类，如男女乐队等
	/// </summary>
	public string SongClass{ get; set; }

	/// <summary>
	/// 艺术家		演唱者，对唱则用斜杠"/"分隔
	/// </summary>
	public string Singer { get; set; }

	/// <summary>
	/// 歌曲编号		 歌曲编号
	/// </summary>
	public int InternalNumber{ get; set; }

	/// <summary>
	/// 歌曲风格
	/// </summary>
	public string SongStyle{ get; set; }

	/// <summary>
	/// 视频编号 
	/// </summary>
	public string VideoFileName{ get; set; }

	/// <summary>
	/// 前景颜色
	/// </summary>
	public Color Mcolor{ get; set; }

	/// <summary>
	/// 后景颜色
	/// </summary>
	public Color Wcolor{ get; set; }

	/// <summary>
	/// 偏移量
	/// </summary>
	public string Offset { get; set; }

	/// <summary>
	/// 各类标签
	/// </summary>
	public List<string> listTags = new List<string> ();

	/// <summary>
	/// 歌词正文部分信息 ( key = 行号 value = 解析出来的歌词正文部分的每句歌词信息 )
	/// </summary>
	public Dictionary<int,KscWord> Add = new Dictionary<int, KscWord> ();


	/// <summary>
	/// 获得歌词信息
	/// </summary>
	/// <param name="LrcPath">歌词路径</param>
	/// <returns>返回歌词信息(Lrc实例)</returns>
	public static KSC InitKsc (string LrcPath)
	{
		int row = 0;
		//KscWord对象
		//清除之前的歌曲歌词, 保持当前
		KSC.Instance.Add.Clear ();
		using (FileStream fs = new FileStream (LrcPath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
			string line = string.Empty;
			using (StreamReader sr = new StreamReader (fs, Encoding.Default)) {
				
				while ((line = sr.ReadLine ()) != null) {
					//每次循环新建一个对象用于存储不同行数内容
					KSC.KscWord kscWord = new KSC.KscWord ();
					#region ######################################合唱歌曲格式
					if (line.StartsWith ("karaoke.songname := '")) {
						Instance.SongName = SplitStrInfo (line);
					} else if (line.StartsWith ("karaoke.internalnumber := ")) {
						if (SplitIntInfo (line) != 0) {
							Instance.InternalNumber = SplitIntInfo (line);
						}
					} else if (line.StartsWith ("karaoke.singer := '")) {
						Instance.Singer = SplitStrInfo (line);
					} else if (line.StartsWith ("karaoke.wordcount := ")) {
						if (SplitIntInfo (line) != 0) {
							Instance.WordCount = SplitIntInfo (line);
						}
					} else if (line.StartsWith ("karaoke.pinyin := '")) {
						Instance.Pinyin = SplitStrInfo (line);
					} else if (line.StartsWith ("karaoke.langclass := '")) {
						Instance.LangClass = SplitStrInfo (line);
					} else if (line.StartsWith ("karaoke.songclass := '")) {
						Instance.SongClass = SplitStrInfo (line);
					} else if (line.StartsWith ("karaoke.songstyle := '")) {
						Instance.SongStyle = SplitStrInfo (line);
					} else if (line.StartsWith ("karaoke.videofilename :='")) {
						Instance.VideoFileName = SplitStrInfo (line);
					} else if (line.StartsWith ("mcolor :=rgb(")) {
						if (SplitColorInfo (line) != Color.clear) {
							Instance.Mcolor = SplitColorInfo (line);
						}
					} else if (line.StartsWith ("wcolor :=rgb(")) {
						if (SplitColorInfo (line) != Color.clear) {
							Instance.Wcolor = SplitColorInfo (line);
						}
						#endregion

						#region ##################################################独唱歌曲风格
					} else if (line.StartsWith ("karaoke.tag('")) {
						//获取tag标签的参数信息
						KSC.Instance.listTags.Add (SplitTagInfo (line));
						#endregion	

						#region ################################################歌词正文部分解析
					} else if (line.StartsWith (("karaoke.add"))) {																		//表示歌词正文部分
						if (SplitLyricStartTime (line) != null) {
							//行号 ( 从0行开始 )

							//获取每句歌词部分的开始时间
							kscWord.PerLineLyricStartTimer = SplitLyricStartTime (line);
							//获取每句歌词部分的结束时间
							kscWord.PerLintLyricEndTimer = SplitLyricEndTime (line);
							//获取每行歌词的内容,并存储到KSCWord中 ( 歌词文字的数组信息 左 → 右 )
							kscWord.PerLineLyrics = SplitPerLineLyrics (line);
							//获取每行中单个文字的过渡时间数组 ( 歌词文字过渡时间数组 左 → 右 )
							kscWord.PerLinePerLyricTime = SplitPerLinePerLyricTime (line);
							KSC.Instance.Add.Add (row, kscWord);
							row++;
						}
					} else {
						//忽略ksc文件中的其他部分
						if (line != "" && !line.Contains ("CreateKaraokeObject") && !line.Contains ("karaoke.rows") && !line.Contains ("karaoke.clear;") && !Regex.IsMatch (line, @"^\//")) {
							Debug.LogWarning ("歌词含有不能解析的部分 ===> " + line);
						}
					}
					#endregion
				}
			}
		} 
		Debug.Log ("LyricFileInitialized" + "      Path : \n" + LrcPath);
		return Instance;
	}

	#region ****************************************************************解析歌词用的正则表达式部分 需更新

	/// <summary>
	/// 处理信息(私有方法)
	/// </summary>
	/// <param name="line"></param>
	/// <returns>返回基础信息</returns>
	public static string SplitStrInfo (string line)
	{
//		char[] ch = new char[]{ '\0', '\0' };
//		return line.Substring (line.IndexOf ("'") + 1).TrimEnd (ch);
		string pattern = @"'\S{1,20}'";		//获取歌曲标签信息
		Match match = Regex.Match (line, pattern);

		//去除两端的小分号
		string resout = string.Empty;
		resout = match.Value.Replace ("\'", string.Empty);
		return resout;
	}

	/// <summary>
	/// 处理参数是数字的情况
	/// </summary>
	/// <returns>The int info.</returns>
	/// <param name="line">Line.</param>
	public static int SplitIntInfo (string line)
	{
		string pattern = @"\d+";		//获取歌曲标签参数为数字的信息
		Match match = Regex.Match (line, pattern);

		//去除两端的小分号
		int result = 0;
		result = Int32.Parse (match.Value);
		return result;
	}

	/// <summary>
	/// 处理参数颜色色值的情况	如: mcolor :=rgb(0, 0, 255);
	/// </summary>
	/// <returns>The color info.</returns>
	/// <param name="line">Line.</param>
	public static Color32 SplitColorInfo (string line)
	{
		string pattern = @"[r,R][g,G][b,G]?[\(](2[0-4][0-9])|25[0-5]|[01]?[0-9][0-9]?";		//获取歌曲标签参数为颜色值的信息
		MatchCollection matches = Regex.Matches (line, pattern);

		return new Color (float.Parse (matches [0].ToString ()), float.Parse (matches [1].ToString ()), float.Parse (matches [2].ToString ()));
	}

	/// <summary>
	/// 获取歌曲的标签部分信息	如 : karaoke.tag('语种', '国语');	// 国语/粤语/台语/外语
	/// </summary>
	/// <returns>The tag info.</returns>
	public static string SplitTagInfo (string line)
	{
		string temp;
		string pattern = @"\([\W|\w]+\)";		//获取歌曲标签参数为颜色值的信息
		Match match = Regex.Match (line, pattern);
		temp =	match.Value.Replace ("(", string.Empty).Replace (")", string.Empty).Replace ("'", string.Empty).Replace (",", "：");
		return temp;
	}

	/// <summary>
	/// 获取每句歌词正文部分的开始时间   (单位 : 秒)
	/// </summary>
	/// <returns>The lyric start time.</returns>
	/// <param name="line">Line.</param>
	public static float SplitLyricStartTime (string line)
	{
		float time = 0f;
		Regex regex = new Regex (@"\d{2}:\d{2}\.\d{2,3}", RegexOptions.IgnoreCase);				//匹配单句歌词时间 如: karaoke.add('00:29.412', '00:32.655'
		MatchCollection mc = regex.Matches (line);
		time = (float)TimeSpan.Parse ("00:" + mc [0].Value).TotalSeconds;
		return time;
	}

	/// <summary>
	/// 获取每句歌词正文部分的结束时间  (单位 : 秒)
	/// </summary>
	/// <returns>The lyric start time.</returns>
	/// <param name="line">Line.</param>
	public static float SplitLyricEndTime (string line)
	{
		Regex regex = new Regex (@"\d{2}:\d{2}\.\d{2,3}", RegexOptions.IgnoreCase);				//匹配单句歌词时间 如: karaoke.add('00:29.412', '00:32.655'
		MatchCollection mc = regex.Matches (line);
		float time = (float)TimeSpan.Parse ("00:" + mc [1].Value).TotalSeconds;
		return time;
	}

	/// <summary>
	/// 获取每句歌词部分的每个文字  和 PerLinePerLyricTime相匹配  (单位 : 秒)
	/// </summary>
	/// <returns>The line lyrics.</returns>
	/// <param name="line">Line.</param>
	public static string[] SplitPerLineLyrics (string line)
	{
		List<string> listStrResults = new List<string> ();
		string pattern1 = @"\[[\w|\W]{1,}]{1,}";		//获取歌曲正文每个单词 如 : karaoke.add('00:25.183', '00:26.730', '[五][十][六][个][星][座]', '312,198,235,262,249,286');
		string pattern2 = @"\'(\w){1,}\'";				//获取歌曲正文每个单词 如 : karaoke.add('00:28.420', '00:35.431', '夕阳底晚风里', '322,1256,2820,217,1313,1083');
		Match match = (line.Contains ("[") && line.Contains ("]")) ? Regex.Match (line, pattern1) : Regex.Match (line, pattern2);
		//删除掉  [ ] '
		if (match.Value.Contains ("[") && match.Value.Contains ("]")) {				//用于合唱类型的歌词文件
			string[] resultStr = match.Value.Replace ("][", "/").Replace ("[", string.Empty).Replace ("]", string.Empty).Split ('/');
			foreach (var item in resultStr) {
				listStrResults.Add (item);
			}
		} else if (match.Value.Contains ("'")) {														//用于独唱类型的歌词文件 ( 尚未测试英文歌词文件!!!!!!!!!!!!!!!!!!!!!!! )
			char[] tempChar = match.Value.Replace ("'", string.Empty).ToCharArray ();
			foreach (var item in tempChar) {
				listStrResults.Add (item.ToString ());
			}
		}
		return listStrResults.ToArray ();
	}

	/// <summary>
	/// 获取每句歌词部分的每个文字需要的过渡时间  和 PerLineLyrics相匹配  (单位 : 秒)
	/// </summary>
	/// <returns>The line per lyric time.</returns>
	/// <param name="line">Line.</param>
	public static float[] SplitPerLinePerLyricTime (string line)
	{
		string pattern = @"\'((\d){0,}\,{0,1}){0,}\'";		//获取歌曲正文每个单词过渡时间 如 : karaoke.add('00:25.183', '00:26.730', '[五][十][六][个][星][座]', '312,198,235,262,249,286');

		string str = null;
		List<float> listfloat = new List<float> ();
		//删除掉  多余项
		str = Regex.Match (line, pattern).Value.Replace ("'", string.Empty);
//		Debug.Log (str);
		foreach (var item in  str.Split (',')) {
			listfloat.Add (float.Parse (item));
		}
		return listfloat.ToArray ();
	}

	#endregion

	#region ********************************************************************歌词正文部分的时间与文字信息

	/// <summary>
	/// 用单独的类来管理歌词的正文部分 ( 在KSC类下 )主要用来存储每句歌词和每个歌词的时间信息
	/// 1. 每句歌词的时间的 ( 开始 - 结束 )
	/// 2. 每句歌词中单个文字的时间信息 (集合的形式实现)
	/// </summary>
	public class KscWord
	{
		/// <summary>
		/// 每行歌词部分开始的时间 (单位 : 秒)  (key=行号,value=时间)
		/// </summary>
		public  float PerLineLyricStartTimer  { get; set; }

		/// <summary>
		/// 每行歌词部分结束时间 (单位 : 秒) (key=行号,value=时间)
		/// </summary>
		public  float PerLintLyricEndTimer  { get; set; }

		/// <summary>
		/// 每行歌词的单个文字集合
		/// </summary>
		public  string[] PerLineLyrics{ get; set; }

		/// <summary>
		/// 每行歌词中单个文字的速度过渡信息 (单位 : 毫秒)
		/// </summary>
		public  float[] PerLinePerLyricTime{ get; set; }
	}

	#endregion
}
