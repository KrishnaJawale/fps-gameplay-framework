using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    private PlayerMovement pm;

    private void Awake()
    {
        //get script references
        pm = GetComponent<PlayerMovement>();
    }

    // collision detections
    private void OnCollisionEnter(Collision collision)
    {
        if (pm.state == PlayerMovement.movementState.sliding)
        {
            if (collision.gameObject.TryGetComponent<Destructible>(out Destructible destructible))
            {
                destructible.takeDamage(100, transform.position);
            }
        }
    }
}