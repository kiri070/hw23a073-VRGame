using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Sword_red : MonoBehaviour
{
    //===== エフェクト =====
    public GameObject hitEffect;
    //エンチャントレベル1
    public GameObject explosionEffect;
    public GameObject fireEffect;
    //エンチャントレベル2

    //エンチャントレベル3


    XRBaseController rightController;
    XRBaseController leftController;
    Transform leftHand;

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

    bool isActivated = false;

    //振り状態管理
    bool wasSwinging = false;

    //エンチャントレベル
    int enchantLevel = 0;

    bool isAttack = false;

    void Start()
    {
        rightController = GameObject.Find("Right Controller").GetComponent<XRBaseController>();
        leftController = GameObject.Find("Left Controller").GetComponent<XRBaseController>();
        leftHand = GameObject.Find("Left Controller").transform;
        effect_level1 = transform.Find("Effect_Level1").gameObject;
        effect_level2 = transform.Find("Effect_Level2").gameObject;
        effect_level3 = transform.Find("Effect_Level3").gameObject;

        prevLeftPos = leftHand.position;
        prevRot = transform.rotation;
        prevSwordPos = transform.position;
    }

    void Update()
    {
        cooldown -= Time.deltaTime;

        bool traced = CheckTrace();

        if (traced && cooldown <= 0f)
        {
            ActivateEnchant();
        }

        if (isActivated)
        {
            UpdateEnchant();

            // ===== 振り判定 =====
            bool isSwinging = CheckSwing();

            // 振り終わりで発動
            if (!isSwinging && wasSwinging)
            {
                ExecuteSkill();
            }

            wasSwinging = isSwinging;
        }
        else
        {
            effect_level1.SetActive(false);
            effect_level2.SetActive(false);
            effect_level3.SetActive(false);
        }

        UpdatePreviousState();
    }

    // ===== なぞり判定 =====
    bool CheckTrace()
    {
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
        // エンチャントレベルを上げる
        if(enchantLevel <= 3)
        {
            enchantLevel++;
        }
        // エンチャントレベルに応じた振動
        if(enchantLevel == 1)
        {
            Debug.Log("エンチャントレベル: " + enchantLevel);
            SendHaptic(0.5f, 1f, leftController);
        }
        else if(enchantLevel == 2)
        {
            Debug.Log("エンチャントレベル: " + enchantLevel);
            SendHaptic(0.8f, 1f, leftController);
        }
        else if(enchantLevel == 3)
        {
            Debug.Log("エンチャントレベル: " + enchantLevel);
            SendHaptic(1f, 1f, leftController);
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
            SendHaptic(0.2f, 0.05f, rightController);
        }
        else if(enchantLevel == 2)
        {
            effect_level1.SetActive(false);
            effect_level2.SetActive(true);
            effect_level3.SetActive(false);
            SendHaptic(0.5f, 0.05f, rightController);
        }
        else if(enchantLevel == 3)
        {
            effect_level1.SetActive(false);
            effect_level2.SetActive(false);
            effect_level3.SetActive(true);
            SendHaptic(1f, 0.05f, rightController);
        }
        // effect.SetActive(true);
        // SendHaptic(0.2f, 0.05f, rightController);
    }

    // ===== 振り判定 =====
    bool CheckSwing()
    {
        float angle = Quaternion.Angle(prevRot, transform.rotation);

        Vector3 swordMove = transform.position - prevSwordPos;
        float moveAmount = swordMove.magnitude;

        return (angle > 10f && moveAmount > 0.05f) || moveAmount > 0.15f;
    }

    // ===== スキル発動 =====
    void ExecuteSkill()
    {
        Debug.Log("スキル発動！");
        if(enchantLevel == 1)
        {
            Debug.Log("エンチャントレベル1のスキル発動");
            // Instantiate(fireEffect, tipPoint.position, fireEffect.transform.rotation);
            Quaternion randomRot = Random.rotation;

            Instantiate(fireEffect, tipPoint.position, randomRot);
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
        enchantLevel = 0; // エンチャントレベルリセット
    }

    // ===== 前フレーム更新 =====
    void UpdatePreviousState()
    {
        prevLeftPos = leftHand.position;
        prevRot = transform.rotation;
        prevSwordPos = transform.position;
    }

    // ===== 振動 =====
    void SendHaptic(float amplitude, float duration, XRBaseController controller)
    {
        if (controller != null)
        {
            controller.SendHapticImpulse(amplitude, duration);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        //===エフェクトが出ない不具合===
        // 敵に当たったときの処理
        if(other.gameObject.CompareTag("Enemy") && !isAttack)
        {
            // 接触点を取得
            ContactPoint contact = other.contacts[0];
            // 接触点にエフェクトを生成
            Vector3 hitPos = contact.point;

            Instantiate(hitEffect, hitPos, Quaternion.identity);
            isActivated = false;
            enchantLevel = 0;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            isAttack = false;
        }
    }
}