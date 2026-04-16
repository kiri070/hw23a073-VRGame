using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゴーレムの挙動を制御するクラス
/// </summary>
public class Golem : MonoBehaviour
{
    Animator anim; // アニメーターコンポーネント
    float attackCooldown = 0; // 攻撃のクールダウン
    GameObject player; //プレイヤーのbody
    public List<Transform> throwObj_Pos; //攻撃の投げる位置
    public GameObject throwObj; //攻撃の投げるオブジェクト
    
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player").transform.Find("Body").gameObject;
    }

    void Update()
    {
        if(attackCooldown <= 0)
        {
            int rnd = Random.Range(3, 4);
            
            if(rnd == 0) Idle();
            else if(rnd == 1) Attack1();
            else if(rnd == 2) Attack2();
            else if(rnd == 3) Attack3();

        }
        
        //0以下にならないようにクールタイムを減らす
        if(attackCooldown >= 0) attackCooldown -= Time.deltaTime;
    }
    //Idle状態
    void Idle()
    {
        anim.SetTrigger("IdleAction");
        attackCooldown = 10f;
    }
    //攻撃パターン1
    void Attack1()
    {
        anim.SetTrigger("Rage");
        attackCooldown = 5f;
    }
    //攻撃パターン2
    void Attack2()
    {
        anim.SetTrigger("Hit");
        attackCooldown = 5f;
    }
    //攻撃パターン3 (投げる攻撃)
    void Attack3()
    {
        anim.SetTrigger("Hit2");
        int rnd = Random.Range(1, 5);
        if(rnd == 1) Instantiate(throwObj, throwObj_Pos[0].position, Quaternion.identity);
        else if(rnd == 2)
        {
            Instantiate(throwObj, throwObj_Pos[0].position, Quaternion.identity);
            Instantiate(throwObj, throwObj_Pos[1].position, Quaternion.identity);
        }
        else if(rnd == 3)
        {
            Instantiate(throwObj, throwObj_Pos[0].position, Quaternion.identity);
            Instantiate(throwObj, throwObj_Pos[1].position, Quaternion.identity);
            Instantiate(throwObj, throwObj_Pos[2].position, Quaternion.identity);
        }
        else if(rnd == 4)
        {
            Instantiate(throwObj, throwObj_Pos[0].position, Quaternion.identity);
            Instantiate(throwObj, throwObj_Pos[1].position, Quaternion.identity);
            Instantiate(throwObj, throwObj_Pos[2].position, Quaternion.identity);
            Instantiate(throwObj, throwObj_Pos[3].position, Quaternion.identity);
        }
        attackCooldown = 10f;
    }
}
