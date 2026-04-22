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

    

    void Start()
    {
        anim = GetComponent<Animator>();
        enemy01Detector = GetComponentInChildren<Enemy01Detector>();
        enemyChase = GetComponent<EnemyChase>();
    }

    // === 攻撃開始・終了処理 === //
    public void Enemy01_AttackStart()
    {
        punchCollider.SetActive(true);
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
        death = true;
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