using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState : MonoBehaviour
{
    protected float meleeRange = 2.5f;
    protected float exitMeleeRange = 3.0f;
    protected float shootRange = 15f;
    protected float exitShootRange = 16f;

    protected Animator enemyAnimator;
    protected EnemyStateMachine enemyStateMachine;
    protected NavMeshAgent agent;
    protected EnemyFOV FOV;

    private void Awake ()
    {
        enemyAnimator = GetComponent<Animator>();
        enemyStateMachine = GetComponent<EnemyStateMachine>();
        agent = GetComponent<NavMeshAgent>();
        FOV = GetComponent<EnemyFOV>();
    }

    public abstract void enterState ();
    public abstract void runCurrentState();
    public abstract void exitState ();

    public abstract EnemyStateMachine.enemyStates getStateId (); 
}
