using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform FOVOrigin;
    private Transform playerTarget;

    [Header("Field of View")]
    [SerializeField] private float rangeFOV;
    [SerializeField] private float angleFOV;
    [SerializeField] private LayerMask obstructFOV;

    public bool canSeePlayer = false;

    private void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerTarget");
        StartCoroutine(playerInFOVRoutine());
    }
    private IEnumerator playerInFOVRoutine()
    {
        //make it so enemy only checks if player is in view, 5 times every second
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            playerFOVCheck();
        }
    }

    private void playerFOVCheck()
    {
        if (playerTarget)
        {
            //direction to player
            if (Vector3.Distance(FOVOrigin.position, playerTarget.position) < rangeFOV)
            {
                Vector3 directionToPlayer = (playerTarget.position - FOVOrigin.position).normalized;
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
                float distanceToPlayer = Vector3.Distance(FOVOrigin.position, playerTarget.position);

                if (angleToPlayer < (angleFOV / 2))
                {
                    if (!Physics.Raycast(FOVOrigin.position, directionToPlayer, out RaycastHit hit, distanceToPlayer, obstructFOV))
                    {
                        canSeePlayer = true;
                    }
                    else
                    {
                        canSeePlayer = false;
                    }
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }
        } else
        {
            return;
        }
    }
}
