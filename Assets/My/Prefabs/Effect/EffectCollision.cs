using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCollision : MonoBehaviour
{
    GameManager gm;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            //攻撃処理
            Enemy01 enemy01 = other.GetComponent<Enemy01>();
            enemy01.TakeDamage(100);
            gm = FindObjectOfType<GameManager>();
            StartCoroutine(gm.HitStop(0.7f, 0.7f));
        }
    }
}
