using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDie : DeathBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyStateMachine enemyStateMachine;

    private Ragdoll ragdoll;

    private void Start()
    {
        ragdoll = GetComponent<Ragdoll>();
    }
    public override void Die ( Vector3 direction )
    {
        ragdoll.activateRagdoll();
        ragdoll.addForce(direction);
        enemyStateMachine.setState(EnemyStateMachine.enemyStates.Dead);
    }
}
