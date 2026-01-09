using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth;
    public float health;
    public DeathBehaviour deathBehaviour;
    [SerializeField]
    private Ragdoll ragdoll;

    private void Start()
    {
        health = maxHealth;

        var childRbs = GetComponentsInChildren<Rigidbody>();

        foreach (var rb in childRbs)
        {
            Hitbox hitbox = rb.gameObject.AddComponent<Hitbox>();

            hitbox.health = this;
        }
    }

    public void takeDamage (float damage, Vector3 direction )
    {
        if (health > 0)
        {
            health -= damage;

            if (health <= 0)
            {
                Die(direction);
            }
        } else
        {
            return;
        }
    }

    private void Die ( Vector3 direction )
    {
        deathBehaviour?.Die( direction );
    }
}
