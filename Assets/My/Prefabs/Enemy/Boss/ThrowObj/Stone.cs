using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    GameObject player;

    public float speed = 3f;        // 横のスピード（遅め）
    public float arcHeight = 2f;    // 山なりの高さ
    public float offsetRange = 1.5f; // 少しズラす

    Vector3 startPos;
    Vector3 targetPos;
    float progress = 0f;

    void Start()
    {
        player = GameObject.Find("Player").transform.Find("Body").gameObject;

        startPos = transform.position;

        // プレイヤー付近に少しズラす
        Vector3 offset = new Vector3(
            Random.Range(-offsetRange, offsetRange),
            0,
            Random.Range(-offsetRange, offsetRange)
        );

        targetPos = player.transform.position + offset;
    }

    void Update()
    {
        // 進行度（ゆっくり進む）
        progress += Time.deltaTime * speed;

        // 位置補間
        Vector3 pos = Vector3.Lerp(startPos, targetPos, progress);

        // 放物線（山なり）
        float height = Mathf.Sin(progress * Mathf.PI) * arcHeight;

        pos.y += height;

        transform.position = pos;

        // 終わったら消す
        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
