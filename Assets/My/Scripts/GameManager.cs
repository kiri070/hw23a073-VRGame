using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform mainCamera;

    [HideInInspector] public bool gameClear = false;
    [HideInInspector] public bool gameOver = false;
    public GameObject clearImage;
    public GameObject gameOverImage;
    bool gameSet = false;

    System_SoundList system_SoundList;
    SoundManager sm;
    AudioSource audioSource;

    // === ボスのスポーン ===
    public Transform bossSpawnEffect_Pos;
    public GameObject bossSpawnEffect;
    public GameObject bossObj;

    //クリア時のエフェクト
    public GameObject clearEffect_Group;

    //難易度
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }
    public Difficulty difficulty; // 難易度の変数

    //ボススポーン処理
    IEnumerator BossSpawn()
    {
        sm.OnPlaySE(audioSource, system_SoundList.spawnBoss01Sound, 2f);
        yield return new WaitForSeconds(1.5f);
        
        //エフェクト生成
        Instantiate(bossSpawnEffect, bossSpawnEffect_Pos.position, bossSpawnEffect.transform.rotation);
        sm.OnPlaySE(audioSource, system_SoundList.spawnBoss02Sound, 2f);
        yield return new WaitForSeconds(3f);

        //ボスを表示
        bossObj.SetActive(true);
    }
    void Start()
    {
        system_SoundList = GetComponent<System_SoundList>();
        sm = FindObjectOfType<SoundManager>();
        audioSource = GetComponent<AudioSource>();

        //難易度を取得
        string getDifficulty = PlayerPrefs.GetString("Difficulty");
        if(getDifficulty == "Easy") difficulty = Difficulty.Easy; 
        if(getDifficulty == "Normal") difficulty = Difficulty.Normal; 
        if(getDifficulty == "Hard") difficulty = Difficulty.Hard; 

        //ボスの召喚
        StartCoroutine(BossSpawn());
        
    }

    void Update()
    {
        //ゲームをクリアしたら
        if(gameClear)
        {
            clearImage.SetActive(true);
            GotoTitleScene();
        }
        else if(gameOver && !gameClear)
        {
            gameOverImage.SetActive(true);
            GotoTitleScene();
        }      
    }

    /// <summary>
    /// ヒットストップを行う関数
    /// </summary>
    /// <param name="duration"> 持続時間 </param>
    /// <param name="timeScale"> 時間スケール </param>
    /// <returns></returns>
    public IEnumerator HitStop(float duration, float timeScale)
    {
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f; // 時間を戻す
    }

    /// <summary>
    /// カメラを揺らす関数
    /// </summary>
    /// <param name="time">揺れる時間</param>
    /// <param name="power">揺れの強さ</param>
    /// <param name="vaibration">揺れ回数</param>
    /// <param name="random">ランダム度</param>
    public void Shake(float time, float power, int vaibration, int random)
    {
        mainCamera.DOShakePosition(
            duration: time,   // 揺れる時間
            strength: power,   // 揺れの強さ
            vibrato: vaibration,      // 揺れ回数
            randomness: random    // ランダム度
        );
    }

    //タイトルシーンに戻る
    void GotoTitleScene()
    {
        if(!gameSet) StartCoroutine(DelayTitleScene());
        gameSet = true;
    }

    IEnumerator DelayTitleScene()
    {
        if(gameOver) sm.OnPlaySE(audioSource, system_SoundList.gameOverSound, 2f);
        if(gameClear)
        {
            sm.OnPlaySE(audioSource, system_SoundList.gameClearSound, 2f);
            clearEffect_Group.SetActive(true);
        }
        yield return new WaitForSeconds(7f);
        SceneManager.LoadScene("TitleScene");
    }
}
