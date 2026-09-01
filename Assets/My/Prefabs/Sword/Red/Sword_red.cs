using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class Sword_red : MonoBehaviour
{
    //===== エフェクト =====
    public GameObject hitEffect;
    GameObject majicEffect;
    //エンチャントレベル1
    public GameObject explosionEffect;
    public GameObject fireEffect;
    //エンチャントレベル2
    public GameObject skill2Effect;

    //エンチャントレベル3
    [Tooltip("エンチャント3の持続時間")]public float enchant03_duration = 10f;
    public GameObject additional_Sword;
    [HideInInspector] public bool isEnchant03 = false;
    Coroutine enchant03_cor = null;
    List<Transform> addtional_Sword_Pos = new List<Transform>();

    public Vector3 hitPos; //剣が当たったコライダーの位置
    Player_EXBar player_EXBar;
    EXSkill eXSkill;
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
    public Transform skill2LaunchPoint;

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

    Skill3_showTime showTime;
    void Start()
    {
        rightController = GameObject.Find("Right Controller").GetComponent<XRBaseController>();
        leftController = GameObject.Find("Left Controller").GetComponent<XRBaseController>();
        gm = FindObjectOfType<GameManager>();
        sm = FindObjectOfType<SoundManager>();
        swordSoundList = FindObjectOfType<SwordSoundList>();
        audioSource = GetComponent<AudioSource>();
        player_EXBar = FindObjectOfType<Player_EXBar>();
        eXSkill = FindObjectOfType<EXSkill>();

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

        //追撃剣のスポーン位置を取得
        for(int i = 0; i < 3; i++)
        {
            addtional_Sword_Pos.Add(GameObject.Find($"Addtional_SwordPos{i}").transform);
        }

        //スキル3の残り時間UIに参照を渡す
        showTime = FindObjectOfType<Skill3_showTime>();
        showTime.Set_Sword_red(this);

    }

    void Update()
    {
        if(golem == null)
        {
            golem = FindObjectOfType<Golem>();
        }

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

        //エンチャント3の追撃処理
        // Additional_Attack();

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
            if (!isSwinging && wasSwinging && isActivated)
            {
                ExecuteSkill();
            }
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

        wasSwinging = isSwinging; //振り終わりフラグ
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
            sm.OnPlaySE(audioSource, swordSoundList.enchant_Level3_sound02, 3f);
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
            // 剣のtipPoint - rootPointの方向を使う（槍のように刺す方向）
            Vector3 swordDir = (tipPoint.position - rootPoint.position).normalized;
            // Y軸方向を少し残す（0.5倍）
            swordDir.y *= 0.5f;
            Quaternion skillRotation = Quaternion.LookRotation(swordDir);
            // skill2LaunchPointがあれば使う、なければtipPointを使う
            Vector3 launchPos = skill2LaunchPoint != null ? skill2LaunchPoint.position : tipPoint.position;
            Instantiate(skill2Effect, launchPos, skillRotation);
            // sm.OnPlaySE(audioSource, swordSoundList.skill2Sound, 3f);
        }
        else if(enchantLevel == 3)
        {
            if(player_EXBar.ex >= 100)
            {
                Debug.Log("必殺技を発動");
                player_EXBar.UseEX(100);
                gm.StartHitStop(0.7f, 0.7f);
                StartCoroutine(eXSkill.spawnSword());
            }
            else
            {
                Debug.Log("エンチャントレベル3のスキル発動");
                showTime.StartBar(); //スキル3の残り時間UIを更新
                if (enchant03_cor != null) StopCoroutine(enchant03_cor);
                enchant03_cor = StartCoroutine(Enchant03_Calculation()); //持続時間計測

            }
            
        }
        
        isActivated = false;
        enchantLevel = 0;
    }

    // === エンチャント3の追撃処理関連 === //
    //エンチャント3の持続時間を管理するコルーチン
    IEnumerator Enchant03_Calculation()
    {
        isEnchant03 = true;
        yield return new WaitForSeconds(enchant03_duration);
        isEnchant03 = false;
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

    //当たったコライダーの位置を取得
    Vector3 GetHitPoint(Collider other)
    {
        if (tipPoint != null)
        {
            return other.ClosestPoint(tipPoint.position);
        }
        return other.ClosestPoint(transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        //石の当たり判定
        if(other.gameObject.CompareTag("Stone") && !isAttack && isSwinging && isActivated)
        {
            player_EXBar.UpdateEXBar(10); // EXゲージ
            gm.StartHitStop(0.5f, 0.2f); // ヒットストップ
            SendHaptic(1f, 0.5f, rightController);
            sm.OnPlaySE(audioSource, swordSoundList.flictionSound, 2f);
        }

        //敵
        if(other.gameObject.CompareTag("Enemy") && !isAttack && isSwinging)
        {
            isAttack = true;

            // Vector3 hitPos = GetHitPoint(other);
            hitPos = GetHitPoint(other);

            Instantiate(hitEffect, hitPos, Quaternion.identity);

            //攻撃処理
            Enemy01 enemy01 = other.GetComponent<Enemy01>();
            Enemy01_HPBar enemy01_HPBar = other.gameObject.GetComponentInChildren<Enemy01_HPBar>();
            if(enchantLevel == 0) enemy01.TakeDamage(50);
            if(enchantLevel == 1) enemy01.TakeDamage(70);
            if(enchantLevel == 2) enemy01.TakeDamage(100);
            if(enchantLevel == 3) enemy01.TakeDamage(150);

            SendHaptic(1f, 0.5f, rightController);  //振動
            gm.StartHitStop(0.7f, 0.7f); //ヒットストップ
            sm.OnPlaySE(audioSource, swordSoundList.hitSwordSound, 3f);


            if(isEnchant03)
            {
                int rnd = Random.Range(0, addtional_Sword_Pos.Count);
                
                GameObject sword = Instantiate(additional_Sword, addtional_Sword_Pos[rnd].position, Quaternion.identity);

                sword.GetComponent<Addtional_Sword>().SetTarget(other.transform);
            }   
        }

        //ボスの腕の当たり判定(ダウン時のみ)
        if(other.gameObject.CompareTag("WeakPoint") && !isAttack && isSwinging && golem.isDown) //ボスがダウンしているときのみ攻撃可能
        {

            isAttack = true;

            // Vector3 hitPos = GetHitPoint(other);
            hitPos = GetHitPoint(other);

            Instantiate(hitEffect, hitPos, Quaternion.identity);

            //攻撃処理
            if(enchantLevel == 0) golem.TakeDamage(3);
            if(enchantLevel == 1) golem.TakeDamage(6);
            if(enchantLevel == 2) golem.TakeDamage(9);
            if(enchantLevel == 3) golem.TakeDamage(12);

            SendHaptic(1f, 0.5f, rightController);  //振動
            gm.StartHitStop(0.7f, 0.7f); //ヒットストップ
            sm.OnPlaySE(audioSource, swordSoundList.hitSwordSound, 3f);

            if(isEnchant03)
            {
                int rnd = Random.Range(0, addtional_Sword_Pos.Count);

                GameObject sword = Instantiate(additional_Sword, addtional_Sword_Pos[rnd].position, Quaternion.identity);

                sword.GetComponent<Addtional_Sword>().SetTarget(hitPos);
            }   
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Enemy") || other.CompareTag("WeakPoint"))
        {
            isAttack = false;
        }
    }

}