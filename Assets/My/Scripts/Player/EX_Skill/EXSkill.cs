using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXSkill : MonoBehaviour
{
    [Tooltip("剣のスポーン位置")]
    public List<Transform> sword_SpawnPos = new List<Transform>();

    [Tooltip("剣のPrefab")]
    public List<GameObject> sword_Prefabs = new List<GameObject>();

    [Tooltip("剣のヒットエフェクト")]
    public GameObject hitEffect01;

    [HideInInspector] public bool isExSkill = false;

    [Tooltip("剣が向かうボス")]
    public GameObject bossObj;

    [Tooltip("剣を1本ずつ召喚する間隔")]
    public float spawnInterval = 1f;

    [Tooltip("剣が動き出すまでの待ち時間")]
    public float swordLaunchDelay = 2f;

    [Tooltip("剣が飛んでいく速さ")]
    public float swordFlySpeed = 15f;

    [Tooltip("この距離まで近づいたら命中扱いにする")]
    public float swordHitDistance = 0.2f;

    [Tooltip("命中時に与えるダメージ。0ならダメージなし")]
    public int swordDamage = 0;

    [Tooltip("ボスの位置からどれくらいずらして狙うか")]
    public Vector3 bossTargetOffset = new Vector3(0f, 1.5f, 0f);

    SoundManager sm;
    AudioSource audioSource;
    public AudioClip spawnSound;
    public AudioClip hitSound;

    void Start()
    {
        sm = FindObjectOfType<SoundManager>();
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 0f;
    }

    public IEnumerator spawnSword()
    {
        isExSkill = true;

        int spawnCount = Mathf.Min(sword_SpawnPos.Count, sword_Prefabs.Count);

        for(int i = 0; i < spawnCount; i++)
        {
            GameObject sword = Instantiate(
                sword_Prefabs[i],
                sword_SpawnPos[i].position,
                sword_Prefabs[i].transform.rotation);

            PlaySpawnSound();
            StopSwordPhysics(sword);
            StartCoroutine(LaunchSwordToBoss(sword));

            yield return new WaitForSeconds(spawnInterval);
        }

        isExSkill = false;
    }

    IEnumerator LaunchSwordToBoss(GameObject sword)
    {
        if(sword == null) yield break;

        yield return new WaitForSeconds(swordLaunchDelay);

        if(sword == null) yield break;

        GameObject target = GetBossTarget();
        if(target == null) yield break;

        SetMoveEffectActive(sword, true);

        while(sword != null && target != null)
        {
            Vector3 targetPos = GetTargetPosition(target);
            Vector3 direction = targetPos - sword.transform.position;

            if(direction.magnitude <= swordHitDistance)
            {
                sword.transform.position = targetPos;

                if(hitEffect01 != null)
                {
                    Instantiate(hitEffect01, new Vector3(
                        target.transform.position.x,
                        target.transform.position.y + 20f,
                        target.transform.position.z - 20f), hitEffect01.transform.rotation);
                }

                PlayHitSound(targetPos);
                DamageBoss(target);
                Destroy(sword);
                yield break;
            }

            // Prefabの回転値を保ったまま、位置だけボスへ近づける
            sword.transform.position = Vector3.MoveTowards(
                sword.transform.position,
                targetPos,
                swordFlySpeed * Time.deltaTime);

            yield return null;
        }
    }

    void StopSwordPhysics(GameObject sword)
    {
        if(sword == null) return;

        Rigidbody rb = sword.GetComponent<Rigidbody>();
        if(rb == null) return;

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void SetMoveEffectActive(GameObject sword, bool active)
    {
        if(sword == null) return;

        Transform[] children = sword.GetComponentsInChildren<Transform>(true);
        foreach(Transform child in children)
        {
            if(child.name == "MoveEffect")
            {
                child.gameObject.SetActive(active);
                return;
            }
        }
    }

    void PlayHitSound(Vector3 position)
    {
        if(hitSound == null) return;

        float volume = sm != null ? sm.seVolume * 5f : 5f;
        audioSource.PlayOneShot(hitSound, volume);
    }

    void PlaySpawnSound()
    {
        if(spawnSound == null) return;

        float volume = sm != null ? sm.seVolume * 5f : 5f;
        audioSource.PlayOneShot(spawnSound, volume);
    }

    GameObject GetBossTarget()
    {
        if(bossObj != null) return bossObj;

        Golem golem = FindObjectOfType<Golem>();
        if(golem == null) return null;

        return golem.gameObject;
    }

    Vector3 GetTargetPosition(GameObject target)
    {
        return target.transform.position + bossTargetOffset;
    }

    void DamageBoss(GameObject target)
    {
        if(swordDamage <= 0) return;

        Golem golem = target.GetComponent<Golem>();
        if(golem == null)
        {
            golem = target.GetComponentInParent<Golem>();
        }

        if(golem != null)
        {
            if(golem.hp <= 0) return;

            golem.TakeDamage(swordDamage);
            if(golem.hp > 0)
            {
                golem.Damage();
            }
        }
    }
}
