using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("タイトルUI")]
    public GameObject mainUI;

    [Header("フェード用Sphere")]
    public GameObject fadeSphere;

    [Header("フェード設定")]
    public float fadeTime = 0.5f;

    [Header("難易度ボタンの上に表示するオブジェクト")]
    public GameObject selectEasyObj;
    public GameObject selectNormalObj;
    public GameObject selectHardObj;

    public GameObject hardEffect;

    bool isTutorial = false;
    private bool isLoading = false;

    // フェード用
    private Renderer fadeRenderer;
    private Material fadeMaterial;
    private Color fadeColor;

    // 音声
    SoundManager sm;
    TitleSoundList titleSoundList;
    AudioSource audioSource;

    void Start()
    {
        sm = FindObjectOfType<SoundManager>();
        titleSoundList = FindObjectOfType<TitleSoundList>();
        audioSource = this.gameObject.GetComponent<AudioSource>();

        // フェードSphereの準備
        if (fadeSphere != null)
        {
            fadeRenderer = fadeSphere.GetComponent<Renderer>();
            fadeMaterial = fadeRenderer.material;

            fadeColor = fadeMaterial.color;

            // 最初は透明
            fadeColor.a = 0f;
            fadeMaterial.color = fadeColor;
        }

        OnNormalButton();
    }


    // =========================
    // 難易度ボタン
    // =========================

    public void OnEasyButton()
    {
        sm.OnPlaySE(audioSource, titleSoundList.clickSound);

        PlayerPrefs.SetString("Difficulty", "Easy");

        selectEasyObj.SetActive(true);
        selectNormalObj.SetActive(false);
        selectHardObj.SetActive(false);

        hardEffect.SetActive(false);
    }


    public void OnNormalButton()
    {
        sm.OnPlaySE(audioSource, titleSoundList.clickSound);

        PlayerPrefs.SetString("Difficulty", "Normal");

        selectEasyObj.SetActive(false);
        selectNormalObj.SetActive(true);
        selectHardObj.SetActive(false);

        hardEffect.SetActive(false);
    }


    public void OnHardButton()
    {
        sm.OnPlaySE(audioSource, titleSoundList.clickSound);

        PlayerPrefs.SetString("Difficulty", "Hard");

        selectEasyObj.SetActive(false);
        selectNormalObj.SetActive(false);
        selectHardObj.SetActive(true);

        hardEffect.SetActive(true);
    }


    // =========================
    // プレイボタン
    // =========================

    public void OnPlayGameSceneButton()
    {
        if (isLoading) return;

        isTutorial = false;
        StartLoadScene();
    }


    // =========================
    // チュートリアルボタン
    // =========================

    public void OnPlayGameSceneButton(Object clickedButton)
    {
        if (isLoading) return;

        isTutorial = clickedButton != null &&
                     clickedButton.name == "Tutorial";

        StartLoadScene();
    }


    // =========================
    // ロード開始
    // =========================

    void StartLoadScene()
    {
        isLoading = true;
        StartCoroutine(LoadScene());
    }


    IEnumerator LoadScene()
    {
        // タイトルUIを消す
        if (mainUI != null)
            mainUI.SetActive(false);


        // =========================
        // フェードアウト
        // =========================

        yield return StartCoroutine(Fade(0f, 1f));


        // =========================
        // シーンロード
        // =========================

        string sceneName = isTutorial
            ? "TutorialScene"
            : "GameScene";

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);

        load.allowSceneActivation = false;


        while (load.progress < 0.9f)
        {
            yield return null;
        }


        // 完全にロード完了
        load.allowSceneActivation = true;
    }


    // =========================
    // フェード処理
    // =========================

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeMaterial == null)
            yield break;

        float time = 0f;

        while (time < fadeTime)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / fadeTime);

            fadeColor.a = Mathf.Lerp(
                startAlpha,
                endAlpha,
                t
            );

            fadeMaterial.color = fadeColor;

            yield return null;
        }

        fadeColor.a = endAlpha;
        fadeMaterial.color = fadeColor;
    }
}