using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill02 : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 20f;
    public int destroyTime = 7;
    bool hasHit = false;
    GameManager gm;
    Sword_red sword_Red;
    Coroutine delayDestroyCoroutine;
    public GameObject hitEffect;

    void Start()
    {
        // 親オブジェクトのRigidbodyを取得
        rb = GetComponentInParent<Rigidbody>();
        
        // 自分の向き（剣の方向）に飛ぶように速度を設定
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }

        gm = FindObjectOfType<GameManager>();
        sword_Red = FindObjectOfType<Sword_red>();

        //何もなくても時間経過で削除
        delayDestroyCoroutine = StartCoroutine(DelayDestory());
    }

    void OnTriggerEnter(Collider other)
    {
        // 敵に当たった時の処理
        if (other.gameObject.CompareTag("Enemy") && !hasHit)
        {
            hasHit = true;

            // 敵へのダメージ処理
            Enemy01 enemy01 = other.GetComponent<Enemy01>();
            if (enemy01 != null)
            {
                enemy01.TakeDamage(150); // スキル2のダメージ
            }

            // ヒットストップと振動
            if (gm != null)
            {
                gm.StartHitStop(0.7f, 0.7f);
            }
            if (sword_Red != null)
            {
                sword_Red.SendHaptic(1f, 0.5f, sword_Red.rightController);
            }

            if (delayDestroyCoroutine != null)
            {
                StopCoroutine(delayDestroyCoroutine);
                delayDestroyCoroutine = null;
            }
            //ヒットエフェクト
            Instantiate(hitEffect, transform.position, hitEffect.transform.rotation);
            // 親オブジェクトを破棄
            Destroy(transform.parent.gameObject);
        }

        // 石に当たった時の処理
        if (other.gameObject.CompareTag("Stone") && !hasHit)
        {
            hasHit = true;

            if (sword_Red != null)
            {
                sword_Red.SendHaptic(1f, 0.5f, sword_Red.rightController);
            }

            if (delayDestroyCoroutine != null)
            {
                StopCoroutine(delayDestroyCoroutine);
                delayDestroyCoroutine = null;
            }

            // 親オブジェクトを破棄
            Destroy(transform.parent.gameObject);
        }
    }

    //一定時間後に削除
    IEnumerator DelayDestory()
    {
        yield return new WaitForSeconds(destroyTime);
        //ヒットエフェクト
        Instantiate(hitEffect, transform.position, hitEffect.transform.rotation);
        Destroy(transform.parent.gameObject);
    }
}

