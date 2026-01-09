using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdle : EnemyState
{
    public override void enterState()
    {

    }
    public override void runCurrentState()
    {
        if (FOV.canSeePlayer)
        {
            enemyStateMachine.setState(EnemyStateMachine.enemyStates.Chase);
        }
    }

    public override void exitState()
    {

    }

    public override EnemyStateMachine.enemyStates getStateId ()
    {
        return EnemyStateMachine.enemyStates.Idle;
    }
}
