using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement2 : MonoBehaviour
{
    public InputActionReference move;
    public InputActionReference rightStick;

    private enum movementState
    {
        ground,
        air,
        wallrun,
        crouch,
        slide
    }

    private movementState currentState = movementState.ground;
    private movementState previousState = movementState.ground;

    private Rigidbody rb;
    new public Camera camera;

    [Header("Movement")]
    public float acceleration = 70;
    public float maxSpeed = 10;
    public float deceleration = 20;
    private Vector2 moveInputDirection;
    
    [Header("Slope movement")]
    public float maxSlopeAngle = 45;
    public bool onSlope;
    private RaycastHit groundHit;
    private Vector3 slopeMoveDir;

    [Header("Crouch")]
    public float crouchAcc = 70;
    public float maxCrouchSpeed = 10;
    private bool crouching;
    public bool pressingCrouch;

    [Header("Jump")]
    public float jumpForce = 12;
    public float landingGracePeriod = 0.2f;
    private float landingGrace;
    public bool pressingJump;
    private bool waitingToJump;
    private bool hasJumped;
    private float timeSinceJump;

    [Header("Wallrun")]
    public bool wallRunning;
    public int wallRunDirection;
    private Vector3 wallRunForce;

    [Header("Slide")]
    public float maxSlideSpeed = 30;
    public bool sliding;
    private float slideForceTimer;
    private Vector3 slideDirection;

    [Header("Dash")]
    public bool canDash = true;
    public bool dashing;
    public float dashDistance = 5;
    private float dashCoolDown = 0.1f;

    [Header("Camera")]
    public float mouseSensitivity = 2f;
    private float cameraVerticalRotation;
    private float cameraHorizontalRotation;

    [Header("Checks")]
    public GameObject groundCheck;
    public LayerMask terrainLayer;
    public bool grounded;
    public LayerMask wallLayer;
    public bool walled;
    [HideInInspector] public List<Collider> wallColList = new List<Collider>();
    private Vector3 dirToWall;

    // Other

    public float gravity = 20;
    public bool enterState = true;
    public bool underTerrain;
    public bool slamming;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GroundCheck();
        rb.useGravity = !grounded; // Disable gravity when grounded
        Walled();
        SlopeCheck();
        if (crouching || sliding) RoofCheck();

        CameraRotation();

        moveInputDirection = move.action.ReadValue<Vector2>();
        // BigAssBall is real?!?!?!

        // Dash
        dashCoolDown -= Time.deltaTime;
        if ((grounded || wallRunning) && !dashing && dashCoolDown < 0) canDash = true;
    }

    private void FixedUpdate()
    {
        // Define movement state
        if (grounded) {
            // Grounded
            if (pressingCrouch) {
                if (rb.velocity.magnitude < maxSpeed * 0.5) currentState = movementState.crouch; // Crouch
                else currentState = movementState.slide; // Slide
            }else currentState = movementState.ground; // Ground
        }
        else {
            // Not grounded
            if (walled) {
                if (pressingJump && !pressingCrouch && !slamming) currentState = movementState.wallrun; // Wallrun
                else currentState = movementState.air; // Air
            }else currentState = movementState.air;
        }

        // When switching movement states
        if (currentState != previousState) enterState = true;
        
        switch (currentState) {
            case movementState.ground: GroundMovement(); break;
            case movementState.air: AirMovement(); break;
            case movementState.wallrun: Wallrun(); break;
            case movementState.crouch: Crouchmovement(); break;
            case movementState.slide: SlideMovement(); break;
        }
    }

    /*
     __  __                                            _
    |  \/  |                                          | |
    | \  / |  ___ __   __ ___  _ __ ___    ___  _ __  | |_
    | |\/| | / _ \\ \ / // _ \| '_ ` _ \  / _ \| '_ \ | __|
    | |  | || (_) |\ V /|  __/| | | | | ||  __/| | | || |_
    |_|  |_| \___/  \_/  \___||_| |_| |_| \___||_| |_| \__|
    */

    void GroundMovement()
    {
        Physics.Raycast(transform.position, Vector3.down, out groundHit, 1.3f, terrainLayer);

        // Movement direction on flat ground
        Vector3 flatMovementDirection = transform.rotation * new Vector3(moveInputDirection.x, 0, moveInputDirection.y);
        // Movement direction projected onto the ground
        Vector3 movementDirection = Vector3.ProjectOnPlane(flatMovementDirection, groundHit.normal);

        // Velocity on x and z-axes projected onto the ground
        Vector3 velocityAlongGround = Vector3.ProjectOnPlane(new Vector3(rb.velocity.x, 0, rb.velocity.z), groundHit.normal);
        float velocityMovementDirAngle = Vector3.Angle(velocityAlongGround, movementDirection);
        
        // Adjust movement direction to avoid getting stuck to walls
        if (walled && Vector3.Angle(dirToWall, movementDirection) < 90) movementDirection = Vector3.ProjectOnPlane(movementDirection, dirToWall);
        
        // Movement force
        rb.AddForce(movementDirection * acceleration);
        // Speed limit - apply force to the opposite direction of velocity
        if (velocityAlongGround.magnitude > maxSpeed) rb.AddForce(-velocityAlongGround.normalized * acceleration
        * (1 + velocityAlongGround.magnitude - maxSpeed) * 0.2f);
        // Slow down when no movement inputs or if trying to move against the direction of velocity
        else if (movementDirection == Vector3.zero || velocityMovementDirAngle > 90) rb.AddForce(-velocityAlongGround * deceleration);
        else rb.AddForce(Vector3.ProjectOnPlane(-velocityAlongGround, movementDirection) * deceleration);
    }

    void AirMovement()
    {
        Vector3 movementDirection = transform.rotation * new Vector3(moveInputDirection.x, 0, moveInputDirection.y);
        Vector3 velocityAlongXZ = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float velocityMovementDirAngle = Vector3.Angle(velocityAlongXZ, movementDirection);
        
        // Adjust movement direction to avoid getting stuck to walls
        if (walled && Vector3.Angle(dirToWall, movementDirection) < 90) movementDirection = Vector3.ProjectOnPlane(movementDirection, dirToWall);
        
        // Apply movement force normally when below speed limit or if moving in a way that would not increase speed
        if (velocityAlongXZ.magnitude < maxSpeed || velocityMovementDirAngle > 90) rb.AddForce(movementDirection * acceleration);
        // Apply only the relative sideways movement when over the speed limit to not speed up without assistance
        else rb.AddForce(Vector3.ProjectOnPlane(movementDirection, velocityAlongXZ) * acceleration);
    }

    void Wallrun()
    {
        
    }

    void Crouchmovement()
    {
        
    }

    void SlideMovement()
    {
        
    }
    
    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    
    void WallJump()
    {
        Vector3 velocity = rb.velocity;
        
        // Walljumping makes the player jump away from the wall regardless of directional movement inputs
        rb.velocity = new Vector3(velocity.x, 0, velocity.z) * 1.1f - dirToWall * 8 + Vector3.up * jumpForce * 0.6f;
        
        hasJumped = true;
    }
    
    /*
     _____                       _        
    |_   _|                     | |       
      | |   _ __   _ __   _   _ | |_  ___ 
      | |  | '_ \ | '_ \ | | | || __|/ __|
     _| |_ | | | || |_) || |_| || |_ \__ \
    |_____||_| |_|| .__/  \__,_| \__||___/
                  | |
                  |_|
    */
    
    // Call path: player (object in scene hierarchy) -> Player Input (component) -> Events -> player
    public void JumpInput(InputAction.CallbackContext action)
    {
        if (action.performed)
        {
            if (!waitingToJump) StartCoroutine(WaitToJump());
            pressingJump = true;
        }
        else if (action.canceled)
        {
            pressingJump = false;
            if (wallRunning) WallJump();
            wallRunning = false;
        }
        
        IEnumerator WaitToJump() // Wait until the player is grounded to jump
        {
            waitingToJump = true;
            float i = landingGracePeriod;
            yield return new WaitUntil(() =>
            {
                if (grounded) Jump();
                i -= Time.deltaTime;
                return i <= 0 || grounded;
            });
            waitingToJump = false;
        }
    }
    
    public void CrouchInput(InputAction.CallbackContext action)
    {
        if (action.performed) pressingCrouch = true;
        else if (action.canceled)
        {
            pressingCrouch = false;
            if (!underTerrain) sliding = false;
        }
    }
    
    public void DashInput(InputAction.CallbackContext action)
    {
        // Dash if not touching terrain or performing dash or slam
        if (action.performed && canDash && !dashing && !grounded && !walled && !slamming)
        {
            StartCoroutine(Dash());
        }
    }
    
    IEnumerator Dash()
    {
        rb.useGravity = false;
        canDash = false;
        dashing = true;
        dashCoolDown = 0.1f;

        Vector3 startPos = transform.position;
        Vector3 dashDirection = camera.transform.rotation * Vector3.forward;
        float startVelocity = rb.velocity.magnitude;
        
        rb.AddForce(dashDirection * (dashDistance * 10), ForceMode.Impulse);

        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(delegate
        {
            // Slow down when reaching max dash distance or touching terrain
            if (Vector3.Distance(transform.position, startPos) > dashDistance || grounded || walled) rb.velocity *= 0.9f;
            // Return when slowing down and velocity is the same as before dashing
            return (Vector3.Distance(transform.position, startPos) > dashDistance || grounded || walled || underTerrain) && rb.velocity.magnitude < startVelocity;
        });
        
        rb.useGravity = true;
        dashing = false;
    }
    
    void CameraRotation()
    {
        // Rotate the Camera around its local X axis
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        camera.transform.localRotation = Quaternion.Euler(cameraVerticalRotation, 0f, 0f);
        
        // Rotate the Player Object around its Y axis
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        cameraHorizontalRotation += inputX;
        transform.rotation = Quaternion.Euler(0f, cameraHorizontalRotation, 0f);

        // Gamepad input
        Vector2 input = rightStick.action.ReadValue<Vector2>();
        cameraVerticalRotation -= input.y;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        camera.transform.localRotation = Quaternion.Euler(cameraVerticalRotation, 0, 0);

        cameraHorizontalRotation += input.x;
        transform.rotation = Quaternion.Euler(0f, cameraHorizontalRotation, 0f);
    }
    
    // ENVIROMENT CHECKS
    
    void GroundCheck()
    {
        if (Physics.OverlapBox(groundCheck.transform.position, groundCheck.transform.localScale, Quaternion.identity, terrainLayer).Length > 0)
        {
            grounded = true;
            wallRunning = false;
            dashing = false;
        }
        else // Not grounded
        {
            grounded = false;
            hasJumped = false;
        }
    }

    void SlopeCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, 1.3f, terrainLayer) && Vector3.Angle(Vector3.up, groundHit.normal) < maxSlopeAngle && Vector3.Angle(Vector3.up, groundHit.normal) > 0)
            onSlope = true;
        else onSlope = false;
    }

    void RoofCheck()
    {
        bool checkTrue = false;
        Vector3 rayPos = new Vector3(0.5f,-transform.localScale.y,0);
        for (int i = 0; i < 8; i++)
        {
            Quaternion rotation = Quaternion.Euler(0,45 * i,0);
            if (Physics.Raycast(transform.position + rotation * rayPos, Vector3.up, 2, terrainLayer))
                checkTrue = true;
        }

        underTerrain = checkTrue;
    }

    void Walled()
    {
        if (walled)
        {
            dirToWall = (wallColList[0].ClosestPoint(transform.position) - transform.position).normalized;

            dashing = false;
        }
    }
}
