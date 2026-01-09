using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : EnemyState
{
    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed;

    [Header("Update Navmesh Constraints")]
    [SerializeField] private float maxTime = 0.3f;
    //the minimum distance for the player to move (navmesh destination to be updated) for the enemy to update its path
    [SerializeField] private float minDist = 1.0f;

    private float timer = 0.0f;

    private Transform playerTransform;

    private float attackRange;
    
    public override void enterState()
    {
        agent.speed = chaseSpeed;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        attackRange = GetComponent<EnemyShoot>() ? shootRange : meleeRange;
    }

    public override void runCurrentState()
    {
        //update navmesh destination to follow player
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

        //look at player
        Vector3 lookAtPlayer = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        transform.LookAt(lookAtPlayer);

        //change to attack state logic
        if (Vector3.Distance(transform.position, playerTransform.position) < attackRange)
        {
            enemyStateMachine.setState(EnemyStateMachine.enemyStates.Attack);
        }

        //can't see player logic
        if (!FOV.canSeePlayer)
        {
            
            enemyStateMachine.setState(EnemyStateMachine.enemyStates.Idle);
        }
    }

    private void stopPath ()
    {
        agent.SetDestination(transform.position);
        agent.ResetPath();
        enemyAnimator.SetFloat("Speed", 0);
    }

    public override void exitState()
    {
        stopPath();
    }

    public override EnemyStateMachine.enemyStates getStateId ()
    {
        return EnemyStateMachine.enemyStates.Chase;
    }
}