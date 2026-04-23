using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : MonoBehaviour
{
    // === ステータス ===
    public int hp = 100;

    [HideInInspector] public bool death = false;
    Animator anim;
    Enemy01Detector enemy01Detector;
    EnemyChase enemyChase;
    public GameObject punchCollider;

    //サウンド
    AudioSource audioSource;
    Enemy01_SoundList enemy01_SoundList;
    SoundManager sm;

    Golem golem;
    

    void Start()
    {
        anim = GetComponent<Animator>();
        enemy01Detector = GetComponentInChildren<Enemy01Detector>();
        enemyChase = GetComponent<EnemyChase>();
        audioSource = GetComponent<AudioSource>();
        enemy01_SoundList = GetComponent<Enemy01_SoundList>();
        sm = FindObjectOfType<SoundManager>();

        golem = FindObjectOfType<Golem>();
    }

    // === 攻撃開始・終了処理 === //
    public void Enemy01_AttackStart()
    {
        punchCollider.SetActive(true);
        sm.OnPlaySE(audioSource, enemy01_SoundList.attackSound);
    }
    public void Enemy01_AttackEnd()
    {
        punchCollider.SetActive(false);      //コライダーをオフ
        ChangeAnim("Punch", false);          //パンチをオフ
        // ChangeAnim("Walk1", true);           //歩く
        enemy01Detector.isChasing = true;    //追跡
        enemy01Detector.isAttacking = false; //攻撃フラグをオフ
    }


    /// <summary>
    /// アニメーション関数
    /// </summary>
    /// <param name="animName">アニメーション名</param>
    /// <param name="trigger">オンオフ</param>
    public void ChangeAnim(string animName, bool trigger)
    {
        anim.SetBool(animName, trigger);
    }

    // === ダメージ処理 === //
    //攻撃を受ける関数
    public void TakeDamage(int damage)
    {
        hp -= damage;
        if(hp <= 0 && !death) Die();

    }
    //死んだとき
    void Die()
    {
        punchCollider.SetActive(false);
        golem.TakeDamage(3); //ボスにダメージ
        sm.OnPlaySE(audioSource, enemy01_SoundList.deathSound, 3f);
        death = true;
        punchCollider.SetActive(false);
        enemyChase.chase = false;
        ChangeAnim("Punch", false);
        ChangeAnim("Walk1", false);
        ChangeAnim("Death", true); //死亡アニメーション
    }


    //Deathアニメーションが終了したら呼ばれる
    public void DeathAnim_End()
    {
        Destroy(this.gameObject);
    }
    
}