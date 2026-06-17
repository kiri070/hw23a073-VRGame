using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor.EditorTools;
using UnityEngine;

public class Addtional_Sword : MonoBehaviour
{
    Sword_red sword_Red; //剣のスクリプト
    Vector3 hitPos; //当たったコライダーの位置
    [SerializeField] float delayTime = 0.2f; //攻撃までの時間
    [SerializeField] float speed = 3f; //速さ
    Transform target;
    Vector3 targetPos;
    bool usePositionTarget;

    //エフェクト
    public GameObject hitEffect;

    //音声
    AudioSource audioSource;
    SoundManager sm;
    [Tooltip("ヒットサウンド")]public AudioClip hitSE01;
    [Tooltip("ヒットサウンド")]public AudioClip hitSE02;
    [Tooltip("ヒットサウンド")]public AudioClip hitSE03;
    [Tooltip("召喚サウンド")] public AudioClip spawnSE;
    
    void Start()
    {
        sword_Red = FindObjectOfType<Sword_red>();
        sm = FindObjectOfType<SoundManager>();
        audioSource = GetComponent<AudioSource>();

        sm.OnPlaySE(audioSource, spawnSE, 8f);
        StartCoroutine(Attack());
    }

    //外部から剣を飛ばす位置を設定
    //敵
    public void SetTarget(Transform enemy)
    {
        target = enemy;
        usePositionTarget = false;
    }
    //ボスの弱点コライダー
    public void SetTarget(Vector3 pos)
    {
        targetPos = pos;
        usePositionTarget = true;
    }

    IEnumerator Attack()
    {
        hitPos = sword_Red.hitPos; 
        yield return new WaitForSeconds(delayTime); // 指定した時間待機


        while (true)
        {
            Vector3 destination;

            //弱点コライダー
            if(usePositionTarget)
            {
                destination = targetPos;
            }
            //敵
            else
            {
                if(target == null) yield break;
                destination = target.position;
            }

            Vector3 dir = (destination - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(90, 0, 0); //剣先を向ける

            //移動
            transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            speed * Time.deltaTime);

            //近くまで来たら
            if(Vector3.Distance(transform.position, destination) < 0.1f)
            {
                break;
            }

            yield return null;
        }
        
    }

    //ヒットサウンドを鳴らす関数
    void Random_HitSound()
    {
        int rnd = Random.Range(1, 4);

        switch(rnd)
        {
            case 1:
                sm.OnPlaySE(this.audioSource, hitSE01, 5f);
            break;
            case 2:
                sm.OnPlaySE(this.audioSource, hitSE02, 5f);
            break;
            case 3:
                sm.OnPlaySE(this.audioSource, hitSE03, 5f);
            break;
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("WeakPoint"))
        {
            Random_HitSound();
            Instantiate(hitEffect, targetPos, hitEffect.transform.rotation);
            Destroy(gameObject, 2f);
        }
        if(other.gameObject.CompareTag("Enemy"))
        {
            Enemy01 enemy01 = other.GetComponent<Enemy01>();
            enemy01.TakeDamage(50);
            Random_HitSound();
            Instantiate(hitEffect, new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y + 1.5f, other.gameObject.transform.position.z), hitEffect.transform.rotation);
        }
        if(other.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject, 2f);
        }
    }


}
