using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKO : EnemyState
{
    [Header("KO settings")]
    [SerializeField] private float KOTime;

    public override void enterState()
    {
        enemyAnimator.SetBool("KO", true);
    }

    public override void runCurrentState()
    {
        Invoke("endKO", KOTime);
    }

    private void endKO ()
    {
        enemyStateMachine.setState(EnemyStateMachine.enemyStates.Idle);
    }

    public override void exitState()
    {
        CancelInvoke();
        enemyAnimator.SetBool("KO", false);
    }

    public override EnemyStateMachine.enemyStates getStateId ()
    {
        return EnemyStateMachine.enemyStates.KO;
    }
}
