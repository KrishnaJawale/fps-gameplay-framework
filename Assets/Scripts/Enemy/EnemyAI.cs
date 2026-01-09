using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    Animator enemyAnimator;
    NavMeshAgent agent;
    private Transform playerTransform;

    [Header("Update Navmesh Constraints")]
    [SerializeField] private float maxTime = 0.3f;

    //the minimum distance for the player to move (navmesh destination to be updated) for the enemy to update its path
    [SerializeField] private float minDist = 1.0f;

    private float timer = 0.0f;

    private void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0.0f)
        {
            float distance = (playerTransform.position - agent.destination).magnitude;

            if (distance > minDist)
            {
                agent.destination = playerTransform.position;
            }

            timer = maxTime;
        }

        enemyAnimator.SetFloat("Speed", agent.velocity.magnitude);
    }
}
