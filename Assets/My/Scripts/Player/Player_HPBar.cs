using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_HPBar : MonoBehaviour
{
    GameManager gm; // ゲームマネージャー
    public Slider hpbar;
    [HideInInspector]public float maxHP;
    Body body; //プレイヤーのbodyスクリプト
    public Text hpText;


    void Start()
    {
        body = FindObjectOfType<Body>();

        //HPバーに最大HPを設定
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
        hpbar.value -= damage; //HPバー
        hpText.text = "HP:" + ((body.hp / maxHP) * 100).ToString("F0") + "%"; //残りHPパーセント(小数点以下カット)
    }
}
