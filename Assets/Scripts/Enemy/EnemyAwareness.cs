using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAwareness : MonoBehaviour
{
    [Header("Awareness settings")]
    [SerializeField] private float hearRange;

    Transform player;
    private WeaponManager playerWeaponManager;
    private Weapon playerWeapon;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        playerWeaponManager = player?.GetComponent<WeaponManager>();
    }

    private void Update()
    {
        //update scripts playerweapon reference to be the weapon the player is currently holding
        if (playerWeaponManager.weaponEquipped && playerWeapon != playerWeaponManager.equippedWeapon)
        {
            playerWeapon = playerWeaponManager.equippedWeapon;
        }

        //listen for player shooting - if so, turn to face player
        //added autoreloading check as if player shoots last bullet, reloading happens instead of shooting
        if (playerWeapon.shooting || playerWeapon.autoReloading)
        {
            if (Vector3.Distance(transform.position, player.position) < hearRange)
            {
                transform.LookAt(player.position);
            }
        }
        
    }
}
