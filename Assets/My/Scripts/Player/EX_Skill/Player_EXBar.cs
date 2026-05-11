using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player_EXBar : MonoBehaviour
{
    public Slider exBar;
    public GameObject maxImg;
    public float autoChargeEXnum = 5f;
    [HideInInspector] public int maxEX = 100;
    [HideInInspector] public float ex = 0;

    //sound
    SoundManager sm;
    AudioSource audioSource;
    public AudioClip exSkillMax_SE;

    Tween exBarTween;
    void Start()
    {
        sm = FindObjectOfType<SoundManager>();
        audioSource = GetComponent<AudioSource>();

        exBar.maxValue = maxEX;
        exBar.value = ex;
        UpdateEXBar(0); // 初期化
        StartCoroutine(AutoChargeSkill()); //一定時間ごとにEXゲージが溜まる処理

        // UpdateEXBar(100); // デバック用

    }

    /// <summary>
    /// EXゲージを更新する関数
    /// </summary>
    /// <param name="value">増減するEXの数値</param>
    public void UpdateEXBar(float value)
    {
        ex = Mathf.Clamp(ex + value, 0, maxEX);
        float targetEX = ex;

        exBarTween?.Kill();
        exBarTween = exBar.DOValue(targetEX, 1.0f).SetEase(Ease.OutCubic);

        //ゲージが満タンなら
        if(maxImg != null && targetEX >= maxEX)
        {
            maxImg.SetActive(true); //MaxImgを表示
            sm.OnPlaySE(audioSource, exSkillMax_SE, 5f); //効果音
        }
        //ゲージが満タン未満なら
        else
        {
            maxImg.SetActive(false);
        }
    }

    /// <summary>
    /// EXゲージを消費する関数
    /// </summary>
    /// <param name="value">消費するEXの数値</param>
    /// <returns>Whether the gauge was consumed.</returns>
    public bool UseEX(float value)
    {
        if(ex < value) return false;

        UpdateEXBar(-value);
        return true;
    }

    //一定時間ごとにEXゲージが溜まる処理
    IEnumerator AutoChargeSkill()
    {
        while(true)
        {
            yield return new WaitForSeconds(20f);
            UpdateEXBar(autoChargeEXnum);
        }
    }

    void OnDestroy()
    {
        exBarTween?.Kill();
    }
}
