using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using JetBrains.Annotations;

public class Sword_red : MonoBehaviour
{
    //===== エフェクト =====
    public GameObject hitEffect;
    GameObject majicEffect;
    //エンチャントレベル1
    public GameObject explosionEffect;
    public GameObject fireEffect;
    //エンチャントレベル2

    //エンチャントレベル3


    Golem golem;
    [HideInInspector] public XRBaseController rightController;
    [HideInInspector] public XRBaseController leftController;
    Transform leftHand;
    GameManager gm;
    SoundManager sm;
    SwordSoundList swordSoundList;
    AudioSource audioSource;
    public Transform rootPoint;
    public Transform tipPoint;

    GameObject effect_level1;
    GameObject effect_level2;
    GameObject effect_level3;

    Vector3 prevLeftPos;
    Quaternion prevRot;
    Vector3 prevSwordPos;

    float detectDistance = 0.4f;
    float moveThreshold = 0.01f;

    float traceTime = 0f;
    float traceThreshold = 0.1f;

    float cooldown = 0f;

    [HideInInspector] public bool isActivated = false;

    //振り状態管理(外部ではisSwingingとisActivatedで振ったかどうか管理する)
    [HideInInspector] public bool wasSwinging = false;
    [HideInInspector] public bool isSwinging = false;

    //エンチャントレベル
    [HideInInspector] public int enchantLevel = 0;
    [HideInInspector] public int currentSkillLevel = 0;

    bool isAttack = false;

    //プレイヤー移動量を計算
    Transform head;
    Vector3 prevHeadPos;
    float headSpeed;
    void Start()
    {
        rightController = GameObject.Find("Right Controller").GetComponent<XRBaseController>();
        leftController = GameObject.Find("Left Controller").GetComponent<XRBaseController>();
        gm = FindObjectOfType<GameManager>();
        sm = FindObjectOfType<SoundManager>();
        swordSoundList = FindObjectOfType<SwordSoundList>();
        audioSource = GetComponent<AudioSource>();
        golem = FindObjectOfType<Golem>();
        leftHand = GameObject.Find("Left Controller").transform;
        effect_level1 = transform.Find("Effect_Level1").gameObject;
        effect_level2 = transform.Find("Effect_Level2").gameObject;
        effect_level3 = transform.Find("Effect_Level3").gameObject;

        prevLeftPos = leftHand.position;
        prevRot = transform.rotation;
        prevSwordPos = transform.position;

        //頭の位置を代入
        head = Camera.main.transform;
        prevHeadPos = head.position;
        majicEffect = GameObject.Find("MajicEffect");
    }

    void Update()
    {

        // headSpeed = (head.position - prevHeadPos).magnitude / Time.deltaTime; //プレイヤーの移動量を計算
        Vector3 move = head.position - prevHeadPos;
        move.y = 0f; //上下・回転の影響カット

        headSpeed = move.magnitude / Time.deltaTime;
        //止まっているときは魔方陣をオン
        if (headSpeed < 1f)
        {
            majicEffect.SetActive(true);
        }
        else
        {
            majicEffect.SetActive(false);
        }

        cooldown -= Time.deltaTime;

        // ===== 剣の振り判定 =====
        InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        Vector3 velocity;

        if (rightDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out velocity))
        {
            float speed = velocity.magnitude;
            isSwinging = speed > 3f; // 振りとみなす速度の閾値
        }
        else
        {
            isSwinging = false;
        }

        // ===============================================

        bool traced = CheckTrace();

        if (traced && cooldown <= 0f)
        {
            ActivateEnchant();
        }

        // スキル発動中の処理
        if (isActivated) 
        {
            UpdateEnchant();

            //振り終わりで発動
            // if (!isSwinging && wasSwinging ) 
            if (!isSwinging && wasSwinging && isActivated)
            {
                ExecuteSkill();
            }

            wasSwinging = isSwinging;
        }
        // スキル非発動中はエフェクトを消す
        else 
        {
            effect_level1.SetActive(false);
            effect_level2.SetActive(false);
            effect_level3.SetActive(false);
        }
        UpdatePreviousState();




