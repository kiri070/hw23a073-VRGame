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

    //回避
    public float sprintSpeed = 5f;
    float velocityX;
    bool isSprint = false;
    float sprintCoolTime;
    [SerializeField] Camera playerCamera;
     Vector3 sprint_Direction;

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

        //===== 回避 ===== //
        //回避クール関連
        if(isSprint)
        {
            sprintCoolTime += Time.deltaTime;
        }
        if(sprintCoolTime >= 5f)
        {
            isSprint = false;
        }
        //回避ボタンを押したとき
        bool sprintInput;
        if(leftDevice.TryGetFeatureValue(CommonUsages.triggerButton, out sprintInput))
        {
            if(sprintInput && !isSprint)
            {
                StartCoroutine(GameSpeed(0.3f, 0.3f)); //ゲーム速度変更
                velocityX = sprintSpeed; //速さを代入

                //回避方向を取得
                if(input.magnitude <= 0) sprint_Direction = playerCamera.transform.forward; //移動していない時は前に回避
                else sprint_Direction = move; //移動中はスティックの方向に回避
                
                sprint_Direction.y = 0; //高さは考慮しない
                sprint_Direction.Normalize(); //方向を1に正規化
                sprintCoolTime = 0f; //クールタイムをリセット
                isSprint = true; //回避フラグをオン
            }
        }
        //回避中
        if(isSprint)
        {
            controller.Move(sprint_Direction * velocityX * Time.deltaTime); //毎フレーム回避処理

            velocityX -= 20f * Time.deltaTime; //徐々に減衰

            if(velocityX <= 0)
            {
                velocityX = 0;
            }
        }   
        // ===== ///

        // 重力
        if (controller.isGrounded && velocityY < 0)
            velocityY = -2f;

        velocityY += Physics.gravity.y * Time.deltaTime;
        
        Vector3 finalMove = move * moveSpeed + new Vector3(0, velocityY, 0);
        controller.Move(finalMove * Time.deltaTime);
    }

    /// <summary>
    /// ゲーム速度を変える関数
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator GameSpeed(float duration, float time)
    {
        Time.timeScale = time;
        yield return new WaitForSecondsRealtime(duration); //実際の時間待つ
        Time.timeScale = 1f;
    }
}