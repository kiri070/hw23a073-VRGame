using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stone : MonoBehaviour
{
    SoundManager soundManager; // サウンドマネージャー
    Stone_SoundList stoneSoundList; // 石のサウンドリスト
    AudioSource audioSource; // オーディオソースコンポーネント

    Rigidbody rb;
    GameObject player;

    public float speed = 3f;        // 横のスピード（遅め）
    public float arcHeight = 2f;    // 山なりの高さ
    public float offsetRange = 1.5f; // 少しズラす

    Vector3 targetPos;

    bool isHitSword = false; //剣に跳ね返されたか
    Transform boss_Pos; //ボスの位置を取得するための変数

    bool isReachedPlayer = false; // 追加
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        stoneSoundList = FindObjectOfType<Stone_SoundList>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.Find("Player").transform.Find("Body").gameObject;
        boss_Pos = GameObject.Find("GolemPrefab").transform; //ボスの位置を取得
        rb = GetComponent<Rigidbody>();

        // プレイヤー付近に少しズラす
        Vector3 offset = new Vector3(
            Random.Range(-offsetRange, offsetRange),
            0,
            Random.Range(-offsetRange, offsetRange)
        );

        targetPos = player.transform.position + offset;
    }

    void Update()
    {
        if (isHitSword)
        {
            Vector3 dir = (boss_Pos.position - transform.position).normalized;
            Vector3 velocity = dir * speed * 5f + Vector3.up * arcHeight * 8f;
            rb.velocity = velocity;
        }
        else
        {
            // プレイヤーに到達したら落下モード
            if (!isReachedPlayer)
            {
                float distance = Vector3.Distance(transform.position, targetPos);

                if (distance < 0.5f)
                {
                    isReachedPlayer = true;
                    return; // このフレームは何もしない
                }

                Vector3 dir = (targetPos - transform.position).normalized;
                Vector3 velocity = dir * speed + Vector3.up * arcHeight;
                rb.velocity = velocity;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword"))
        {
            //剣を振っていたら
            Sword_red sword_Red = other.gameObject.GetComponent<Sword_red>();
            //剣が振られていて、かつスキルが発動している（エンチャントの状態）なら跳ね返る
            if(sword_Red != null && sword_Red.isSwinging && sword_Red.isActivated)
            {
                isHitSword = true;
                rb.velocity = Vector3.zero; // 一旦速度をリセット
            }
        }
        if(other.gameObject.CompareTag("Boss"))
        {
            Debug.Log("Bossに当たった");
            Destroy(gameObject); // 石を消す
        }
        if(other.gameObject.CompareTag("Floor"))
        {
            soundManager.OnPlaySE(audioSource, stoneSoundList.impactSound01, 2f);
            StartCoroutine(DestroyAfterSound(stoneSoundList.impactSound01.length));
        }
    }

    // サウンドが再生された後にオブジェクトを破壊するコルーチン
    IEnumerator DestroyAfterSound(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
