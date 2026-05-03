using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CharacterCnt : MonoBehaviour
{
    CharacterController controller; // キャラクターコントローラー
    [Range(1, 10)] public float moveSpeed = 3f; // 移動速度
    float velocityY;

    bool prevJump; // 押した瞬間検知用

    //スナップターン用
    float turnCooldown = 0f;
    public float turnAngle = 30f; // 回転角度
    public float turnDelay = 0.3f; // 連続防止

    EXSkill exSkill;
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        exSkill = FindObjectOfType<EXSkill>();
    }

    void Update()
    {
        // 入力デバイスの取得
        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        Vector3 move = Vector3.zero;

        //必殺技中は移動しない
        if(exSkill.isExSkill) return;

        // 移動
        Vector2 input;
        if (leftDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out input))
        {
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;

            forward.y = 0;
            right.y = 0;

            move = forward * input.y + right * input.x;
        }

        //右スティックでスナップターン
        Vector2 rightInput;
        if (rightDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightInput))
        {
            if (turnCooldown <= 0f)
            {
                if (rightInput.x > 0.7f)
                {
                    transform.Rotate(0, turnAngle, 0);
                    turnCooldown = turnDelay;
                }
                else if (rightInput.x < -0.7f)
                {
                    transform.Rotate(0, -turnAngle, 0);
                    turnCooldown = turnDelay;
                }
            }
        }

        // クールタイム減少
        turnCooldown -= Time.deltaTime;

        // ジャンプ
        bool isJump;
        if (rightDevice.TryGetFeatureValue(CommonUsages.primaryButton, out isJump))
        {
            if (isJump && !prevJump && controller.isGrounded)
            {
                velocityY = 6.5f;
            }

            prevJump = isJump;
        }

        // 重力
        if (controller.isGrounded && velocityY < 0)
            velocityY = -2f;

        velocityY += Physics.gravity.y * Time.deltaTime;

        Vector3 finalMove = move * moveSpeed + new Vector3(0, velocityY, 0);
        controller.Move(finalMove * Time.deltaTime);
    }
}