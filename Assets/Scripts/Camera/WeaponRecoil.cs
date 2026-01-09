using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform recoilPosition;
    [SerializeField] private Transform rotationPoint;

    [Header("Recoil Speed Settings")]
    [SerializeField] private float positionalRecoilSpeed;
    [SerializeField] private float rotationalRecoilSpeed;
    [SerializeField] private float positionalReturnSpeed;
    [SerializeField] private float rotationalReturnSpeed;

    [Header("Recoil Values")]
    [SerializeField] private Vector3 recoilRotation;
    [SerializeField] private Vector3 recoilKickback;
    [SerializeField] private Vector3 recoilRotationAiming;
    [SerializeField] private Vector3 recoilKickbackAiming;

    [Header("State")]
    private bool aiming = false;

    private Vector3 rotationalRecoil;
    private Vector3 positionalRecoil;
    private Vector3 rotation;

    private void FixedUpdate()
    {
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalReturnSpeed * Time.deltaTime);

        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed * Time.fixedDeltaTime);
        rotation = Vector3.Slerp(rotation, rotationalRecoil, rotationalRecoilSpeed * Time.fixedDeltaTime);
        rotationPoint.localRotation = Quaternion.Euler(rotation);
    }

    private void Update()
    {
        //handle ads/not ads state here
    }

    public void weaponRecoilOnFire ()
    {
        if (aiming)
        {
            //ads weapon recoil;
            rotationalRecoil += new Vector3(-recoilRotationAiming.x, Random.Range(-recoilRotationAiming.y, recoilRotationAiming.y), Random.Range(-recoilRotationAiming.z, recoilRotationAiming.z));
            positionalRecoil += new Vector3(Random.Range(-recoilKickbackAiming.x, recoilKickbackAiming.x), Random.Range(-recoilKickbackAiming.y, recoilKickbackAiming.y), recoilKickbackAiming.z);
        } else
        {
            rotationalRecoil += new Vector3(-recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), Random.Range(-recoilRotation.z, recoilRotation.z));
            positionalRecoil += new Vector3(Random.Range(-recoilKickback.x, recoilKickback.x), Random.Range(-recoilKickback.y, recoilKickback.y), recoilKickback.z);
        }
    }

}
