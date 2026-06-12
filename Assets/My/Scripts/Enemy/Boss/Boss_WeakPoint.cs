using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_WeakPoint : MonoBehaviour
{
    Golem golem; //ボスのスクリプト
    SoundManager soundManager; //サウンドマネージャーのスクリプト
    Boss_SoundList bossSoundList; //ボスのサウンドリスト
    AudioSource audioSource; //オーディオソース

    void Start()
    {
        golem = FindObjectOfType<Golem>(); //親オブジェクトからGolemスクリプトを取得
        soundManager = FindObjectOfType<SoundManager>(); //シーン内のSoundManagerを探して取得
        bossSoundList = FindObjectOfType<Boss_SoundList>(); //GolemスクリプトからBossSoundListを取得
        audioSource = golem.GetComponent<AudioSource>(); //GolemスクリプトからAudioSourceを取得
    }

    void OnTriggerEnter(Collider other)
    {
        //スキル１の当たり判定（ダウン中のみ）エフェクトなどはあとで変更
        if (other.gameObject.CompareTag("Skill") && golem.isDown)
        {
            soundManager.OnPlaySE(audioSource, bossSoundList.damageSound);
            Instantiate(golem.skill2_DownEffect, other.gameObject.transform.position, golem.skill2_DownEffect.transform.rotation);
            golem.TakeDamage(10);
        }
    }
}
