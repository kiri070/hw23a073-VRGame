using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BossBarrierBar : MonoBehaviour
{
    public Slider barrierBar;
    [SerializeField] float recoverDuration = 1.0f;
    [SerializeField] Ease recoverEase = Ease.OutCubic;

    Golem golem;
    Tween barrierTween;

    void Start()
    {
        golem = FindObjectOfType<Golem>();

        // バリアバーの最大値を設定
        barrierBar.maxValue = golem.barrier_value;
        barrierBar.value = golem.barrier_value;
        UpdateBarrierBar(0); //初期設定用
    }

    /// <summary>
    /// バリアバーを更新する関数
    /// </summary>
    /// <param name="damage">与えるダメージ</param>
    public void UpdateBarrierBar(int damage)
    {
        barrierTween?.Kill();
        //アニメーション付きで減少
        barrierTween = barrierBar.DOValue(barrierBar.value - damage, 1.0f).SetEase(Ease.OutCubic);
        if (barrierBar.value < 0) barrierBar.value = 0;
    }

    /// <summary>
    /// バリアバーをリセットする関数
    /// </summary>
    public void ResetBarrierBar()
    {
        barrierTween?.Kill();
        //アニメーション付きで減少
        barrierTween = barrierBar.DOValue(barrierBar.maxValue, recoverDuration)
            .SetEase(recoverEase);
    }

    void OnDestroy()
    {
        barrierTween?.Kill();
    }
}
