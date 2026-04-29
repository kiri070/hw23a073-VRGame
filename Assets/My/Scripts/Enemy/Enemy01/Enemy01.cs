using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : MonoBehaviour
{
    // === ステータス ===
    public int hp = 100;
    [HideInInspector]public bool canAttack = true;

    [HideInInspector] public bool death = false;
    Animator anim;
    Enemy01Detector enemy01Detector;
    EnemyChase enemyChase;
    public GameObject punchCollider;

    // === エフェクト ===
    public GameObject deathEffect;

    //サウンド
    AudioSource audioSource;
    Enemy01_SoundList enemy01_SoundList;
    SoundManager sm;

    Golem golem;
    Enemy01_HPBar enemy01_HPBar;
    

    void Start()
    {
        anim = GetComponent<Animator>();
        enemy01Detector = GetComponentInChildren<Enemy01Detector>();
        enemyChase = GetComponent<EnemyChase>();
        audioSource = GetComponent<AudioSource>();
        enemy01_SoundList = GetComponent<Enemy01_SoundList>();
        sm = FindObjectOfType<SoundManager>();

        golem = FindObjectOfType<Golem>();
        enemy01_HPBar = GetComponentInChildren<Enemy01_HPBar>();
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

        StartCoroutine(DelayAttack());       //攻撃クールタイム
    }

    //攻撃クールタイム管理
    IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(2f);
        enemy01Detector.attackDistance = enemy01Detector.defaultAttackDistance; //再度攻撃範囲を検出できるようにするため
        canAttack = true; //攻撃可能フラグ
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
    public void AnimTrigger(string animName)
    {
        anim.SetTrigger(animName);
    }

    // === ダメージ処理 === //
    //攻撃を受ける関数
    public void TakeDamage(int damage)
    {
        hp -= damage;
        if(hp < 0) hp = 0;
        enemy01_HPBar.UpdateHPBar(damage);
        if(hp <= 0 && !death) Die();

    }
    //死んだとき
    void Die()
    {
        // 物理演算をオフにして落下を防ぐ
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // 全てのコライダーをオフにする
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        punchCollider.SetActive(false);
        golem.TakeDamage(20); //ボスにダメージ
        sm.OnPlaySE(audioSource, enemy01_SoundList.deathSound, 3f);
        death = true;
        enemyChase.chase = false;
        ChangeAnim("Punch", false);
        ChangeAnim("Walk1", false);
        ChangeAnim("Death", true); //死亡アニメーション
    }


    //Deathアニメーションが終了したら呼ばれる
    public void DeathAnim_End()
    {
        //エフェクト
        Instantiate(deathEffect, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z),
            deathEffect.transform.rotation);
        Destroy(gameObject);
    }
    
}