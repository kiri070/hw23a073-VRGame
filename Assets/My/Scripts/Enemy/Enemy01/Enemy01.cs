using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : MonoBehaviour
{
    Animator anim;
    Enemy01Detector enemy01Detector;
    public GameObject punchCollider;

    void Start()
    {
        anim = GetComponent<Animator>();
        enemy01Detector = GetComponentInChildren<Enemy01Detector>();
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
}