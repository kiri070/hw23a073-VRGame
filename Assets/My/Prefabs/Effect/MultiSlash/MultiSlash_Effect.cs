using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSlash_Effect : MonoBehaviour
{
    //エフェクトの当たり判定
    void OnParticleCollision(GameObject other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
