using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    [Header("Enemy State Info")]
    [SerializeField] private EnemyState currentState;

    [Header("Assign enemy states")]
    [SerializeField] private EnemyState idleState;
    [SerializeField] private EnemyState roamState;
    [SerializeField] private EnemyState chaseState;
    [SerializeField] private EnemyState attackState;
    [SerializeField] private EnemyState stunnedState;
    [SerializeField] private EnemyState KOState;
    [SerializeField] private EnemyState deadState;

    public enum enemyStates
    {
        Idle,
        Roam,
        Chase,
        Attack,
        Stunned,
        KO,
        Dead
    }

    private void Update()
    {
        currentState?.runCurrentState();
    }

    public void setState( enemyStates newState )
    {
        currentState.exitState();
        
        switch (newState)
        {
            case (enemyStates.Idle): currentState = idleState;  break;
            case (enemyStates.Roam): currentState = roamState; break;
            case (enemyStates.Chase): currentState = chaseState; break;
            case (enemyStates.Attack): currentState = attackState; break;
            case (enemyStates.Stunned): currentState = stunnedState; break;
            case (enemyStates.KO): currentState = KOState; break;
            case (enemyStates.Dead): currentState = deadState; break;
        }

        currentState.enterState();
    }

    public enemyStates getState ()
    {
        return currentState.getStateId ();
    }
}
