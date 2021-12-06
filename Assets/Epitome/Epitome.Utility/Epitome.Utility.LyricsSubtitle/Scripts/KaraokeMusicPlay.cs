using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Video;

/// <summary>
/// 用法 :
/// 1. 使用LyricPanelEffect对象调用其中过渡效果的方法
/// 2. StartPlayMusic方法用于初始化歌词文件 , 每次切歌重新播放的时候要重新加载歌词文件
/// </summary>
public class KaraokeMusicPlay : MonoBehaviour
{
	/// <summary>
	/// 声音
	/// </summary>
	/// <value>The audio source.</value>
	[SerializeField]	
	public AudioSource _audioSource;
	[SerializeField] 
	public LayricPanelEffect _lyricEffect;
	public string _lyricFilePath;
	public Button _btnStartPlayMusic, _btnStopPlayMusic, _btnPausePlayMusic;
	public Button _btnFrontAdjust, _btnBackAdjust;

    /// <summary>
    ///配置信息
    /// </summary>
    public Config config = null;

    void Start ()
	{
		_lyricFilePath = Application.dataPath + "/Test/ParseLyrics/" + config.MisicName;
		_audioSource.clip = config.audioClip;
		_lyricEffect.lyricAdjust = config.yanchi;
		_lyricEffect._lyricText.text = config.panelView;
		_lyricEffect.audioSource = _audioSource;

		_btnStartPlayMusic.onClick.RemoveAllListeners ();
		_btnStartPlayMusic.onClick.AddListener (StartPlayMusic);
		_btnStopPlayMusic.onClick.RemoveAllListeners ();
		_btnStopPlayMusic.onClick.AddListener (StopPlayMusic);
		_btnPausePlayMusic.onClick.RemoveAllListeners ();
		_btnPausePlayMusic.onClick.AddListener (PausePlayMusic);

		//前调整歌词按钮
		_btnFrontAdjust.onClick.RemoveAllListeners ();
		_btnFrontAdjust.onClick.AddListener (() => {
			_lyricEffect.lyricAdjust += 0.5f;
		});
		//后退一秒
		_btnBackAdjust.onClick.RemoveAllListeners ();
		_btnBackAdjust.onClick.AddListener (() => {
			_lyricEffect.lyricAdjust -= 0.5f;
		});

		StartPlayMusic ();
	}

	/// <summary>
	/// 开始播放音乐
	/// </summary>
	void StartPlayMusic ()
	{
		//开始加载并初始化歌词文件 ( 路径 , 前景色 , 后景色 , 是否忽略系统颜色配置 )
		_lyricEffect.StartPlayMusic (_lyricFilePath, _audioSource, Color.blue, Color.black, Color.white, true);

		_audioSource.UnPause ();
		_audioSource.Play ();
	}

	/// <summary>
	/// Stops the play music.
	/// </summary>
	void StopPlayMusic ()
	{
		_audioSource.Stop ();
		_lyricEffect.StopPlayMusic ();
	}

	/// <summary>
	/// Pauses the play music.
	/// </summary>
	void PausePlayMusic ()
	{
		Time.timeScale = 0f;
		_audioSource.Pause ();
	}
}