        prevHeadPos = head.position;
    }

    // ===== なぞり判定 =====
    bool CheckTrace()
    {

        //プレイヤーの移動量を代入
        Vector3 headMove = head.position - prevHeadPos;
        headMove.y = 0f; // ★追加

        //動いてたらエンチャント不可
        if (headMove.magnitude > 0.02f)
        {
            traceTime = 0f;
            return false;
        }


        Vector3 currentLeftPos = leftHand.position;
        Vector3 move = currentLeftPos - prevLeftPos;

        float speed = move.magnitude;
        float dist = Vector3.Distance(currentLeftPos, transform.position);

        Vector3 swordDir = (tipPoint.position - rootPoint.position).normalized;
        Vector3 moveDir = move.normalized;

        float dot = Vector3.Dot(moveDir, swordDir);

        if (dist < detectDistance && speed > moveThreshold && dot > 0.5f)
        {
            traceTime += Time.deltaTime;
        }
        else
        {
            traceTime = 0f;
        }

        return traceTime > traceThreshold;
    }

    // ===== エンチャント開始 =====
    void ActivateEnchant()
    {
        if(enchantLevel <= 3)
        {
            enchantLevel++;
        }

        if(enchantLevel == 1)
        {
            Debug.Log("エンチャントレベル: " + enchantLevel);
            SendHaptic(0.5f, 1f, leftController);
            SendHaptic(0.5f, 1f, rightController);

            //エンチャント音声
            sm.OnPlaySE(audioSource, swordSoundList.enchant_Level1_sound, 3f);
        }
        else if(enchantLevel == 2)
        {
            Debug.Log("エンチャントレベル: " + enchantLevel);
            SendHaptic(0.8f, 1f, leftController);
            SendHaptic(0.8f, 1f, rightController);

            //エンチャント音声
            sm.OnPlaySE(audioSource, swordSoundList.enchant_Level2_sound, 3f);
        }
        else if(enchantLevel == 3)
        {
            Debug.Log("エンチャントレベル: " + enchantLevel);
            SendHaptic(1f, 1f, leftController);
            SendHaptic(1f, 1f, rightController);

            //エンチャント音声
            sm.OnPlaySE(audioSource, swordSoundList.enchant_Level3_sound, 3f);
        }

        Debug.Log("なぞり成功！");
        isActivated = true;
        cooldown = 0.3f;
    }

    // ===== エンチャント中処理 =====
    void UpdateEnchant()
    {
        if(enchantLevel == 1)
        {
            effect_level1.SetActive(true);
            effect_level2.SetActive(false);
            effect_level3.SetActive(false);
            // SendHaptic(0.2f, 0.05f, rightController);
        }
        else if(enchantLevel == 2)
        {
            effect_level1.SetActive(false);
            effect_level2.SetActive(true);
            effect_level3.SetActive(false);
            // SendHaptic(0.5f, 0.05f, rightController);
        }
        else if(enchantLevel == 3)
        {
            effect_level1.SetActive(false);
            effect_level2.SetActive(false);
            effect_level3.SetActive(true);
            // SendHaptic(1f, 0.05f, rightController);
        }
    }

    // ===== スキル発動 =====
    void ExecuteSkill()
    {
        Debug.Log("スキル発動！");

        currentSkillLevel = enchantLevel; //現在のエンチャントレベルを保存

        if(enchantLevel == 1)
        {
            Debug.Log("エンチャントレベル1のスキル発動");
            Quaternion randomRot = Random.rotation;
            Instantiate(fireEffect, tipPoint.position, randomRot);
            sm.OnPlaySE(audioSource, swordSoundList.skill1Sound, 3f);
        }
        else if(enchantLevel == 2)
        {
            Debug.Log("エンチャントレベル2のスキル発動");
        }
        else if(enchantLevel == 3)
        {
            Debug.Log("エンチャントレベル3のスキル発動");
        }
        
        isActivated = false;
        enchantLevel = 0;
    }

    // ===== 前フレーム更新 =====
    void UpdatePreviousState()
    {
        prevLeftPos = leftHand.position;
        prevRot = transform.rotation;
        prevSwordPos = transform.position;
    }

    // ===== 振動 =====
    public void SendHaptic(float amplitude, float duration, XRBaseController controller)
    {
        if (controller != null)
        {
            controller.SendHapticImpulse(amplitude, duration);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //石の当たり判定
        if(other.gameObject.CompareTag("Stone") && !isAttack && isSwinging && isActivated)
        {
            StartCoroutine(gm.HitStop(0.5f, 0.2f)); // ヒットストップ
            SendHaptic(1f, 0.5f, rightController);
            sm.OnPlaySE(audioSource, swordSoundList.flictionSound, 2f);
        }

        //敵
        if(other.gameObject.CompareTag("Enemy") && !isAttack && isSwinging)
        {
            isAttack = true;

            Vector3 hitPos = other.ClosestPoint(transform.position);

            Instantiate(hitEffect, hitPos, Quaternion.identity);

            //攻撃処理
            Enemy01 enemy01 = other.GetComponent<Enemy01>();
            Enemy01_HPBar enemy01_HPBar = other.gameObject.GetComponentInChildren<Enemy01_HPBar>();
            if(enchantLevel == 0) enemy01.TakeDamage(50);
            if(enchantLevel == 1) enemy01.TakeDamage(70);
            if(enchantLevel == 2) enemy01.TakeDamage(100);
            if(enchantLevel == 3) enemy01.TakeDamage(150);

            SendHaptic(1f, 0.5f, rightController);  //振動
            StartCoroutine(gm.HitStop(0.7f, 0.7f)); //ヒットストップ
            sm.OnPlaySE(audioSource, swordSoundList.hitSwordSound, 3f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            isAttack = false;
        }
    }

}