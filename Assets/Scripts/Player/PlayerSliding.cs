using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSliding : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField]
    private KeyCode slideKey = KeyCode.LeftShift;

    [Header("References")]
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")]
    [SerializeField]
    private float maxSlideTime;
    [SerializeField]
    private float slideForce;
    private float slideTimer;
    [SerializeField]
    private float slideYScale;

    private float startYScale;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        //check for slide key
        if (Input.GetKeyDown(slideKey) && (pm.horizontalInput != 0 || pm.verticalInput != 0))
        {
            startSlide();
        }

        if (Input.GetKeyUp(slideKey) && pm.sliding)
        {
            stopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (pm.sliding) slidingMovement();
    }

    private void startSlide ()
    {
        pm.sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);

        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void slidingMovement()
    {
        Vector3 inputDirection = orientation.forward * pm.verticalInput + orientation.right * pm.horizontalInput;

        // sliding normal
        if (!pm.onSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        //sliding down slope
        else
        {
            rb.AddForce(pm.getSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        } 
        

        if (slideTimer <= 0) stopSlide();
    }

    private void stopSlide ()
    {
        pm.sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}
