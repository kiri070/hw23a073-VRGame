using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    Transform player;
    NavMeshAgent agent;
    Enemy01 enemy01;
    public bool chase = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        enemy01 = GetComponent<Enemy01>();
    }

    void Update()
    {
        //追跡
        if(chase)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(transform.position);
        }

        // ★ここが最重要
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
