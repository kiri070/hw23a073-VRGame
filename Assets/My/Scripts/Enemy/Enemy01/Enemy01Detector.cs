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
    [HideInInspector] public float defaultAttackDistance;

    [SerializeField] float attackDelay = 1f; // 攻撃開始までの待ち時間
    bool attackWaiting = false;              // 攻撃待機中か

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

        defaultAttackDistance = attackDistance;
    }

    void OnTriggerStay(Collider other)
    {
        if (enemy01.death) return;
        if (gm.gameClear || gm.gameOver) return;

        if (other.CompareTag("Player"))
        {
            Vector3 dir = (other.transform.position - transform.position).normalized;
            Vector3 forward = transform.forward;

            float angle = Vector3.Angle(forward, dir);
            float distance = Vector3.Distance(transform.position, other.transform.position);

            // 視界に入っている
            if (angle < viewAngle)
            {
                // 攻撃距離
                if (distance < attackDistance)
                {
                    if (enemy01.canAttack && !attackWaiting)
                    {
                        StartCoroutine(AttackDelay(other.transform));
                    }
                }
                // 移動
                else
                {
                    if (!isChasing)
                    {
                        enemyChase.chase = true;
                        isChasing = true;
                    }
                }
            }
            // 視界の外
            else
            {
                if (isChasing)
                {
                    enemyChase.chase = false;
                    isChasing = false;
                }
            }
        }
    }

    // 攻撃開始まで待つ
    IEnumerator AttackDelay(Transform player)
    {
        attackWaiting = true;

        // 攻撃開始まで待つ
        yield return new WaitForSeconds(attackDelay);

        // 待っている間にプレイヤーが範囲外へ出た場合
        if (player == null)
        {
            attackWaiting = false;
            yield break;
        }

        Vector3 dir = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        float distance = Vector3.Distance(transform.position, player.position);

        // まだ視界内＋攻撃範囲内なら攻撃
        if (!enemy01.death &&
            !gm.gameClear &&
            !gm.gameOver &&
            enemy01.canAttack &&
            angle < viewAngle &&
            distance < defaultAttackDistance)
        {
            isAttacking = true;
            enemyChase.chase = false;
            isChasing = false;

            enemy01.AnimTrigger("Punch");

            enemy01.canAttack = false;
            attackDistance = 0f;
        }

        attackWaiting = false;
    }
}