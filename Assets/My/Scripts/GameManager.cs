using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public Transform mainCamera;

    [HideInInspector] public bool gameClear = false;
    [HideInInspector] public bool gameOver = false;
    public GameObject clearImage;
    public GameObject gameOverImage;

    //難易度
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }
    public Difficulty difficulty; // 難易度の変数
    void Start()
    {
        difficulty = Difficulty.Normal; // デフォルトの難易度をNormalに設定
    }

    void Update()
    {
        //ゲームをクリアしたら
        if(gameClear)
        {
            clearImage.SetActive(true);
        }
        else if(gameOver && !gameClear)
        {
            gameOverImage.SetActive(true);
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
}
