using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Option_GameScene : MonoBehaviour
{

    public void OnGameSelectButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");
    }
}
