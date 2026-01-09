using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform breakInstance;

    [Header("Environment Health")]
    [SerializeField] private float health;

    [Header("Break Impact Settings")]
    [SerializeField] private float breakImpactForce;
    [SerializeField] private float breakImpactForceRadius;

    private bool breakInstanceCreated = false;

    public void takeDamage (int damageTaken, Vector3 damagePosition)
    {
        health -= damageTaken;

        if (health <= 0)
        {
            Break(damagePosition);
        }
    }
    private void Break (Vector3 breakForcePosition)
    {
        if (!breakInstanceCreated)
        {
            Transform breakInstance_ = Instantiate(breakInstance, transform.position, transform.rotation);
            foreach (Transform piece in breakInstance_)
            {
                if (piece.TryGetComponent<Rigidbody>(out Rigidbody rb)) { rb.AddExplosionForce(breakImpactForce, breakForcePosition, breakImpactForceRadius, 5.0f); }
            }

            //if breakable has parent (ex door pivot), destroy parent otherwise destroy itself
            if (transform.parent != null) Destroy(transform.parent.gameObject); else Destroy(gameObject);

            breakInstanceCreated = true;
        }
    }
}