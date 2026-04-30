using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player_HPBar : MonoBehaviour
{
    GameManager gm; // ゲームマネージャー
    public Slider hpbar;
    [HideInInspector]public float maxHP;
    Body body; //プレイヤーのbodyスクリプト
    public Text hpText;
    Tween hpbarTween;


    void Start()
    {
        body = FindObjectOfType<Body>();
        gm = FindObjectOfType<GameManager>();

        //HPバーに最大HPを設定
        if(gm.difficulty == GameManager.Difficulty.Easy) body.hp = 250;
        if(gm.difficulty == GameManager.Difficulty.Normal) body.hp = 200;
        if(gm.difficulty == GameManager.Difficulty.Hard) body.hp = 150;

        maxHP = body.hp;
        hpbar.maxValue = maxHP;
        hpbar.value = maxHP;
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
        hpText.text = "HP:" + ((body.hp / maxHP) * 100).ToString("F0") + "%"; //残りHPパーセント(小数点以下カット)
    }

    void Oestroy()
    {
        hpbarTween?.Kill();
    }
}
