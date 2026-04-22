using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゴーレムの挙動を制御するクラス
/// </summary>
public class Golem : MonoBehaviour
{
    AudioSource audioSource; // オーディオソースコンポーネント
    SoundManager soundManager; // サウンドマネージャー
    Boss_SoundList bossSoundList; // ゴーレムのサウンドリスト
    BossHPBar bossHPBar; // HPバー

    Animator anim; // アニメーターコンポーネント

    // === 敵のスポーン処理 ===
    public List<Transform> enemySpawnPos; // 敵をスポーンさせる位置のリスト
    public GameObject enemy01;
    int lastSpawnPos;


    float attackCooldown = 0; // 攻撃のクールダウン
    GameObject player; //プレイヤーのbody
    public List<Transform> throwObj_Pos; //攻撃の投げる位置
    public GameObject throwObj; //攻撃の投げるオブジェクト
    int lastAction = 0; //最後に行った行動

    
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player").transform.Find("Body").gameObject;
        audioSource = GetComponent<AudioSource>();
        soundManager = FindObjectOfType<SoundManager>();
        bossSoundList = FindObjectOfType<Boss_SoundList>();
        bossHPBar = FindObjectOfType<BossHPBar>();
    }

    void Update()
    {
        if(attackCooldown <= 0)
        {
            int rnd;
            
            do
            {
                rnd = Random.Range(1, 4);
            }
            while (rnd == lastAction); // 前回と同じなら引き直し

            if(rnd == 1) Attack1();
            else if(rnd == 2) Attack2();
            else if(rnd == 3) Attack3();

            lastAction = rnd; // 最後の行動を更新
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
    //ダメージを受ける
    void Damage()
    {
        anim.SetTrigger("Damage");
        attackCooldown = 5f;
    }
    // === 攻撃パターン ===
    //攻撃パターン1
    void Attack1()
    {
        anim.SetTrigger("Rage");
        //Rage音を再生
        soundManager.OnPlaySE(audioSource, bossSoundList.rageSound);
        attackCooldown = 10f;

        //敵をスポーンさせる
        int rnd;
        do rnd = Random.Range(0, enemySpawnPos.Count);
        while (lastSpawnPos == rnd); // 前回と同じスポーン位置は避ける
        Instantiate(enemy01, enemySpawnPos[rnd].position, enemySpawnPos[rnd].rotation);
        lastSpawnPos = rnd; // 最後のスポーン位置を更新
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

    //衝突判定
    void OnTriggerEnter(Collider other)
    {
        //岩
        if(other.gameObject.CompareTag("Stone"))
        {
            //ダメージを受ける
            Damage();
            soundManager.OnPlaySE(audioSource, bossSoundList.damageSound);
            
            // HPバーを更新
            // bossHPBar.UpdateHPBar(5f); 
            Stone stone = other.gameObject.GetComponent<Stone>();
            if(stone.sword_enchantLevel == 1) TakeDamage(5);
            if(stone.sword_enchantLevel == 2) TakeDamage(10);
            if(stone.sword_enchantLevel == 3) TakeDamage(15);
        }
    }

    //ダメージを受ける関数
    public void TakeDamage(int damage)
    {
        bossHPBar.UpdateHPBar(damage);
        Debug.Log("ダメージ: "+damage);
    }
}
