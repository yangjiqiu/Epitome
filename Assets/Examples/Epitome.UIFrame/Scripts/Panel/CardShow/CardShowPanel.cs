using Epitome.UIFrame;
using System;
using UnityEngine;
using Epitome;
using System.Collections;

/// <summary>卡牌显示</summary>
public class CardShowPanel : PopUpPanelBase
{
    public Transform card;
    public GameObject but;

    public override string GetUIType()
    {
        return UIPanelType.CardShowPanel.ToString();
    }

    Task task;

    private float time;
    private float speedTime = 1;
    protected override void OnAwake()
    {
        UIMaskType = UIMaskType.ImPenetrable;

        task = new Task(DisplayAnimation());
        task.Pause();

        base.OnAwake();
    }

    public override void Display()
    {
        base.Display();

        but.SetActive(false);

        // 显示动画播放

        time = 0;
        task.UnPause();
    }

    private IEnumerator DisplayAnimation()
    {
        while (true)
        {
            yield return null;

            time += Time.deltaTime;
            card.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time / speedTime);

            if (time / speedTime >= 1)
            {
                but.SetActive(true);
                task.Pause();
            }
        }
    }
}