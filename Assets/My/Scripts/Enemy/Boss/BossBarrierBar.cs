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
    int currentBarrierValue;

    void Start()
    {
        golem = FindObjectOfType<Golem>();

        // バリアバーの最大値を設定
        barrierBar.maxValue = golem.barrier_value;
        barrierBar.value = golem.barrier_value;
        currentBarrierValue = golem.barrier_value;
        UpdateBarrierBar(0); //初期設定用
    }

    /// <summary>
    /// バリアバーを更新する関数
    /// </summary>
    /// <param name="damage">与えるダメージ</param>
    public void UpdateBarrierBar(int damage)
    {
        currentBarrierValue = Mathf.Max(currentBarrierValue - damage, 0);
        barrierTween?.Kill();
        //アニメーション付きで減少
        barrierTween = barrierBar.DOValue(currentBarrierValue, 1.0f).SetEase(Ease.OutCubic);
    }

    /// <summary>
    /// バリアバーをリセットする関数
    /// </summary>
    public void ResetBarrierBar()
    {
        currentBarrierValue = golem.barrier_value;
        barrierTween?.Kill();
        //アニメーション付きで減少
        barrierTween = barrierBar.DOValue(currentBarrierValue, recoverDuration)
            .SetEase(recoverEase);
    }

    void OnDestroy()
    {
        barrierTween?.Kill();
    }
}
