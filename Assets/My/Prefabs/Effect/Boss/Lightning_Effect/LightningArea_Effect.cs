using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningArea_Effect : MonoBehaviour
{
    public GameObject lightningEffect;

    void Start()
    {
        StartCoroutine(SpawnLightning_Effect());
    }

    IEnumerator SpawnLightning_Effect()
    {
        yield return new WaitForSeconds(5);
        GameObject effect = Instantiate(lightningEffect, 
        new Vector3(transform.position.x, transform.position.y + 14f, transform.position.z),
         lightningEffect.transform.rotation);

        //削除
        yield return new WaitForSeconds(3);
        Destroy(effect);
        Destroy(this.gameObject);
    }
}
