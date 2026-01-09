using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee : EnemyState
{
    [Header("References")]
    [SerializeField] private Transform FOVOrigin;

    [Header("Melee Attack Settings")]
    [SerializeField] private LayerMask playerTargetLayer;
    [SerializeField] private float meleeDamage;

    private Transform player;
    private Transform playerTarget;

    public override void enterState()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerTarget = player.Find("PlayerTarget");

        enemyAnimator.SetBool("Knife", true);
    }

    public override void runCurrentState()
    {
        if (playerTarget != null)
        {
            //look at player logic
            Vector3 lookAtPlayer = new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z);
            transform.LookAt(lookAtPlayer);

            if (Vector3.Distance(transform.position, playerTarget.position) > exitMeleeRange)
            {
                enemyStateMachine.setState(EnemyStateMachine.enemyStates.Chase);
            }

            if (!FOV.canSeePlayer)
            {
                enemyStateMachine.setState(EnemyStateMachine.enemyStates.Idle);
            }
        }
    }

    private void meleeSwingCheck ()
    {
        //make sure haven't switched state when this function has run

        if (enemyStateMachine.getState() == EnemyStateMachine.enemyStates.Attack)
        {
            //swing attack melee
            if (Physics.Raycast(FOVOrigin.position, transform.forward, out RaycastHit hit, meleeRange, playerTargetLayer))
            {
                //check for player block
                if (player.GetComponent<WeaponManager>().weaponEquipped && player.GetComponent<WeaponManager>().equippedWeapon.blocking)
                {
                    //if player is NOT weapon blocking while enemy slashes at player, enemy goes into stunned state
                    enemyStateMachine.setState(EnemyStateMachine.enemyStates.Stunned);
                }
                else
                {
                    //if player is NOT weapon blocking while enemy slashes at player, deal damage
                    hit.collider.GetComponent<Health>().takeDamage(meleeDamage, transform.forward);
                }
            }
        }
    }

    public override void exitState()
    {
        enemyAnimator.SetBool("Knife", false);
    }

    public override EnemyStateMachine.enemyStates getStateId ()
    {
        return EnemyStateMachine.enemyStates.Attack;
    }
}
