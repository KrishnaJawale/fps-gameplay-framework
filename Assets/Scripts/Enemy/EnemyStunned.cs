using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunned : EnemyState
{

    [Header("Stunned settings")]
    [SerializeField] private float stunTime;

    public override void enterState()
    {
        enemyAnimator.SetBool("Stunned", true);
    }

    public override void runCurrentState()
    {
        //set timer to return to idle state
        Invoke("endStun", stunTime);
    }

    private void endStun ()
    {
        //return to idle state (finish the stun)
        enemyStateMachine.setState(EnemyStateMachine.enemyStates.Idle);
    }

    public override void exitState()
    {
        CancelInvoke();
        enemyAnimator.SetBool("Stunned", false);
    }

    public override EnemyStateMachine.enemyStates getStateId ()
    {
        return EnemyStateMachine.enemyStates.Stunned;
    }
}
