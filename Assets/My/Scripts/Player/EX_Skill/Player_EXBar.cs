using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player_EXBar : MonoBehaviour
{
    public Slider exBar;
    public GameObject maxImg;
    [HideInInspector] public int maxEX = 100;
    [HideInInspector] public float ex = 0;

    Tween exBarTween;
    void Start()
    {
        exBar.maxValue = maxEX;
        exBar.value = ex;
        UpdateEXBar(0); // 初期化

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

        if(maxImg != null)
        {
            maxImg.SetActive(targetEX >= maxEX);
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

    void OnDestroy()
    {
        exBarTween?.Kill();
    }
}
