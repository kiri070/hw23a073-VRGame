using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.OpenXR.Features.Interactions;

public class EnemyChase : MonoBehaviour
{
    Transform player;
    NavMeshAgent agent;
    Enemy01 enemy01;
    [HideInInspector] public bool chase = false;
    GameManager gm;
    Enemy01_HPBar enemy01_HPBar;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        enemy01 = GetComponent<Enemy01>();
        gm = FindObjectOfType<GameManager>();
        enemy01_HPBar = GetComponentInChildren<Enemy01_HPBar>();
    }

    void Update()
    {
        if(gm.gameOver) return;
        if(gm.gameClear)
        {
            enemy01.TakeDamage(enemy01_HPBar.maxHP);
            return;
        }
        
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
