using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : EnemyState
{
    [Header("Shooting settings")]
    [SerializeField] private float shootDelay;

    private bool startShooting;

    private Transform playerTarget;

    private EnemyWeapon equippedWeapon;
    public override void enterState()
    {
        //enemy shoot delay so he doesn't shoot instantaneously at player
        startShooting = false;
        Invoke("enableStartShooting", shootDelay);

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        playerTarget = player.Find("PlayerTarget");

        equippedWeapon = GetComponentInChildren<EnemyWeapon>();

        //if weapon found, equip it
        if (equippedWeapon)
        {
            //???
        }
    }

    public override void runCurrentState()
    {
        //look at player logic
        Vector3 lookAtPlayer = new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z);
        transform.LookAt(lookAtPlayer);

        //make weapon face player, when not reloading

        if (!equippedWeapon.reloading) equippedWeapon.transform.LookAt(playerTarget.position);

        if (startShooting) equippedWeapon.Shoot();

        if (Vector3.Distance(transform.position, playerTarget.position) > exitShootRange)
        {
            enemyStateMachine.setState(EnemyStateMachine.enemyStates.Chase);
        }

        if (!FOV.canSeePlayer)
        {
            enemyStateMachine.setState(EnemyStateMachine.enemyStates.Idle);
        }
    }

    private void enableStartShooting ()
    {
        startShooting = true;
    }

    public override void exitState()
    {
    }

    public override EnemyStateMachine.enemyStates getStateId()
    {
        return EnemyStateMachine.enemyStates.Attack;
    }

}
