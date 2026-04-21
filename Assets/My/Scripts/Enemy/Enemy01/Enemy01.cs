using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : MonoBehaviour
{
    public float viewDistance = 5f;
    public float viewAngle = 90f;

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
        
    }

    void Update()
    {
        
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 dir = (other.transform.position - transform.position).normalized;

            // 自分の前方向
            Vector3 forward = transform.forward;

            // 角度計算
            float angle = Vector3.Angle(forward, dir);

            // 例えば視野90度（左右45度）
            if (angle < 45f)
            {
                // プレイヤーが前にいる
                Debug.Log("発見！");
                
                // ここで追跡処理
            }
        }
    }
}
