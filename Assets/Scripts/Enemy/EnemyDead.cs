using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDead : EnemyState
{
    public override void enterState ()
    {
        agent.ResetPath();
        agent.enabled = false;
    }

    public override void runCurrentState ()
    {

    }

    public override void exitState ()
    {

    }

    public override EnemyStateMachine.enemyStates getStateId ()
    {
        return EnemyStateMachine.enemyStates.Dead;
    }
}
