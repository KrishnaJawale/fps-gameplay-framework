using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRoam : EnemyState
{
    [Header("References")]
    [SerializeField] private Transform roamCenter;

    [Header("Roam Settings")]
    [SerializeField] private float roamRange;
    [SerializeField] private float roamSpeed;
    [SerializeField] private float minWalkDist;

    Vector3 walkPoint;

    public override void enterState()
    {
        agent.speed = roamSpeed;
    }

    public override void runCurrentState ()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (findWalkPoint())
            {
                agent.SetDestination(walkPoint);
            }
        }

        if (FOV.canSeePlayer)
        {
            enemyStateMachine.setState(EnemyStateMachine.enemyStates.Chase);
        }
    }
      
    private bool findWalkPoint ()
    {
        Vector3 randomPoint = roamCenter.position + Random.insideUnitSphere * roamRange;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas) && (Vector3.Distance(transform.position, randomPoint) > minWalkDist))
        {
            walkPoint = hit.position;
            return true;
        }

        return false;
    }

    public override void exitState()
    {

    }

    public override EnemyStateMachine.enemyStates getStateId ()
    {
        return EnemyStateMachine.enemyStates.Roam;
    }
}
