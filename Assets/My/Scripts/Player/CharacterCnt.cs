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
    //エフェクト
    public GameObject sprintForward_Effect;
    public GameObject sprintBack_Effect;
    public GameObject sprintRight_Effect;
    public GameObject sprintLeft_Effect;

    //スナップターン用
    float turnCooldown = 0f;
    public float turnAngle = 30f; // 回転角度
    public float turnDelay = 0.3f; // 連続防止

    EXSkill exSkill;

    Player_SoundList player_SoundList;
    SoundManager sm;
    public AudioSource audioSource;
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        exSkill = FindObjectOfType<EXSkill>();
        player_SoundList = FindObjectOfType<Player_SoundList>();
        sm = FindObjectOfType<SoundManager>();
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

                //効果音
                sm.OnPlaySE(audioSource, player_SoundList.sprintSound, 8f);

                //回避方向を取得
                if(input.magnitude <= 0) sprint_Direction = playerCamera.transform.forward; //移動していない時は前に回避
                else sprint_Direction = move; //移動中はスティックの方向に回避

                //エフェクト
                sprintForward_Effect.SetActive(false);
                sprintBack_Effect.SetActive(false);
                sprintRight_Effect.SetActive(false);
                sprintLeft_Effect.SetActive(false);

                if (input.magnitude <= 0)
                {
                    sprintForward_Effect.SetActive(true);
                }
                //横方向
                else if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                {
                    if (input.x > 0) sprintRight_Effect.SetActive(true); //右
                    else sprintLeft_Effect.SetActive(true); //左
                }
                //縦方向
                else
                {
                    if (input.y > 0) sprintForward_Effect.SetActive(true); //前
                    else sprintBack_Effect.SetActive(true); //後
                }

                StartCoroutine(StopSprintEffect());

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

    IEnumerator StopSprintEffect()
    {
        yield return new WaitForSecondsRealtime(1f);
        //回避エフェクトをオフ
        sprintForward_Effect.SetActive(false);
        sprintBack_Effect.SetActive(false);
        sprintRight_Effect.SetActive(false);
        sprintLeft_Effect.SetActive(false);
    }
}