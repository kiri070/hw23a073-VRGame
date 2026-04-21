using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    GameManager gm; // ゲームマネージャー
    public Slider hpbar;
    [HideInInspector] public float maxHP;

    void Start()
    {
        gm = FindObjectOfType<GameManager>(); // ゲームマネージャーを取得
        // 難易度に応じてHPバーの最大値を設定
        if(gm.difficulty == GameManager.Difficulty.Easy)
        {
            hpbar.maxValue = 100f; // 簡単な難易度の最大HP
        }
        else if(gm.difficulty == GameManager.Difficulty.Normal)
        {
            hpbar.maxValue = 200f; // 普通の難易度の最大HP
        }
        else if(gm.difficulty == GameManager.Difficulty.Hard)
        {
            hpbar.maxValue = 300f; // 難しい難易度の最大HP
        }
        //HPバーに最大HPを設定
        maxHP = hpbar.maxValue;
        hpbar.value = maxHP;
    }

    /// <summary>
    /// HPバーを更新する関数
    /// </summary>
    /// <param name="damage">与えるダメージ</param>
    public void UpdateHPBar(float damage)
    {
        hpbar.value -= damage;
        
    }
}
