using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public float seVolume = 1f; // SEの音量（0.0〜1.0）
    public float bgmVolume = 1f; // BGMの音量（0.0〜1.0）
    void Start()
    {
        
    }

    /// <summary>
    /// SEを再生するためのメソッド
    /// </summary>
    /// <param name="clip">再生するSEのAudioClip</param>
    /// <param name="volumeScale">音量のスケール</param>
    public void OnPlaySE(AudioSource audioSource, AudioClip clip, float volumeScale = 1f)
    {
        audioSource.PlayOneShot(clip, seVolume * volumeScale);
    }

    /// <summary>
    /// BGMを再生するためのメソッド
    /// </summary>
    /// <param name="clip">再生するBGMのAudioClip</param>
    public void OnPlayBGM(AudioSource audioSource,AudioClip clip)
    {
        audioSource.PlayOneShot(clip, bgmVolume);
    }
}
