using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossHPBar : MonoBehaviour
{
    GameManager gm; // ゲームマネージャー
    public Slider hpbar;
    [HideInInspector] public int maxHP;
    public Text hpText;

    Golem golem;
    Tween hpbarTween;
    void Start()
    {
        gm = FindObjectOfType<GameManager>(); // ゲームマネージャーを取得
        golem = FindObjectOfType<Golem>();

        // 難易度に応じてHPバーの最大値を設定
        if(gm.difficulty == GameManager.Difficulty.Easy)
        {
            maxHP = 150; // 簡単な難易度の最大HP
        }
        else if(gm.difficulty == GameManager.Difficulty.Normal)
        {
            maxHP = 250; // 普通の難易度の最大HP
        }
        else if(gm.difficulty == GameManager.Difficulty.Hard)
        {
            maxHP = 500; // 難しい難易度の最大HP
        }
        //HPバーに最大HPを設定
        hpbar.maxValue = maxHP;
        hpbar.value = maxHP;
        golem.hp = maxHP;
        UpdateHPBar(0); //初期設定用
    }

    /// <summary>
    /// HPバーを更新する関数
    /// </summary>
    /// <param name="damage">与えるダメージ</param>
    public void UpdateHPBar(float damage)
    {
        hpbarTween?.Kill();
        // hpbar.value -= damage;
        //アニメーション付きで減少
        hpbarTween = hpbar.DOValue(hpbar.value - damage, 1.0f).SetEase(Ease.OutCubic);
        hpText.text = ((float)golem.hp / maxHP * 100).ToString("F0") + "%";
    }

    void OnDestroy()
    {
        hpbarTween?.Kill();
    }
}
