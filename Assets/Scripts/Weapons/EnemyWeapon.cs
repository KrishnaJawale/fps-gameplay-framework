using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Collider weaponCollider;
    [SerializeField]
    private LayerMask hittable;
    [SerializeField]
    private Transform muzzle;
    [SerializeField]
    private ParticleSystem[] muzzleFlash;
    [SerializeField]
    private ParticleSystem bulletImpact;
    [SerializeField]
    private TrailRenderer bulletTracer;

    [Header("Weapon positions")]
    [SerializeField]
    private Vector3 positionOffset;

    [Header("General properties")]
    [SerializeField]
    private int bulletsPerShot;
    [SerializeField]
    private Vector3 bulletSpread;

    [Header("Ammo/Reload")]
    public int maxAmmo;
    [SerializeField]
    private int fireRate;
    [SerializeField]
    private float reloadTime;
    [SerializeField]
    private float resetPositionSmooth = 12f;

    [Header("Bullet properties")]
    [SerializeField]
    private float damage;
    [SerializeField]
    private int environmentDamage;
    [SerializeField]
    private float range;
    [SerializeField]
    private float hitForce;

    //state related variables
    private bool shooting = false;
    public bool reloading = false;
    private int ammo;
    private Vector3 originalPos;
    private Quaternion originalRot;

    private WeaponRecoil weaponRecoilScript;

    private float reloadAnimationTime;

    /*
    private void OnDrawGizmos()
    {
        Debug.DrawLine(muzzle.position, muzzle.position + muzzle.forward * 50, Color.red);
    }
    */

    private void Start()
    {
        ammo = maxAmmo;
        weaponRecoilScript = GetComponent<WeaponRecoil>();
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }

    private void Update()
    {
        //reload animation
        if (reloading)
        {
            reloadAnimationTime += Time.deltaTime;
            float spinDelta = -(Mathf.Cos(Mathf.PI * (reloadAnimationTime / reloadTime)) - 1f) / 2f;
            transform.localRotation = Quaternion.Euler(new Vector3(spinDelta * 360f, 0, 0));
        }
        else
        {
            if (!shooting)
            {
                transform.localRotation = originalRot;
                transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos + positionOffset, resetPositionSmooth * Time.deltaTime);
            }
        }
    }

    public void Shoot ()
    {
        if (!shooting && !reloading)
        {
            ammo--;

            weaponRecoilScript.weaponRecoilOnFire();
            muzzleFlashEffect();

            for (int i = 0; i < bulletsPerShot; i++)
            {
                var tracer = Instantiate(bulletTracer, muzzle.position, Quaternion.identity);
                tracer.AddPosition(muzzle.position);

                Vector3 shootDirection = muzzle.forward + new Vector3(Random.Range(-bulletSpread.x, bulletSpread.x),
                                                                Random.Range(-bulletSpread.y, bulletSpread.y),
                                                                Random.Range(-bulletSpread.z, bulletSpread.z));

                shootDirection.Normalize();

                if (Physics.Raycast(muzzle.position, shootDirection, out RaycastHit hitInfo, range, hittable))
                {
                    tracer.transform.position = hitInfo.point;
                    bulletImpactEffect(hitInfo);

                    Rigidbody rb = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                    if (rb != null && !rb.isKinematic) rb.velocity += muzzle.forward * hitForce;

                    //if breakable
                    if (hitInfo.collider.gameObject.TryGetComponent<Destructible>(out Destructible destructible))
                    {
                        destructible.takeDamage(environmentDamage, hitInfo.point);
                    }

                    //if has health system
                    if (hitInfo.collider.gameObject.TryGetComponent<Health>(out Health health))
                    {
                        health.takeDamage(damage, shootDirection);
                    }
                }
            }

            StartCoroutine(ammo <= 0 ? reloadCooldown() : shootingCooldown());
        }

    }

    private IEnumerator shootingCooldown()
    {
        shooting = true;
        yield return new WaitForSeconds(1f / fireRate);
        shooting = false;
    }

    private IEnumerator reloadCooldown()
    {
        reloadAnimationTime = 0f;

        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        ammo = maxAmmo;
        reloading = false;
    }

    private void muzzleFlashEffect()
    {
        //muzzle flash
        foreach (var particle in muzzleFlash) particle.Emit(1);
    }

    private void bulletImpactEffect(RaycastHit hitInfo)
    {
        bulletImpact.transform.position = hitInfo.point;
        bulletImpact.transform.forward = hitInfo.normal;
        bulletImpact.Emit(1);
    }
}
