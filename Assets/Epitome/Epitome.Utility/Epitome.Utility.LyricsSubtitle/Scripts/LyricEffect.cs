using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 用于显示歌词过渡动画的类( 可实现多种动画效果 , 这里设置 )
/// </summary>
public class LyricEffect : MonoBehaviour
{

	public static bool isStartEffect = false;

	/// <summary>
	/// 歌词效果实现过渡
	/// 1. 歌词开始的时候
	/// 2. 判断歌词播放过程中是否关闭或者推出了歌曲
	/// </summary>
	public static void StartLyricEffect (RectTransform rectFrontPanel, RectTransform rectBackPanel, int row)
	{
		KSC.KscWord kscword;
		KSC.Instance.Add.TryGetValue (row, out kscword);
//		rectBackPanel.DOSizeDelta (endValue, duration, snaping).SetEase (Ease.Linear);
	}


}
