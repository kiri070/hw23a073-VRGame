using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    Transform player;
    NavMeshAgent agent;
    Enemy01 enemy01;
    [HideInInspector] public bool chase = false;
    GameManager gm;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        enemy01 = GetComponent<Enemy01>();
        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if(gm.gameOver) return;
        
        //追跡
        if(chase && !enemy01.death)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(transform.position);
        }

        if(enemy01.death) return;

        //動作に応じて歩くアニメーション
        if (agent.velocity.magnitude > 0.1f)
        {
            enemy01.ChangeAnim("Walk1", true);
        }
        else
        {
            enemy01.ChangeAnim("Walk1", false);
        }
    }
}
