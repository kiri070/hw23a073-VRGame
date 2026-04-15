using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Sword_red : MonoBehaviour
{
    public GameObject explosionEffect;

    XRBaseController rightController;
    Transform leftHand;

    public Transform rootPoint;
    public Transform tipPoint;

    GameObject effect;

    Vector3 prevLeftPos;
    Quaternion prevRot;
    Vector3 prevSwordPos;

    float detectDistance = 0.4f;
    float moveThreshold = 0.01f;

    float traceTime = 0f;
    float traceThreshold = 0.1f;

    float cooldown = 0f;

    bool isActivated = false;

    // 🔥 追加（振り状態管理）
    bool wasSwinging = false;

    void Start()
    {
        rightController = GameObject.Find("Right Controller").GetComponent<XRBaseController>();
        leftHand = GameObject.Find("Left Controller").transform;
        effect = transform.Find("Effect_Lightning").gameObject;

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

            // 🔥 振り終わりで発動
            if (!isSwinging && wasSwinging)
            {
                ExecuteSkill();
            }

            wasSwinging = isSwinging;
        }
        else
        {
            effect.SetActive(false);
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
        Debug.Log("なぞり成功！");
        isActivated = true;
        cooldown = 0.3f;
    }

    // ===== エンチャント中処理 =====
    void UpdateEnchant()
    {
        effect.SetActive(true);
        SendHaptic(0.2f, 0.05f);
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

        Instantiate(explosionEffect, tipPoint.position, Quaternion.identity);

        isActivated = false;
    }

    // ===== 前フレーム更新 =====
    void UpdatePreviousState()
    {
        prevLeftPos = leftHand.position;
        prevRot = transform.rotation;
        prevSwordPos = transform.position;
    }

    // ===== 振動 =====
    void SendHaptic(float amplitude, float duration)
    {
        if (rightController != null)
        {
            rightController.SendHapticImpulse(amplitude, duration);
        }
    }
}