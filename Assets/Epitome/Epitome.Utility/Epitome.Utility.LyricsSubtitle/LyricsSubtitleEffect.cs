using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LyricsSubtitleEffect : MonoBehaviour 
{
	#region *********************************************************************字段

	//由外部传入的声音资源
	[HideInInspector, SerializeField]
	public AudioSource audioSource;
	//歌词前景颜色;歌词后景颜色
	[SerializeField]
	public Color32 frontTextColor = Color.white, backTextColor = Color.black, outlineColor = Color.white;
	//字体大小
	public int fontSize = 30;
	//歌词面板的前景部分和后景部分
	public RectTransform rectFrontLyricText, rectBackLyricMask, rectBackLyricText;
	public Slider slider;
	//歌词文件路径
	[HideInInspector, SerializeField]
	public string lyricFilePath;

	//是否开始播放当前行歌词内容
	public bool isStartLyricEffectTransition = true;
	//歌词调整进度 ( 纠错 )
	//	[HideInInspector]
	public float lyricAdjust = -5f;


	//歌词文本信息
	//	[HideInInspector]
	[SerializeField, HideInInspector]
	public Text _lyricText, _lyricText2;

	public Text _textContentLyric, _textLogMessage;

	//播放点
	public Transform PlayPoint;
	private int currentLine = -1;

	private Vector2 tempFrontSizeDelta, tempBackSizeDelta;

	//用于访问歌词正文部分的内容在KscWord类中
	private KSC.KscWord kscword = new KSC.KscWord();
	private KSC.KscWord curKscword = new KSC.KscWord();

	//内部定时器( 由外部传入参数来控制 , 用来记录歌曲播放的当前时间轴 )
	private float _timer = 0.00f;


	#endregion

	/// <summary>
	/// 初始化一些变量
	/// </summary>
	void InitSomething()
	{
		//坚持对歌词文件进行赋值操作
		if (_lyricText == null || rectFrontLyricText.GetComponent<ContentSizeFitter>() == null)
		{
			if (rectFrontLyricText.GetComponent<Text>() == null)
			{
				_lyricText = rectFrontLyricText.gameObject.AddComponent<Text>();
			}
			_lyricText = rectFrontLyricText.GetComponent<Text>();

			//保持歌词实现自适应
			//rectFrontLyricText.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			//rectFrontLyricText.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		}
		_lyricText2 = rectBackLyricMask.GetComponentInChildren<Text>();

		_lyricText2.text = _lyricText.text;

		//歌词颜色的更改初始化
		rectBackLyricMask.GetComponentInChildren<Text>().color = backTextColor;
		rectBackLyricMask.GetComponentInChildren<Outline>().effectColor = outlineColor;
		rectFrontLyricText.GetComponent<Text>().color = frontTextColor;

		//rectBackLyricMask.GetComponentInChildren<Text>().fontSize =
		//	rectFrontLyricText.GetComponent<Text>().fontSize = fontSize;



		//歌词过渡的前景部分 ( 用于判断过度遮罩的长度范围 )
		tempFrontSizeDelta = rectFrontLyricText.sizeDelta;
		tempBackSizeDelta = rectBackLyricMask.sizeDelta;

		//是否开始当前歌词行播放标志位
		isStartLyricEffectTransition = true;

		originalData = _lyricText.text;
		MExpalinTextLine = _lyricText.cachedTextGenerator.lines;
	}

	void Awake()
	{
		//初始化
		InitSomething();

	}

	void Start()
	{
		StartCoroutine(CalculateCurrentLine());
	}

	/// <summary>
	/// 控制歌词面板的显示
	///  1. 仅仅显示歌词面板信息 , 没有过渡效果!
	/// </summary>
	/// <param name="row">歌词正文部分行号.</param>
	/// <param name="isPanelView">If set to <c>true</c> 显示面板歌词</param>
	public void LyricPanelControllerView(KSC.KscWord curRowInfo, bool isPanelView)
	{
		//		Debug.Log ("当前行是否开始=====>" + isPanelView.ToString ());
		_textLogMessage.text = isStartLyricEffectTransition.ToString();

		rectBackLyricMask.sizeDelta = new Vector2(0f, rectFrontLyricText.sizeDelta.y);

		rectBackLyricMask.GetComponentInChildren<Text>().text = _lyricText.text = "";
		if (isPanelView)
		{
			//根据时间得到当前播放的是第i行的歌词
			//处理歌词面板信息 , 显示歌词
			foreach (var item in curRowInfo.PerLineLyrics)
			{
				_lyricText.text += item;
				rectBackLyricMask.GetComponentInChildren<Text>().text = _lyricText.text;
			}
			StartCoroutine(LyricPanelControllerEffect(curRowInfo, isPanelView));
		}
		else
		{
			StopAllCoroutines();
			rectBackLyricMask.sizeDelta = new Vector2(0f, rectFrontLyricText.sizeDelta.y);
			//			StartCoroutine (LyricPanelControllerEffect (curRowInfo, isPanelView));
			//当前歌词结束以后将歌词框初始化成0
			rectBackLyricMask.GetComponentInChildren<Text>().text = _lyricText.text = string.Empty;
		}
	}

	/// <summary>
	/// 开始实现歌此过渡效果, 仅仅效果实现
	/// 1. 使用Dotween的doSizedata实现
	/// 2. 动态调整蒙板的sizedata宽度
	/// 3. 根据歌曲当前播放的时间进度进行调整
	/// </summary>
	/// <returns>The panel controller effect.</returns>
	/// <param name="isPanelEffect">If set to <c>true</c> is panel effect.</param>
	public IEnumerator LyricPanelControllerEffect(KSC.KscWord curRowInfo, bool isPanelEffect)
	{
		//当前时间歌词播放进度的百分比比例
		int curWordIndex = 0;
		if (isPanelEffect)
		{
			rectBackLyricMask.DORewind();
			yield return null;
			rectBackLyricMask.sizeDelta = new Vector2(0f, rectFrontLyricText.sizeDelta.y);
			//开始效果过渡
			if (audioSource.isPlaying)
			{
				for (int i = 0; i < curKscword.PerLinePerLyricTime.Length; i++)
				{
					rectBackLyricMask.DOSizeDelta(
						new Vector2(((float)(i + 1) / curKscword.PerLinePerLyricTime.Length) * rectFrontLyricText.sizeDelta.x, rectFrontLyricText.sizeDelta.y)
						, curKscword.PerLinePerLyricTime[i] / 1000f
						, false).SetEase(Ease.Linear);
					//					Debug.Log ("第" + i + "个歌词时间");
					yield return new WaitForSeconds(curKscword.PerLinePerLyricTime[i] / 1000f);
				}
			}
			else
			{
				Debug.LogError("歌曲没有播放 ！！！！");
			}
		}
		else
		{
			yield return null;
			rectBackLyricMask.DOSizeDelta(new Vector2(0f, rectFrontLyricText.sizeDelta.y), 0f, true);
		}
	}

	/// <summary>
	/// 开始播放音乐的时候调用
	/// </summary>
	/// <param name="lyricFilePath">歌词文件路径.</param>
	/// <param name="audioSource">Audiosource用于判断播放状态.</param>
	/// <param name="frontColor">前景色.</param>
	/// <param name="backColor">后景.</param>
	/// <param name="isIgronLyricColor">如果设置为 <c>true</c> 则使用系统配置的默认颜色.</param>
	public void StartPlayMusic(string lyricFilePath, AudioSource audioSource, Color frontColor, Color backColor, Color outlineColor, bool isIgronLyricColor)
	{
		_timer = 0f;

		//初始化ksc文件
		KSC.InitKsc(lyricFilePath);

		this.lyricFilePath = lyricFilePath;
		this.audioSource = audioSource;

		_textContentLyric.text = string.Empty;

		if (!isIgronLyricColor)
		{
			//歌曲颜色信息
			this.frontTextColor = frontColor;
			this.backTextColor = backColor;
			this.outlineColor = outlineColor;
		}

		#region ****************************************************输出歌词文件信息
		//对初始化完成后的信息进行输出
		if (KSC.Instance.SongName != null)
		{
			print("歌名==========>" + KSC.Instance.SongName);
		}
		if (KSC.Instance.Singer != null)
		{
			print("歌手==========>" + KSC.Instance.Singer);
		}
		if (KSC.Instance.Pinyin != null)
		{
			print("拼音==========>" + KSC.Instance.Pinyin);
		}
		if (KSC.Instance.SongClass != null)
		{
			print("歌类==========>" + KSC.Instance.SongClass);
		}
		if (KSC.Instance.InternalNumber > 0)
		{
			print("歌曲编号=======>" + KSC.Instance.InternalNumber);
		}
		if (KSC.Instance.Mcolor != Color.clear)
		{
			print("男唱颜色=======>" + KSC.Instance.Mcolor);
		}
		if (KSC.Instance.Mcolor != Color.clear)
		{
			print("女唱颜色=======>" + KSC.Instance.Wcolor);
		}
		if (KSC.Instance.SongStyle != null)
		{
			print("风格==========>" + KSC.Instance.SongStyle);
		}
		if (KSC.Instance.WordCount > 0)
		{
			print("歌名字数=======>" + KSC.Instance.WordCount);
		}
		if (KSC.Instance.LangClass != null)
		{
			print("语言种类=======>" + KSC.Instance.LangClass);
		}

		//一般是独唱歌曲的时候使用全Tag标签展现参数信息
		foreach (var item in KSC.Instance.listTags)
		{
			print(item);
		}
		#endregion

		//显示整个歌词内容
		for (int i = 0; i < KSC.Instance.Add.Values.Count; i++)
		{
			KSC.Instance.Add.TryGetValue(i, out kscword);
			for (int j = 0; j < kscword.PerLineLyrics.Length; j++)
			{
				_textContentLyric.text += kscword.PerLineLyrics[j];
			}
			_textContentLyric.text += "\n";
		}
	}

	/// <summary>
	/// 停止播放按钮
	/// </summary>
	public void StopPlayMusic()
	{
		Debug.Log("停止播放按钮");
	}

	/// <summary>
	/// 主要用于歌词部分的卡拉OK过渡效果
	/// 1. 动态赋值歌词框的长度
	/// 2. 支持快进快退同步显示
	/// </summary>
	int row = 0, tempRow = 0;

	//void FixedUpdate()
	//{
	//	#region *********************************************************播放过渡效果核心代码
	//	//如果是播放状态并且没有快进或快退 , 获得当前播放时间 , 如果都下一句歌词了 , 则切换到下一句歌词进行过渡效果
	//	//1. 是否是暂停;
	//	//2. 是否开始播放
	//	//3. 是否播放停止
	//	if (audioSource != null && audioSource.isPlaying)
	//	{
	//		//进度条
	//		slider.value = _timer / audioSource.clip.length;
	//		//快进快退快捷键
	//		if (Input.GetKey(KeyCode.RightArrow))
	//		{
	//			audioSource.time = Mathf.Clamp((audioSource.time + 1f), 0f, 4.35f * 60f);
	//		}
	//		else if (Input.GetKey(KeyCode.LeftArrow))
	//		{
	//			audioSource.time = Mathf.Clamp((audioSource.time - 1f), 0f, 4.35f * 60f);
	//			//			} else if (Input.GetKeyUp (KeyCode.LeftArrow)) {
	//			isStartLyricEffectTransition = true;
	//			rectBackLyricMask.GetComponentInChildren<Text>().text = rectFrontLyricText.GetComponent<Text>().text = string.Empty;
	//		}

	//		//实时计时
	//		_timer = audioSource.time;

	//		//歌曲开始播放的时间
	//		_textLogMessage.text = _timer.ToString("F2");

	//		for (int i = 0; i < KSC.Instance.Add.Count; i++)
	//		{

	//			KSC.Instance.Add.TryGetValue(i, out kscword);

	//			//根据时间判断当前播放的是哪一行的歌词文件 ( 减去0.01可保证两句歌词衔接太快的时候的bug )
	//			if ((_timer >= (kscword.PerLineLyricStartTimer + lyricAdjust + 0.1f) && _timer <= (kscword.PerLintLyricEndTimer + lyricAdjust - 0.1f)) && isStartLyricEffectTransition)
	//			{
	//				tempRow = i;
	//				KSC.Instance.Add.TryGetValue(tempRow, out curKscword);
	//				isStartLyricEffectTransition = false;
	//				Debug.Log("当前播放====>" + i + "行");
	//				//歌词面板显示当前播放内容
	//				LyricPanelControllerView(curKscword, !isStartLyricEffectTransition);
	//			}
	//			else if ((_timer >= (curKscword.PerLintLyricEndTimer + lyricAdjust)) && !isStartLyricEffectTransition)
	//			{
	//				isStartLyricEffectTransition = true;
	//				//设置不显示歌词内容
	//				LyricPanelControllerView(curKscword, !isStartLyricEffectTransition);
	//			}
	//		}

	//		//			KSC.Instance.Add.TryGetValue (row, out kscword);
	//		//
	//		//			//根据时间判断当前播放的是哪一行的歌词文件 ( 减去0.01可保证两句歌词衔接太快的时候的bug )
	//		//			if ((_timer >= (kscword.PerLineLyricStartTimer + lyricAdjust + 0.1f) && _timer <= (kscword.PerLintLyricEndTimer + lyricAdjust)) && isStartLyricEffectTransition) {
	//		//				tempRow = row;
	//		//				KSC.Instance.Add.TryGetValue (tempRow, out curKscword);
	//		//				isStartLyricEffectTransition = false;
	//		//				Debug.Log ("当前播放====>" + row + "行");
	//		//				//歌词面板显示当前播放内容
	//		//				LyricPanelControllerView (curKscword, !isStartLyricEffectTransition);
	//		//			} else if ((_timer >= (curKscword.PerLintLyricEndTimer + lyricAdjust)) && !isStartLyricEffectTransition) {
	//		//				isStartLyricEffectTransition = true;
	//		//				//设置不显示歌词内容
	//		//				LyricPanelControllerView (curKscword, !isStartLyricEffectTransition);
	//		//				row = (row + 1) % KSC.Instance.Add.Count;
	//		//			} 
	//		#endregion
	//	}
	//}

	private IList<UILineInfo> MExpalinTextLine;
	private System.Text.StringBuilder MExplainText = null;


	private string originalData;

	private IEnumerator CalculateCurrentLine()
	{
		while (true)
		{
			yield return null;

			if (MExpalinTextLine.Count == 0)
			{
				MExpalinTextLine = _lyricText.cachedTextGenerator.lines;
			}

			int line = (int)((rectFrontLyricText.localPosition.y- PlayPoint.localPosition.y) / (rectFrontLyricText.rect.height / MExpalinTextLine.Count));

			if (currentLine != line)
			{
				currentLine = line;

					_lyricText.text = originalData;
					yield return new WaitForSeconds(0.001f);
					MExpalinTextLine = _lyricText.cachedTextGenerator.lines;
					
				if (MExpalinTextLine.Count-1 > currentLine)
				{
					MExplainText = new System.Text.StringBuilder(originalData);
					string layout = string.Format("<size={0}>", 75);

					MExplainText.Insert(MExpalinTextLine[currentLine].startCharIdx, layout);
					MExplainText.Insert(MExpalinTextLine[currentLine + 1].startCharIdx + layout.Length - 1, string.Format("</size>"));

					_lyricText.text = _lyricText2.text = MExplainText.ToString();
				}
			}

			rectBackLyricText.position = new Vector3(rectBackLyricText.position.x, rectFrontLyricText.position.y, rectBackLyricText.position.z);
		}
	}

	private void HideMask()
	{
		rectBackLyricMask.gameObject.SetActive(false);
		rectBackLyricMask.sizeDelta = new Vector2(0, rectBackLyricMask.sizeDelta.y);
	}

	private void ShowMask()
	{
		rectBackLyricMask.gameObject.SetActive(true);
		rectBackLyricMask.sizeDelta = new Vector2(0, rectBackLyricMask.sizeDelta.y);
	}

	Coroutine coroutine;

	public void ScrollRectChanged()
	{
		if (coroutine == null)
			coroutine = StartCoroutine(ScrollRectUpdata());
	}

	private IEnumerator ScrollRectUpdata()
	{
		HideMask();

		while (true)
		{
			yield return null;

			if (Input.GetMouseButtonUp(0))
			{
				ShowMask();
				StopCoroutine(coroutine);
				coroutine = null;
			}
		}
	}
}
