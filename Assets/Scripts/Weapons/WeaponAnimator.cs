using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    private Weapon weapon;
    private Animator weaponAnimator;
    private void Start()
    {
        weaponAnimator = GetComponent<Animator>();
    }
    public void startBlockAnimation( Weapon weapon_ )
    {
        weapon = weapon_;
        weaponAnimator.SetTrigger("Block");
        weapon.blockingAnimation = true;
    }

    private void startBlock ()
    {
        weapon.blocking = true;
    }
    private void stopBlock ()
    {
        weapon.blocking = false;
    }
    private void stopBlockAnimation ()
    {
        weapon.blockingAnimation = false;
    }
}
