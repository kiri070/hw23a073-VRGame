// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.XR;
// using UnityEngine.XR.Interaction.Toolkit;

// public class CharacterCnt : MonoBehaviour
// {
//     public GameObject optionUI;
//     [HideInInspector] public bool pause = false;
//     bool previousClick;

//     //左手のRay
//     [SerializeField] XRRayInteractor leftRay;

//     CharacterController controller; // キャラクターコントローラー
//     [Range(1, 10)] public float moveSpeed = 3f; // 移動速度
//     float velocityY;

//     bool prevJump; // 押した瞬間検知用

//     //回避
//     public float sprintSpeed = 5f;
//     float velocityX;
//     bool isSprint = false;
//     float sprintCoolTime;
//     [SerializeField] Camera playerCamera;
//     Vector3 sprint_Direction;

//     //エフェクト
//     public GameObject sprintForward_Effect;
//     public GameObject sprintBack_Effect;
//     public GameObject sprintRight_Effect;
//     public GameObject sprintLeft_Effect;

//     //スナップターン用
//     float turnCooldown = 0f;
//     public float turnAngle = 30f; // 回転角度
//     public float turnDelay = 0.3f; // 連続防止

//     EXSkill exSkill;

//     Player_SoundList player_SoundList;
//     SoundManager sm;
//     public AudioSource audioSource;

//     void Start()
//     {
//         controller = GetComponent<CharacterController>();
//         exSkill = FindObjectOfType<EXSkill>();
//         player_SoundList = FindObjectOfType<Player_SoundList>();
//         sm = FindObjectOfType<SoundManager>();

//         //通常時はRayをOFF
//         leftRay.enabled = false;
//     }

//     void Update()
//     {
//         // 入力デバイスの取得
//         InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
//         InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

//         //オプション
//         bool isClick;

//         if (leftDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out isClick))
//         {
//             if (isClick && !previousClick)
//             {
//                 pause = !pause;

//                 Time.timeScale = pause ? 0f : 1f;

//                 //左手のRayをON/OFF
//                 leftRay.enabled = pause;
//             }

//             previousClick = isClick;
//         }

//         Vector3 move = Vector3.zero;

//         //必殺技中は移動しない
//         if (exSkill.isExSkill) return;

//         // 移動
//         Vector2 input;
//         if (leftDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out input))
//         {
//             Vector3 forward = Camera.main.transform.forward;
//             Vector3 right = Camera.main.transform.right;

//             forward.y = 0;
//             right.y = 0;

//             move = forward * input.y + right * input.x;
//         }

//         //右スティックでスナップターン
//         Vector2 rightInput;
//         if (rightDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightInput))
//         {
//             if (turnCooldown <= 0f)
//             {
//                 if (rightInput.x > 0.7f)
//                 {
//                     transform.Rotate(0, turnAngle, 0);
//                     turnCooldown = turnDelay;
//                 }
//                 else if (rightInput.x < -0.7f)
//                 {
//                     transform.Rotate(0, -turnAngle, 0);
//                     turnCooldown = turnDelay;
//                 }
//             }
//         }

//         // クールタイム減少
//         turnCooldown -= Time.deltaTime;

//         // ジャンプ
//         bool isJump;
//         if (rightDevice.TryGetFeatureValue(CommonUsages.primaryButton, out isJump))
//         {
//             if (isJump && !prevJump && controller.isGrounded)
//             {
//                 velocityY = 6.5f;
//             }

//             prevJump = isJump;
//         }

//         //===== 回避 ===== //
//         if (isSprint)
//         {
//             sprintCoolTime += Time.deltaTime;
//         }

//         if (sprintCoolTime >= 5f)
//         {
//             isSprint = false;
//         }

//         bool sprintInput;
//         if (leftDevice.TryGetFeatureValue(CommonUsages.triggerButton, out sprintInput))
//         {
//             if (sprintInput && !isSprint)
//             {
//                 StartCoroutine(GameSpeed(0.3f, 0.3f));
//                 velocityX = sprintSpeed;

//                 sm.OnPlaySE(audioSource, player_SoundList.sprintSound, 8f);

//                 if (input.magnitude <= 0)
//                     sprint_Direction = playerCamera.transform.forward;
//                 else
//                     sprint_Direction = move;

//                 sprintForward_Effect.SetActive(false);
//                 sprintBack_Effect.SetActive(false);
//                 sprintRight_Effect.SetActive(false);
//                 sprintLeft_Effect.SetActive(false);

//                 if (input.magnitude <= 0)
//                 {
//                     sprintForward_Effect.SetActive(true);
//                 }
//                 else if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
//                 {
//                     if (input.x > 0)
//                         sprintRight_Effect.SetActive(true);
//                     else
//                         sprintLeft_Effect.SetActive(true);
//                 }
//                 else
//                 {
//                     if (input.y > 0)
//                         sprintForward_Effect.SetActive(true);
//                     else
//                         sprintBack_Effect.SetActive(true);
//                 }

