using UnityEngine;
using UnityEngine.XR;
using DG.Tweening;


/// <summary>
/// 左コントローラーの入力に応じてステータス画面を表示、非表示
/// </summary>
public class LeftControllerUITrigger : MonoBehaviour
{
    [Header("表示/非表示する UI コンテナ")]
    [SerializeField] GameObject uiContainer;

    [Header("押しっぱなしで表示するトリガー閾値")]
    [SerializeField] float triggerThreshold = 0.1f;

    bool isShownByHold;
    Tween statusUI_Anim;
    Vector3 statusUI_Scale;

    void Update()
    {
        //コントローラーがnullの場合return
        if (uiContainer == null) return; 

        //左コントローラーの入力を受け付ける
        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (!leftDevice.isValid) return;

        //グリップボタンを押したかどうかを判定
        bool gripPressed  = false;
        if (leftDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButtonValue))
        {
            gripPressed  = gripButtonValue;
        }
        else if (leftDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            gripPressed  = gripValue >= triggerThreshold;
        }

        //UIを表示　or 非表示
        if (gripPressed)
        {
            if (!uiContainer.activeSelf)
            {
                uiContainer.SetActive(true);
                isShownByHold = true;

                statusUI_Scale = uiContainer.transform.localScale; //現在の大きさを保存
                //アニメーション
                statusUI_Anim?.Kill();
                uiContainer.transform.localScale = Vector3.zero; //大きさを0にする
                statusUI_Anim = uiContainer.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);

            }
        }
        else if (isShownByHold)
        {
           statusUI_Anim?.Kill();

            statusUI_Anim = uiContainer.transform
                .DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    uiContainer.SetActive(false);
                });

            isShownByHold = false;

        }
    }
}
