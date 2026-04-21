using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
}
