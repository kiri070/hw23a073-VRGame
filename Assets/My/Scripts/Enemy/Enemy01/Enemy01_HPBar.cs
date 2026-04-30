using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Enemy01_HPBar : MonoBehaviour
{
    GameManager gm; // ゲームマネージャー
    public Slider hpbar;
    [HideInInspector] public int maxHP;
    public Text hpText;
    public Enemy01 enemy01;
    Tween hpbarTween;

    void Start()
    {
        gm = FindObjectOfType<GameManager>(); // ゲームマネージャーを取得

        // 難易度に応じてHPバーの最大値を設定
        if(gm.difficulty == GameManager.Difficulty.Easy)
        {
            maxHP = 50; // 簡単な難易度の最大HP
        }
        else if(gm.difficulty == GameManager.Difficulty.Normal)
        {
            maxHP = 100; // 普通の難易度の最大HP
        }
        else if(gm.difficulty == GameManager.Difficulty.Hard)
        {
            maxHP = 150; // 難しい難易度の最大HP
        }
        //HPバーに最大HPを設定
        hpbar.maxValue = maxHP;
        hpbar.value = maxHP;
        enemy01.hp = maxHP;
        UpdateHPBar(0); //初期設定用
    }

    /// <summary>
    /// HPバーを更新する関数
    /// </summary>
    /// <param name="damage">与えるダメージ</param>
    public void UpdateHPBar(float damage)
    {
        hpbarTween?.Kill();
        hpbarTween = hpbar.DOValue(hpbar.value - damage, 1.0f).SetEase(Ease.OutCubic);
        if(hpbar.value < 0) hpbar.value = 0;
        hpText.text = ((float)enemy01.hp / maxHP * 100).ToString("F0") + "%";
    }

    void Oestroy()
    {
        hpbarTween?.Kill();
    }
}
