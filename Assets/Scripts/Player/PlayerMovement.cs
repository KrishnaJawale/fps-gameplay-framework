using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;

    [Header("References")]
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private Transform playerObj;

    [Header("Movement")]
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float groundDrag;

    [Header("Jumping")]
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float jumpCooldown;
    [SerializeField]
    private float airMultiplier;

    private bool readyToJump = true;
    private bool exitingSlope;

    private float startYScale;

    [Header("Sliding")]
    public bool sliding;
    public float maxSlideSpeed;

    [Header("Ground Check")]
    [SerializeField]
    private float playerHeight;
    [SerializeField]
    private LayerMask whatIsGround;

    public bool grounded;

    [Header("Slope Handling")]
    [SerializeField]
    private float maxSlopeAngle;
    private RaycastHit slopeHit;

    [Header("Input")]
    public float horizontalInput;
    public float verticalInput;

    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    Vector3 moveDirection;

    Rigidbody rb;

    public enum movementState
    {
        running,
        sliding,
        air,
    }

    public movementState state;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        //grounded check
        float groundedCheckAddHeight;
        if (onSlope()) groundedCheckAddHeight = 0.5f; else groundedCheckAddHeight = 0.3f;
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + groundedCheckAddHeight, whatIsGround);

        getInput();
        stateHandler();
    }

    private void FixedUpdate()
    {
        speedControl();

        //handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        playerMovement();
    }

    private void stateHandler ()
    {
        //mode -- sliding
        if (sliding)
        {
            state = movementState.sliding;

            if (onSlope() && rb.velocity.y < 0.1f) desiredMoveSpeed = maxSlideSpeed;
            else desiredMoveSpeed = runSpeed;
        }

        //mode -- running
        else if (grounded)
        {
            state = movementState.running;
            desiredMoveSpeed = runSpeed;
        }

        //mode -- in air
        else
        {
            state = movementState.air;
        }

        //check if desiredMoveSpeed has changed drastically
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 5f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(smoothLerpMoveSpeed());
        } else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private void getInput ()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            Jump();

            Invoke(nameof(resetJump), jumpCooldown);
        }
    }

    private void playerMovement()
    {
        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on slope
        if (onSlope() && !exitingSlope)
        {
            rb.AddForce(getSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0) rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        //disable gravity when on slope
        rb.useGravity = !onSlope();
    }

    private IEnumerator smoothLerpMoveSpeed ()
    {
        //smoothly lerp moveSpeed to desiredMoveSpeed
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void Jump ()
    {
        readyToJump = false;
        exitingSlope = true;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void resetJump ()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    public bool onSlope ()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 getSlopeMoveDirection( Vector3 direction )
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void speedControl()
    {
        if (onSlope() && !exitingSlope)
        {
            //limiting speed on slope
            if (rb.velocity.magnitude > moveSpeed) rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        {

            //limiting speed regular
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //limit velocity to movement speed if needed
            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }
    }

}