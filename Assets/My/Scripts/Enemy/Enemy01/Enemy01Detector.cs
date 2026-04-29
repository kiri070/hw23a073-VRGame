using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Detector : MonoBehaviour
{
    public float viewDistance = 5f;
    public float viewAngle = 90f;
    [HideInInspector] public bool isChasing = false;
    [HideInInspector] public bool isAttacking = false;
    public float attackDistance = 2f;
    [HideInInspector]public float defaultAttackDistance; //攻撃範囲を保存する変数
    EnemyChase enemyChase;
    Enemy01 enemy01;
    GameManager gm;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        // 検知距離の円
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 forward = transform.forward;

        Vector3 left = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2, 0) * forward;

        Gizmos.DrawLine(transform.position, transform.position + left * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + right * viewDistance);
    }
    void Start()
    {
        enemyChase = GetComponentInParent<EnemyChase>();
        enemy01 = GetComponentInParent<Enemy01>();
        gm = FindObjectOfType<GameManager>();

        defaultAttackDistance = attackDistance; //攻撃範囲を保存
    }

    void OnTriggerStay(Collider other)
    {
        if(enemy01.death) return;
        if(gm.gameClear || gm.gameOver) return;
        
        if (other.CompareTag("Player"))
        {

            Vector3 dir = (other.transform.position - transform.position).normalized;
            Vector3 forward = transform.forward;

            float angle = Vector3.Angle(forward, dir);
            float distance = Vector3.Distance(transform.position, other.transform.position);

            // 視界に入ってる
            if (angle < viewAngle)
            {
                // 攻撃距離
                if (distance < attackDistance)
                {
                    if (enemy01.canAttack)
                    {
                        
                        isAttacking = true;                 //攻撃フラグをオン
                        enemyChase.chase = false;           //追跡をオフ
                        enemy01.AnimTrigger("Punch");
                        isChasing = false;                  //行動重複防止


                        enemy01.canAttack = false;
                        attackDistance = 0f; // 攻撃中は停止()
                    }
                }
                //移動
                else
                {
                    if (!isChasing)
                    {
                        enemyChase.chase = true;           //追跡をオン
                        isChasing = true;                  
                    } 
                }
            }
            //視界の外
            else
            {
                if (isChasing)
                {
                    enemyChase.chase = false;            //追跡をオフ

                    isChasing = false;
                }
            }
        }
    }
}