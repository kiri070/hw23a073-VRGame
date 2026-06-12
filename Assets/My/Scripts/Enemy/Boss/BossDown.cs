using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボスがダウンしたときの処理
/// </summary>
public class BossDown : MonoBehaviour
{
    public Golem golem; //ボスのスクリプト
    public GameObject bridgeObj; //橋のオブジェクト
    public GameObject effectObj; //エフェクトオブジェクト
    Vector3 bridge_position; //橋の初期位置
    float counter = 0f; //時間計測変数
    
    Coroutine moveBridgeCoroutine; //橋を動かすコルーチンの参照

    bool downStart_hasProcessed = false; //処理を一回だけ行うためのフラグ
    bool downEnd_hasProcessed = false; //処理を一回だけ行うためのフラグ

    void Start()
    {
        bridge_position = bridgeObj.transform.position; //橋の初期位置を保存
    }

    void Update()
    {
        // ダウン開始時
        if (golem.isDown && !downStart_hasProcessed && !downEnd_hasProcessed)
        {
            if (moveBridgeCoroutine != null)
            {
                StopCoroutine(moveBridgeCoroutine); // もし既に橋を動かすコルーチンが動いていたら停止
                moveBridgeCoroutine = null;
            }

            moveBridgeCoroutine = StartCoroutine(MoveBridge()); // 橋を動かすコルーチンを開始
            downStart_hasProcessed = true; // 処理済みフラグを立てる
            counter = 0f;
        }

        //ダウン中
        if (golem.isDown)
        {
            counter += Time.deltaTime;

            // ダウン終了の少し前に橋を元に戻し始める
            if (!downEnd_hasProcessed && (counter >= golem.bossDownTime - 3f))
            {
                Debug.Log("ダウン終了直前：橋を戻し始めます");
                StartCoroutine(ResetBridge()); // 橋を元の位置に戻すコルーチンを開始
                downEnd_hasProcessed = true; // 処理済みフラグを立てる
            }
        }

        // ボスが復帰（ダウン解除）したタイミングでフラグとカウンタをリセット
        if (!golem.isDown && (downStart_hasProcessed || downEnd_hasProcessed))
        {
            downStart_hasProcessed = false;
            downEnd_hasProcessed = false;
            counter = 0f;
            if (moveBridgeCoroutine != null)
            {
                StopCoroutine(moveBridgeCoroutine);
                moveBridgeCoroutine = null;
            }
        }
    }

    //橋を動かすコルーチン
    IEnumerator MoveBridge()
    {
        while (bridgeObj.transform.position.z <= 49f) //到達地点まで
        {
            bridgeObj.transform.position += Vector3.forward * Time.deltaTime * 5f;
            effectObj.SetActive(true); //エフェクトを表示
            yield return null;
        }
        effectObj.SetActive(false); //エフェクトを非表示
    }

    //橋を元の位置に戻すコルーチン
    IEnumerator ResetBridge()
    {
        while (bridgeObj.transform.position.z > bridge_position.z) //到達地点まで
        {
            bridgeObj.transform.position -= Vector3.forward * Time.deltaTime * 5f;
            effectObj.SetActive(true); //エフェクトを表示
            yield return null;
        }
        effectObj.SetActive(false); //エフェクトを非表示

        bridgeObj.transform.position = bridge_position;
        // フラグのリセットはボスが復帰したタイミングで行う（Update内で処理）
        moveBridgeCoroutine = null;
    }
}
