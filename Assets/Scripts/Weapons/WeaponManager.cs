using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private TMP_Text ammoDisplay;

    [Header("References")]
    [SerializeField]
    private Image crosshair;
    [SerializeField]
    private Transform playerCam;
    [SerializeField]
    private Camera playerCam_;
    [SerializeField]
    private Transform weaponHolder;
    [SerializeField]
    private CameraRecoil cameraRecoil;

    [Header("Weapon Pickup/Drop")]
    [SerializeField]
    private float weaponPickupRange;
    [SerializeField]
    private LayerMask interactableLayer;

    [Header("FOV")]
    [SerializeField]
    private float defaultFOV;
    [SerializeField]
    private float adsFOV;
    [SerializeField]
    private float smoothFOV;

    public bool weaponEquipped;
    public Weapon equippedWeapon;
    private Weapon hoveredWeapon;

    private void Start()
    {
        //look for weapon class in player children
        equippedWeapon = GetComponentInChildren<Weapon>();

        //if weapon found, equip it
        if (equippedWeapon)
        {
            weaponEquipped = true;
            //player has default weapon, assign it any needed variables
            equippedWeapon.Equip(weaponHolder, playerCam, cameraRecoil, ammoDisplay);
        }
    }

    private void Update()
    {
        playerCam_.fieldOfView = Mathf.Lerp(playerCam_.fieldOfView, weaponEquipped && equippedWeapon.ads ? adsFOV : defaultFOV, smoothFOV * Time.deltaTime);

        handleWeaponPickup();

        if (weaponEquipped)
        {
            //weapon input handling here
            if (Input.GetMouseButton(0)) equippedWeapon.Shoot();
            if (Input.GetKeyDown(KeyCode.R)) equippedWeapon.Reload();
            if (Input.GetMouseButton(1) && !equippedWeapon.reloading) equippedWeapon.ads = true; else equippedWeapon.ads = false;
            if (Input.GetKeyDown(KeyCode.LeftControl)) equippedWeapon.Block();
            if (Input.GetKeyDown(KeyCode.Q)) { if (!equippedWeapon.blockingAnimation) { equippedWeapon.Drop(true); weaponEquipped = false; equippedWeapon = null; } }
        }
    }

    private void handleWeaponPickup ()
    {
        //pickup weapon (swap)
        //raycast for detecting weapons to pickup, hover effect
        Ray weaponPickupRay = new Ray(playerCam.transform.position, playerCam.transform.forward);

        if (Physics.Raycast(weaponPickupRay, out RaycastHit hitInfo, weaponPickupRange, interactableLayer))
        {
            //weapon outline effect
            if (hoveredWeapon == null)
            {
                hoveredWeapon = hitInfo.transform.GetComponent<Weapon>();
                hoveredWeapon.weaponOutline.enabled = true;
            }
            else
            {
                Weapon hoveredWeaponTemp = hitInfo.transform.GetComponent<Weapon>();

                if (hoveredWeapon == hoveredWeaponTemp)
                {
                    hoveredWeapon.weaponOutline.enabled = true;
                }
                else
                {
                    hoveredWeapon.weaponOutline.enabled = false;
                    hoveredWeapon = hoveredWeaponTemp;
                    hoveredWeapon.weaponOutline.enabled = true;
                }
            }

            //weapon pickup
            if (Input.GetKeyDown(KeyCode.E))
            {
                //if we are already holding a weapon, throw current weapon
                if (weaponEquipped)
                {
                    equippedWeapon.Drop(false);
                    weaponEquipped = false;
                    equippedWeapon = null;
                }

                //equip new weapon
                weaponEquipped = true; //this variable doesn't really make sense since player always has a weapon (can't drop weapons)
                equippedWeapon = hitInfo.transform.GetComponent<Weapon>();
                equippedWeapon.Equip(weaponHolder, playerCam, cameraRecoil, ammoDisplay);
            }
        }
        else
        {
            if (hoveredWeapon != null)
            {
                hoveredWeapon.weaponOutline.enabled = false;
                hoveredWeapon = null;
            }
        }
    }
}
