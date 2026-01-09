using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform weaponGraphics;
    [SerializeField]
    private WeaponAnimator weaponAnimatorScript;
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
    [SerializeField]
    private Vector3 adsPosition;

    [Header("General properties")]
    [SerializeField]
    private int bulletsPerShot;
    [SerializeField]
    private Vector3 bulletSpread;
    [SerializeField]
    private Vector3 bulletSpreadAiming;

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

    [Header("Camera Recoil")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float returnSpeed;
    [SerializeField] private Vector3 recoilRotation;
    [SerializeField] private Vector3 recoilRotationAiming;

    [Header("Drop")]
    [SerializeField]
    private float dropForce;
    [SerializeField]
    private float dropUpwardsForce;
    [SerializeField]
    private float dropRotationForce;

    [Header("Throw")]
    [SerializeField]
    private float throwDamage;
    [SerializeField]
    private float throwForce;
    [SerializeField]
    private float throwUpwardsForce;
    [SerializeField]
    private float throwRotationForce;
    private Vector3 throwDir;

    //weapon outline
    [HideInInspector] public Outline weaponOutline;
    
    //state related variables
    [Header("State")]
    public bool shooting = false;
    public bool reloading = false;
    public bool autoReloading = false;
    public bool blockingAnimation = false;
    public bool blocking = false;
    public bool ads = false;
    public bool inThrow = false;
    private int ammo;
    private bool equipped;

    //private references
    private CameraRecoil cameraRecoil;
    private WeaponRecoil weaponRecoilScript;
    private Transform playerCam;
    private TMP_Text ammoDisplay;
    private float reloadAnimationTime;
    private Rigidbody rb;

    private void Awake ()
    {
        ammo = maxAmmo;
        weaponRecoilScript = GetComponent<WeaponRecoil>();

        addRigidbody();
        addWeaponOutline();
    }

    private void Update()
    {
        if (!equipped) return;

        //reload animation
        if (reloading)
        {
            reloadAnimationTime += Time.deltaTime;
            float spinDelta = -(Mathf.Cos(Mathf.PI * (reloadAnimationTime / reloadTime)) - 1f) / 2f;
            transform.localRotation = Quaternion.Euler(new Vector3 (spinDelta * 360f, 0, 0));
        } else
        {
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.Lerp(transform.localPosition, ads ? adsPosition : Vector3.zero + positionOffset, resetPositionSmooth * Time.deltaTime);
        }
    }

    public void Shoot ()
    {
        if (!shooting && !reloading && !blockingAnimation)
        {
            cameraRecoil.recoilOnFire(rotationSpeed, returnSpeed, recoilRotation, recoilRotationAiming);

            ammo--;   
            weaponRecoilScript.weaponRecoilOnFire();
            muzzleFlashEffect();

            for (int i = 0; i < bulletsPerShot; i++)
            {
                var tracer = Instantiate(bulletTracer, muzzle.position, Quaternion.identity);
                tracer.AddPosition(muzzle.position);

                Vector3 shootDirection = Vector3.zero;

                if (ads)
                {
                    shootDirection = playerCam.forward + new Vector3(Random.Range(-bulletSpreadAiming.x, bulletSpreadAiming.x),
                                                                    Random.Range(-bulletSpreadAiming.y, bulletSpreadAiming.y),
                                                                    Random.Range(-bulletSpreadAiming.z, bulletSpreadAiming.z));
                }
                else
                {
                    shootDirection = playerCam.forward + new Vector3(Random.Range(-bulletSpread.x, bulletSpread.x),
                                                                    Random.Range(-bulletSpread.y, bulletSpread.y),
                                                                    Random.Range(-bulletSpread.z, bulletSpread.z));
                }

                shootDirection.Normalize();

                if (Physics.Raycast(playerCam.position, shootDirection, out RaycastHit hitInfo, range, hittable))
                {
                    tracer.transform.position = hitInfo.point;
                    bulletImpactEffect(hitInfo);

                    Rigidbody rb = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                    if (rb != null && !rb.isKinematic) rb.velocity += playerCam.forward * hitForce;

                    //if breakable
                    if (hitInfo.collider.gameObject.TryGetComponent<Destructible>(out Destructible destructible))
                    {
                        destructible.takeDamage(environmentDamage, hitInfo.point);
                    }

                    //if has hitbox
                    if (hitInfo.collider.TryGetComponent<Hitbox>(out Hitbox hitbox))
                    {
                        hitbox.Hit(damage, shootDirection);

                        //check if hit entity is an enemy - if so, if the enemy is idle, set enemy to chase state to turn to face bullet
                        EnemyStateMachine enemyStateMachine = hitbox.GetComponentInParent<EnemyStateMachine>();
                        if (enemyStateMachine.getState() == EnemyStateMachine.enemyStates.Idle)
                        {
                            enemyStateMachine.setState(EnemyStateMachine.enemyStates.Chase);
                        }
                    }
                }
            }

            StartCoroutine(ammo <= 0 ? reloadCooldown() : shootingCooldown());
            
            //if reloading is false (reloading coroutine did not start), update the bullet count ui
            if (!reloading) updateAmmoDisplay();
        }
    }

    public void Reload ()
    {
        if (ammo < maxAmmo && !reloading && !blockingAnimation)
        {
            StartCoroutine(reloadCooldown());
        }
    }

    public void Block ()
    {
        weaponAnimatorScript.startBlockAnimation(this);
    }

    private IEnumerator shootingCooldown ()
    {
        shooting = true;
        yield return new WaitForSeconds(1f / fireRate);
        shooting = false;
    }

    private IEnumerator reloadCooldown()
    {
        ammoDisplay.text = "RELOADING";
        reloadAnimationTime = 0f;


        //if ammo is less than or equal to 0, that means its an auto reload (player didn't click r)
        autoReloading = true;

        //regular state reloading
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        ammo = maxAmmo;

        autoReloading = false;
        reloading = false;

        updateAmmoDisplay();
    }

    public void Equip ( Transform weaponHolder, Transform playerCam_, CameraRecoil cameraRecoil_, TMP_Text _ammoDisplay)
    {
        if (equipped) return;

        //reset state related variables
        resetValues();
        equipped = true;

        //assign references
        playerCam = playerCam_;
        cameraRecoil = cameraRecoil_;

        Destroy(rb);

        transform.parent = weaponHolder;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        weaponCollider.enabled = false;

        //change gun graphics and all child grahpics layer to weapons so visible by weapons cam
        weaponGraphics.gameObject.layer = LayerMask.NameToLayer("WeaponHeld");
        foreach (Transform graphics in weaponGraphics) {
            graphics.gameObject.layer = LayerMask.NameToLayer("WeaponHeld");
        }

        ammoDisplay = _ammoDisplay;
        updateAmmoDisplay();
    }

    public void Drop ( bool thrown )
    {
        //can't drop if not equipped/in blocking animation
        if (!equipped || blockingAnimation) return;

        //reset state related variables
        resetValues();
        equipped = false;

        addRigidbody();
        if (thrown)
        {
            applyDropForce(throwForce, throwUpwardsForce, throwRotationForce, true);
        }   
        else
        {
            applyDropForce(dropForce, dropUpwardsForce, dropRotationForce, false);
        }

        weaponCollider.enabled = true;

        transform.parent = null;
        playerCam = null;
        equipped = false;

        //change gun graphics and all child grahpics layer to weapons so visible by weapons cam
        foreach (Transform graphics in weaponGraphics)
        {
            graphics.gameObject.layer = LayerMask.NameToLayer("Weapon");
        }
        weaponGraphics.gameObject.layer = LayerMask.NameToLayer("Weapon");

        //set ammo display to empty (no equipped weapon)
        ammoDisplay.text = "";
    }

    private void applyDropForce( float force, float upwardsForce, float rotationForce, bool thrown )
    {
        Vector3 forceDir = playerCam.forward;

        if (!thrown)
        {
            forceDir.y = 0f;
        } 
        else {
            inThrow = true; 

            //calculate throw direction
            RaycastHit hit;

            if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, float.MaxValue))
            {
                forceDir = (hit.point - transform.position).normalized;
            }

            throwDir = forceDir;
        }

        rb.velocity = forceDir * force;
        rb.velocity += Vector3.up * upwardsForce;
        rb.angularVelocity = Random.onUnitSphere * rotationForce;
    }

    private void muzzleFlashEffect ()
    {
        //muzzle flash
        foreach (var particle in muzzleFlash) particle.Emit(1);
    }

    private void bulletImpactEffect ( RaycastHit hitInfo )
    {
        bulletImpact.transform.position = hitInfo.point;
        bulletImpact.transform.forward = hitInfo.normal;
        bulletImpact.Emit(1);
    }

    private void addRigidbody ()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 0.1f;
    }

    private void addWeaponOutline ()
    {
        weaponOutline = gameObject.AddComponent<Outline>();
        weaponOutline.OutlineWidth = 5f;
        weaponOutline.enabled = false;
    }

    private void updateAmmoDisplay()
    {
        ammoDisplay.text = ammo + " / " + maxAmmo;
    }

    private void resetValues ()
    {
        shooting = false;
        reloading = false;
        blockingAnimation = false;
        blocking = false;
        ads = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if weapon hits something while being thrown, set inthrow to false (can no longer affect enemies)
        if (inThrow)
        {
            //if has hitbox (ideally this would put enemy in a "stumble/dazed" state (not stunned, thats for blocks) instead of doing damage)
            if (collision.gameObject.TryGetComponent<Hitbox>(out Hitbox hitbox))
            {
                //check if weapon has hit an enemy, if so, set into stumble/dazed state
                EnemyStateMachine enemyStateMachine = collision.gameObject.GetComponentInParent<EnemyStateMachine>();

                if (enemyStateMachine)
                {
                    enemyStateMachine.setState(EnemyStateMachine.enemyStates.KO);
                }

                hitbox.Hit(throwDamage, throwDir);
            }

            inThrow = false;
        }
    }
}
