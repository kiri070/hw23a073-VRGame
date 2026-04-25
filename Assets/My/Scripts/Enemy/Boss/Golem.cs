using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

/// <summary>
/// ゴーレムの挙動を制御するクラス
/// </summary>
public class Golem : MonoBehaviour
{
    AudioSource audioSource; // オーディオソースコンポーネント
    SoundManager soundManager; // サウンドマネージャー
    Boss_SoundList bossSoundList; // ゴーレムのサウンドリスト
    BossHPBar bossHPBar; // HPバー
    [HideInInspector] public int hp;
    GameManager gm;
    public GameObject bgm_Obj;

    // === ボスのムービー ===
   bool bossMovie = true;
   bool introStart = false;
    bool introEnd = false;
    public Transform landPoint; // 着地点


    Animator anim; // アニメーターコンポーネント

    // === 敵のスポーン処理 ===
    public List<Transform> enemySpawnPos; // 敵をスポーンさせる位置のリスト
    public GameObject enemy01;
    public GameObject enemy01_SpawnEffect;
    int lastSpawnPos;

    // === 波動攻撃 === 
    public Transform hadouPos;
    public GameObject hadouEffect;

    float attackCooldown = 0; // 攻撃のクールダウン
    GameObject player; //プレイヤーのbody
    public List<Transform> throwObj_Pos; //攻撃の投げる位置
    public GameObject throwObj; //攻撃の投げるオブジェクト
    int lastAction = 0; //最後に行った行動

    Sword_red sword_Red;

    
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player").transform.Find("Body").gameObject;
        audioSource = GetComponent<AudioSource>();
        soundManager = FindObjectOfType<SoundManager>();
        bossSoundList = FindObjectOfType<Boss_SoundList>();
        bossHPBar = FindObjectOfType<BossHPBar>();
        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        // ボス登場ムービー
        if (bossMovie && !introStart)
        {
            introStart = true;
            StartCoroutine(BossIntro());
            return;
        }
        //ムービーが終わったら
        if (!introEnd) return;

        if(gm.gameOver) return;
        if(hp <= 0) return;

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
        gm.Shake(1f, 0.9f, 50, 90);

        //Rage音を再生
        soundManager.OnPlaySE(audioSource, bossSoundList.rageSound);
        attackCooldown = 10f;

        StartCoroutine(SpawnEnemy01());
    }
    //攻撃パターン2 (波動攻撃)
    void Attack2()
    {
        anim.SetTrigger("Hit");
        Instantiate(hadouEffect, hadouPos.transform.position, hadouEffect.transform.rotation); //エフェクト
        gm.Shake(0.7f, 0.6f, 30, 90);
        soundManager.OnPlaySE(audioSource, bossSoundList.hadouAttackSound, 3f); //音
        attackCooldown = 8f;
    }
    //攻撃パターン3 (投げる攻撃)
    void Attack3()
    {
        //音
        soundManager.OnPlaySE(audioSource, bossSoundList.throwSound, 3f);

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
        if(hp <= 0) return;

        //岩
        if(other.gameObject.CompareTag("Stone"))
        {
            //ダメージを受ける
            Damage();
            soundManager.OnPlaySE(audioSource, bossSoundList.damageSound);
            
            // HPバーを更新
            // bossHPBar.UpdateHPBar(5f); 
            Stone stone = other.gameObject.GetComponent<Stone>();
            if(stone.sword_enchantLevel == 1) TakeDamage(20);
            if(stone.sword_enchantLevel == 2) TakeDamage(25);
            if(stone.sword_enchantLevel == 3) TakeDamage(30);
        }
    }

    //敵のスポーン処理
    IEnumerator SpawnEnemy01()
    {
        List<GameObject> particlesl = new List<GameObject>();

        // 敵スポーン数(難易度ごとに変化)最大4
        int spawnCount = 0; //宣言
        if(gm.difficulty == GameManager.Difficulty.Easy) spawnCount = Random.Range(1, 2);
        if(gm.difficulty == GameManager.Difficulty.Normal) spawnCount = Random.Range(1, 3);
        if(gm.difficulty == GameManager.Difficulty.Hard) spawnCount = Random.Range(1, 5);
        

        // シャッフル
        List<Transform> shuffled = new List<Transform>(enemySpawnPos);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int rnd = Random.Range(i, shuffled.Count);

            Transform temp = shuffled[i];
            shuffled[i] = shuffled[rnd];
            shuffled[rnd] = temp;
        }

        // エフェクト生成
        for (int i = 0; i < spawnCount && i < shuffled.Count; i++)
        {
            GameObject effect = Instantiate(
                enemy01_SpawnEffect,
                shuffled[i].position,
                shuffled[i].rotation
            );

            particlesl.Add(effect);
        }

        // 全エフェクト終了待ち
        yield return new WaitUntil(() =>
        {
            foreach (GameObject obj in particlesl)
            {
                if (obj == null) continue;

                ParticleSystem[] ps = obj.GetComponentsInChildren<ParticleSystem>();

                foreach (ParticleSystem p in ps)
                {
                    if (p.IsAlive(true))
                        return false;
                }
            }

            return true;
        });

        // 敵スポーン
        for (int i = 0; i < spawnCount && i < shuffled.Count; i++)
        {
            Instantiate(enemy01, shuffled[i].position, shuffled[i].rotation);
        }
    }

    //Dieアニメーションが終了時に呼ばれる
    public void BossDie()
    {
        gm.gameClear = true;
    }

    //ボスが死んで地面についたとき
    public void Boss_Vibration()
    {
        //振動
        sword_Red = FindObjectOfType<Sword_red>();
        sword_Red.SendHaptic(1, 1, sword_Red.leftController);
        sword_Red.SendHaptic(1, 1, sword_Red.rightController);
        gm.Shake(2f, 1.5f, 90, 90);
    }

    //ボスムービー
    IEnumerator BossIntro()
    {
        anim.SetTrigger("Jump");
        //効果音
        soundManager.OnPlaySE(audioSource, bossSoundList.rageSound);
        Vector3 startPos = transform.position;
        Vector3 targetPos = landPoint.position;

        float time = 0f;
        float duration = 1.2f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            t = t * t; // 加速落下

            transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        transform.position = targetPos;

        anim.SetTrigger("Land");

        gm.Shake(2f, 1f, 90, 50);

        //挑戦開始効果音
        soundManager.OnPlaySE(audioSource, bossSoundList.bossBGM01, 3f);
        yield return new WaitForSeconds(3.5f);

        //咆哮
        anim.SetTrigger("Rage");
        soundManager.OnPlaySE(audioSource, bossSoundList.rageSound);
        yield return new WaitForSeconds(4f);

        bgm_Obj.SetActive(true); //BGMをオン
        introEnd = true; //ムービー終了
    }

    //ダメージを受ける関数
    public void TakeDamage(int damage)
    {
        hp -= damage;
        if(hp < 0) hp = 0;
        bossHPBar.UpdateHPBar(damage);
        Debug.Log("ダメージ: "+ damage);

        if(hp <= 0)
        {
            anim.SetTrigger("Die");
        }
    }
}