//                 StartCoroutine(StopSprintEffect());

//                 sprint_Direction.y = 0;
//                 sprint_Direction.Normalize();
//                 sprintCoolTime = 0f;
//                 isSprint = true;
//             }
//         }

//         if (isSprint)
//         {
//             controller.Move(sprint_Direction * velocityX * Time.deltaTime);

//             velocityX -= 20f * Time.deltaTime;

//             if (velocityX <= 0)
//             {
//                 velocityX = 0;
//             }
//         }

//         // 重力
//         if (controller.isGrounded && velocityY < 0)
//             velocityY = -2f;

//         velocityY += Physics.gravity.y * Time.deltaTime;

//         Vector3 finalMove = move * moveSpeed + new Vector3(0, velocityY, 0);
//         controller.Move(finalMove * Time.deltaTime);
//     }

//     IEnumerator GameSpeed(float duration, float time)
//     {
//         Time.timeScale = time;
//         yield return new WaitForSecondsRealtime(duration);
//         Time.timeScale = 1f;
//     }

//     IEnumerator StopSprintEffect()
//     {
//         yield return new WaitForSecondsRealtime(1f);

//         sprintForward_Effect.SetActive(false);
//         sprintBack_Effect.SetActive(false);
//         sprintRight_Effect.SetActive(false);
//         sprintLeft_Effect.SetActive(false);
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class CharacterCnt : MonoBehaviour
{
    public GameObject optionUI;
    [HideInInspector] public bool pause = false;
    bool previousClick;

    //左手のRay
    [SerializeField] XRRayInteractor leftRay;

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

        //通常時はRayをOFF
        leftRay.enabled = false;
    }

    void Update()
    {
        // 入力デバイスの取得
        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        //オプション
        bool isClick;

        if (leftDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out isClick))
        {
            if (isClick && !previousClick)
            {
                pause = !pause;

                Time.timeScale = pause ? 0f : 1f;

                //左手のRayをON/OFF
                leftRay.enabled = pause;
                
                optionUI.SetActive(!optionUI.activeSelf);
            }

            previousClick = isClick;
        }

        //ポーズ中は通常操作をしない
        if (pause) return;

        Vector3 move = Vector3.zero;

        //必殺技中は移動しない
        if (exSkill.isExSkill) return;

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
        if (isSprint)
        {
            sprintCoolTime += Time.deltaTime;
        }

        if (sprintCoolTime >= 5f)
        {
            isSprint = false;
        }

        bool sprintInput;
        if (leftDevice.TryGetFeatureValue(CommonUsages.triggerButton, out sprintInput))
        {
            if (sprintInput && !isSprint)
            {
                StartCoroutine(GameSpeed(0.3f, 0.3f));
                velocityX = sprintSpeed;

                sm.OnPlaySE(audioSource, player_SoundList.sprintSound, 8f);

                if (input.magnitude <= 0)
                    sprint_Direction = playerCamera.transform.forward;
                else
                    sprint_Direction = move;

                sprintForward_Effect.SetActive(false);
                sprintBack_Effect.SetActive(false);
                sprintRight_Effect.SetActive(false);
                sprintLeft_Effect.SetActive(false);

                if (input.magnitude <= 0)
                {
                    sprintForward_Effect.SetActive(true);
                }
                else if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                {
                    if (input.x > 0)
                        sprintRight_Effect.SetActive(true);
                    else
                        sprintLeft_Effect.SetActive(true);
                }
                else
                {
                    if (input.y > 0)
                        sprintForward_Effect.SetActive(true);
                    else
                        sprintBack_Effect.SetActive(true);
                }

                StartCoroutine(StopSprintEffect());

                sprint_Direction.y = 0;
                sprint_Direction.Normalize();
                sprintCoolTime = 0f;
                isSprint = true;
            }
        }

        if (isSprint)
        {
            controller.Move(sprint_Direction * velocityX * Time.deltaTime);

            velocityX -= 20f * Time.deltaTime;

            if (velocityX <= 0)
            {
                velocityX = 0;
            }
        }

        // 重力
        if (controller.isGrounded && velocityY < 0)
            velocityY = -2f;

        velocityY += Physics.gravity.y * Time.deltaTime;

        Vector3 finalMove = move * moveSpeed + new Vector3(0, velocityY, 0);
        controller.Move(finalMove * Time.deltaTime);
    }

    IEnumerator GameSpeed(float duration, float time)
    {
        Time.timeScale = time;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    IEnumerator StopSprintEffect()
    {
        yield return new WaitForSecondsRealtime(1f);

        sprintForward_Effect.SetActive(false);
        sprintBack_Effect.SetActive(false);
        sprintRight_Effect.SetActive(false);
        sprintLeft_Effect.SetActive(false);
    }


    public void ResumeGame()
    {
        pause = false;
        Time.timeScale = 1f;
        leftRay.enabled = false;
        optionUI.SetActive(false);
    }
}