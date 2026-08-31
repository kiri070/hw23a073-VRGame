using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボスがダウンしたときの誘導エフェクト
/// </summary>
public class GuidEffect : MonoBehaviour
{
    Golem golem;
    [SerializeField] GameObject guidEffect;
    bool canSpawn = true;

    //Golemから参照をもらう
    public void SetGolem(Golem golem)
    {
        this.golem = golem;
    }

    void Update()
    {
        if(golem != null && golem.isDown && canSpawn)
        {
            StartCoroutine(SpawnEffect());
        }
    }

    IEnumerator SpawnEffect()
    {
        Instantiate(guidEffect, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        canSpawn = false;
        yield return new WaitForSeconds(golem.bossDownTime + 3f);
        canSpawn = true;
    }
}
