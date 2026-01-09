using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [Header("References")]
    public Health health;

    public void Hit ( float damage, Vector3 direction )
    {
        health.takeDamage(damage, direction);
    }
}
