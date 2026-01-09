using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKick : MonoBehaviour
{

    [Header("Keybinds")]
    [SerializeField] private KeyCode kickKey = KeyCode.V;

    [Header("References")]
    [SerializeField] private Transform playerCam;
    [SerializeField] private CameraRecoil cameraRecoil;

    [Header("Kick Settings")]
    [SerializeField] private float kickRange;
    [SerializeField] private LayerMask kickable;
    [SerializeField] private int kickDamage;

    [Header("Camera Recoil")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float returnSpeed;
    [SerializeField] private Vector3 recoilRotation;
    [SerializeField] private Vector3 recoilRotationAiming;

    [SerializeField] private float rotationSpeedImpact;
    [SerializeField] private float returnSpeedImpact;
    [SerializeField] private Vector3 recoilRotationImpact;
    [SerializeField] private Vector3 recoilRotationAimingImpact;

    private Animator legsAnimator;

    public bool kickEnabled = false;

    private void Start()
    {
        legsAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(kickKey))
        {
            if (Physics.Raycast(playerCam.position, playerCam.forward, out RaycastHit hitInfo, kickRange, kickable))
            {
                cameraRecoil.recoilOnFire(rotationSpeedImpact, returnSpeedImpact, recoilRotationImpact, recoilRotationAimingImpact);
            }
            else
            {
                cameraRecoil.recoilOnFire(rotationSpeed, returnSpeed, recoilRotation, recoilRotationAiming);
            }

            legsAnimator.SetTrigger("kick");
        }

        if (kickEnabled)
        {
            if (Physics.Raycast(playerCam.position, playerCam.forward, out RaycastHit hitInfo, kickRange, kickable))
            {
                cameraRecoil.recoilOnFire(rotationSpeedImpact, returnSpeedImpact, recoilRotationImpact, recoilRotationAimingImpact);

                if (hitInfo.transform.TryGetComponent<Destructible>(out Destructible destructible))
                {
                    //kick should break any destructible
                    destructible.takeDamage(10000, hitInfo.point);
                }

                if (hitInfo.transform.TryGetComponent<Hitbox>(out Hitbox hitbox))
                {
                    //check if its an enemy, if so, set enemy state to ko
                    EnemyStateMachine enemyStateMachine = hitbox.GetComponentInParent<EnemyStateMachine>();

                    if (enemyStateMachine)
                    {
                        if (enemyStateMachine.getState() != EnemyStateMachine.enemyStates.Dead)
                            //kick should insta kill enemy for now
                            hitbox.Hit(kickDamage, playerCam.forward);
                    }
                }
            }
        }
    }

    private void enableKick ()
    {
        kickEnabled = true;
    }

    private void disableKick ()
    {
        kickEnabled = false;
    }
}
