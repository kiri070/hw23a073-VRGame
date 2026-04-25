using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [Header("タイトルUI")]
    public GameObject mainUI;

    [Header("ロード画面全体")]
    public GameObject loadingUI;     // LoadingUI親ごと指定

    [Header("任意")]
    public Slider loadingBar;
    public Text loadingText;

    private bool isLoading = false;

    [Header("難易度ボタンの上に表示するオブジェクト")]
    public GameObject selectEasyObj;
    public GameObject selectNormalObj;
    public GameObject selectHardObj;

    void Start()
    {
        OnNormalButton(); //初期難易度はNormal
        // 最初はロード画面を非表示
        if (loadingUI != null)
            loadingUI.SetActive(false);
    }

    //難易度ボタン
    public void OnEasyButton()
    {
        PlayerPrefs.SetString("Difficulty", "Easy");
        selectEasyObj.SetActive(true);
        selectNormalObj.SetActive(false);
        selectHardObj.SetActive(false);
    }
    public void OnNormalButton()
    {
        PlayerPrefs.SetString("Difficulty", "Normal");
        selectEasyObj.SetActive(false);
        selectNormalObj.SetActive(true);
        selectHardObj.SetActive(false);
    }
    public void OnHardButton()
    {
        PlayerPrefs.SetString("Difficulty", "Hard");
        selectEasyObj.SetActive(false);
        selectNormalObj.SetActive(false);
        selectHardObj.SetActive(true);
    }

    //プレイボタン
    public void OnPlayGameSceneButton()
    {
        if (isLoading) return;

        isLoading = true;
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        // タイトルUI消す
        if (mainUI != null)
            mainUI.SetActive(false);

        // ロードUI表示
        if (loadingUI != null)
            loadingUI.SetActive(true);

        // 初期化
        if (loadingBar != null)
            loadingBar.value = 0f;

        if (loadingText != null)
            loadingText.text = "0%";

        AsyncOperation load = SceneManager.LoadSceneAsync("GameScene");
        load.allowSceneActivation = false;

        while (!load.isDone)
        {
            float progress = Mathf.Clamp01(load.progress / 0.9f);

            // バー更新
            if (loadingBar != null)
                loadingBar.value = progress;

            // 数字更新
            if (loadingText != null)
                loadingText.text = (progress * 100f).ToString("F0") + "%";

            // 読み込み完了
            if (load.progress >= 0.9f)
            {
                if (loadingBar != null)
                    loadingBar.value = 1f;

                if (loadingText != null)
                    loadingText.text = "100%";

                yield return new WaitForSeconds(0.5f);
                load.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}