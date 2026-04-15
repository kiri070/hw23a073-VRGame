using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CheckSkill : MonoBehaviour
{
    [Tooltip("右コントローラー")]public Transform rightHand;
    [Tooltip("左コントローラー")]public Transform leftHand;
    [Tooltip("ヘッド")]public Transform head;

    void Start()
    {
        
    }

    void Update()
    {
        IsCheckBack(rightHand);
        IsCheckBack(leftHand);
    }

    //コントローラーが後ろにあるか確認
    void IsCheckBack(Transform controller)
    {
        //頭からの位置に変換
        Vector3 localPos = head.InverseTransformPoint(controller.position);

        if (localPos.z > 0.2f)
        {
            Debug.Log(controller.name + ":前");
        }
        else if (localPos.z < -0.2f)
        {
            Debug.Log(controller.name + ":後ろ");
        }
    }
}
