using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordManager : MonoBehaviour
{
    [Tooltip("剣のprefab")] public List<GameObject> swords = new List<GameObject>();
    [Tooltip("剣の配置位置")] public Transform swordPos;
    void Start()
    {
        //手のオブジェクトに剣を生成
        Instantiate(swords[0], swordPos.position, swordPos.rotation, swordPos);
    }

    void Update()
    {
        
    }
}
