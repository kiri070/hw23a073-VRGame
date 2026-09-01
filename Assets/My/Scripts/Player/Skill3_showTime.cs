using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class Skill3_showTime : MonoBehaviour
{
    Sword_red sword_Red;
    public Slider bar;
    int maxValue;
    float enchant_time; //エンチャントの時間を入れる
    Tween tween;
    Coroutine calculation;

    SoundManager sm;
    public AudioClip enchant_end;
    AudioSource audioSource;

    private void Start()
    {
        sm = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        audioSource = GetComponent<AudioSource>();
    }
    //剣のスクリプトを入れる
    public void Set_Sword_red(Sword_red script)
    {
        sword_Red = script;
        enchant_time = sword_Red.enchant03_duration;
        bar.value = 0;
    }

    //スキル3が発動されたときに呼ばれる関数
    public void StartBar()
    {
        if (calculation != null)
        {
            StopCoroutine(calculation);
        }

        tween?.Kill();

        bar.maxValue = enchant_time;
        bar.value = enchant_time;
        calculation = StartCoroutine(CalculateTime());
    }
    public void UpdateBar(float time)
    {
        tween?.Kill();

        float targetValue = Mathf.Max(bar.value - time, 0);

        tween = bar.DOValue(targetValue, 1.0f)
            .SetEase(Ease.OutCubic);
    }

    void OnDestroy()
    {
        tween?.Kill();
    }

    IEnumerator CalculateTime()
    {
        for (float i = enchant_time; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            UpdateBar(1);
        }
        sm.OnPlaySE(audioSource, enchant_end, 5f);
    }

}
