using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    [Header("Recoil Config")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float returnSpeed;

    [Header("Hipfire")]
    [SerializeField]  private Vector3 recoilRotation;

    [Header("Aiming")]
    [SerializeField]  private Vector3 recoilRotationAiming;

    [Header("State")]
    [SerializeField] private bool aiming;

    private Vector3 currentRotation;
    private Vector3 rotation;

    private void FixedUpdate()
    {
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        rotation = Vector3.Slerp(rotation, currentRotation, rotationSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(rotation);
    }

    public void recoilOnFire (float rotationSpeed_, float returnSpeed_, Vector3 recoilRotation_, Vector3 recoilRotationAiming_)
    {
        //assign new recoil settings
        rotationSpeed = rotationSpeed_;
        returnSpeed = returnSpeed_;
        recoilRotation = recoilRotation_;
        recoilRotationAiming = recoilRotationAiming_;


        if (aiming)
        {
            //ads recoil
            currentRotation += new Vector3(-recoilRotationAiming.x, Random.Range(-recoilRotationAiming.y, recoilRotationAiming.y), Random.Range(-recoilRotationAiming.z, recoilRotationAiming.z));
        } else
        {
            currentRotation += new Vector3(-recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), Random.Range(-recoilRotation.z, recoilRotation.z));
        }
    }
}
