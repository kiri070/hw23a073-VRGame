using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using System.Collections;

public class Tutorial : MonoBehaviour
{
    [SerializeField] Text tutorialText;
    [SerializeField, TextArea(2, 4)] string[] messages = new string[0];

    int currentIndex = 0;
    bool prevXButton = false;
    bool prevYButton = false;

    void Start()
    {
        ShowMessage();
    }

    void Update()
    {
        //コントローラーの入力を受け取る
        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        bool xButton = false;
        bool yButton = false;
        leftDevice.TryGetFeatureValue(CommonUsages.primaryButton, out xButton);
        leftDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out yButton);

        //進む
        if ((xButton && !prevXButton) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            NextMessage();
        }
        //戻る
        if ((yButton && !prevYButton) || Input.GetKeyDown(KeyCode.Backspace))
        {
            BackMessage();
        }

        prevXButton = xButton;
        prevYButton = yButton;
    }

    public void NextMessage()
    {
        if (messages == null || messages.Length == 0) return;

        currentIndex++;
        //テキストの最後でボタンが押されたとき
        if (currentIndex >= messages.Length)
        {
            currentIndex = messages.Length - 1;
            StartCoroutine(ChangeScene()); //タイトルに移動
            return;
        }

        ShowMessage();
    }

    public void BackMessage()
    {
        if (messages == null || messages.Length == 0) return;

        currentIndex = Mathf.Max(0, currentIndex - 1);
        ShowMessage();
    }

    void ShowMessage()
    {
        if (tutorialText == null || messages == null || messages.Length == 0) return;

        currentIndex = Mathf.Clamp(currentIndex, 0, messages.Length - 1);
        tutorialText.text = messages[currentIndex];
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("TitleScene");
    }
}
