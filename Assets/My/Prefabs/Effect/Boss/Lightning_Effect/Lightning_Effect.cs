using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning_Effect : MonoBehaviour
{
    ParticleSystem ps;
    GameObject player;
    Collider playerBody;

    HashSet<Collider> hitList = new HashSet<Collider>();

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        player = GameObject.FindWithTag("Player");
        playerBody = player.GetComponentInChildren<Collider>();

        var trigger = ps.trigger;
        trigger.AddCollider(playerBody);
    }

    void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

        int count = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        for (int i = 0; i < count; i++)
        {
            if (hitList.Contains(playerBody)) continue;

            hitList.Add(playerBody);

            //プレイヤーの体力を減らす
            Body body = FindObjectOfType<Body>();
            body.TakeDamage(15);
        }
    }
}
