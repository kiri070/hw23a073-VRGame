using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    // === プレイヤー情報 === //
    public int hp = 100;

    float lastY;
    void Start()
    {
        lastY = Camera.main.transform.eulerAngles.y;
    }

    void Update()
    {
        Vector3 headPos = Camera.main.transform.position;

        // 位置追従
        transform.position = new Vector3(headPos.x, transform.position.y, headPos.z);

        float currentY = Camera.main.transform.eulerAngles.y;

        // 回転差を計算
        float diff = Mathf.Abs(Mathf.DeltaAngle(lastY, currentY));

        // 一定以上向き変わったら回転
        if (diff > 10f)
        {
            transform.rotation = Quaternion.Euler(0, currentY, 0);
            lastY = currentY;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        //Enemy01のパンチ攻撃
        if(other.CompareTag("Enemy01_Punch"))
        {
            hp -= 10;
            Debug.Log("プレイヤーHP:" + hp);
        }
    }

    //プレイヤーが攻撃を受ける関数
    public void TakeDamage(int damage)
    {
        hp -= damage;
    }
}