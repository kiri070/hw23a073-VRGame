using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Body : MonoBehaviour
{
    Player_HPBar player_HPBar;
    // === プレイヤー情報 === //
    [HideInInspector] public int hp = 100;
    public float invincibleTime = 0.5f;
    bool invincible = false;

    CharacterController controller;
    public Transform playerSpawnPos;
    //ノックバック関連
    Vector3 knockbackVelocity;
    float knockbackTime = 0.2f;
    bool isKnockback = false;

    float lastY;

    //サウンド
    SoundManager sm;
    Player_SoundList player_SoundList;
    AudioSource audioSource;

    GameManager gm;

    void Start()
    {
        lastY = Camera.main.transform.eulerAngles.y;
        controller = GetComponentInParent<CharacterController>();

        sm = FindObjectOfType<SoundManager>();
        player_SoundList = GetComponent<Player_SoundList>();
        audioSource = GetComponent<AudioSource>();

        gm = FindObjectOfType<GameManager>();
        player_HPBar = FindObjectOfType<Player_HPBar>();
    }

    void Update()
    {
        // Vector3 headPos = Camera.main.transform.position;

        // // 位置追従
        // transform.position = new Vector3(headPos.x, transform.position.y, headPos.z);

        // float currentY = Camera.main.transform.eulerAngles.y;

        // // 回転差を計算
        // float diff = Mathf.Abs(Mathf.DeltaAngle(lastY, currentY));

        // // 一定以上向き変わったら回転
        // if (diff > 10f)
        // {
        //     transform.rotation = Quaternion.Euler(0, currentY, 0);
        //     lastY = currentY;
        // }

        //テスト(位置ずれ起きなさそう)
        controller.center = new Vector3(
        Camera.main.transform.localPosition.x,
        controller.center.y,
        Camera.main.transform.localPosition.z
    );
    }

    // === ノックバック処理 === //
    public void Knockback(Vector3 enemyPos)
    {
        Vector3 dir = (transform.position - enemyPos).normalized;

        knockbackVelocity = dir * 5f + Vector3.up * 2f;

        StopAllCoroutines();
        StartCoroutine(KnockbackRoutine());
    }
    IEnumerator KnockbackRoutine()
    {
        isKnockback = true;

        float t = 0;

        while (t < knockbackTime)
        {
            controller.Move(knockbackVelocity * Time.deltaTime);

            knockbackVelocity *= 1.2f; // 減速

            t += Time.deltaTime;
            yield return null;
        }

        isKnockback = false;
    }


    void OnTriggerEnter(Collider other)
    {
        //Enemy01のパンチ攻撃
        if(other.CompareTag("Enemy01_Punch"))
        {
            TakeDamage(10);
            Knockback(other.transform.position); //ノックバック処理
            gm.Shake(0.7f, 0.7f, 40, 90); //カメラを揺らす

            //無敵時間
            StartCoroutine(Calculation_InvisibleTime());
        }
        //Bossの石に当たった時
        if(other.CompareTag("Stone"))
        {
            TakeDamage(10);
            //無敵時間
            StartCoroutine(Calculation_InvisibleTime());
        }
        //落下判定
        if(other.CompareTag("DeathGround"))
        {
            TakeDamage(10);
            controller.enabled = false;
            controller.transform.position = playerSpawnPos.position;
            controller.enabled = true;
        }
    }

    //無敵時間の管理
    IEnumerator Calculation_InvisibleTime()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibleTime);
        invincible = false;
    }


    //プレイヤーが攻撃を受ける関数
    public void TakeDamage(int damage)
    {
        //無敵状態なら実行しない
        if(invincible) return;

        hp -= damage;
        if(hp < 0) hp = 0;
        
        sm.OnPlaySE(audioSource, player_SoundList.punchDamageSound, 3f);
        player_HPBar.UpdateHPBar(damage); //HPバー更新
        Debug.Log("プレイヤーHP:" + hp);

        //hpが0以下ならゲームオーバー
        if(hp <= 0)
        {
            gm.gameOver = true;
        }
    }
}