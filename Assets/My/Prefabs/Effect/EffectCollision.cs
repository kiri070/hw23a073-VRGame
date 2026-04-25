using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCollision : MonoBehaviour
{
    GameManager gm;
    Sword_red sword_Red;

    void Start()
    {
        sword_Red = FindObjectOfType<Sword_red>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            //攻撃処理
            Enemy01 enemy01 = other.GetComponent<Enemy01>();
            enemy01.TakeDamage(100);
            gm = FindObjectOfType<GameManager>();

            sword_Red.SendHaptic(1f, 0.5f, sword_Red.rightController); //振動
            StartCoroutine(gm.HitStop(0.7f, 0.7f));                    //ヒットストップ
        }
    }
}
