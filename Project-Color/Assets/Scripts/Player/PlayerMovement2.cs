using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class MovementState
{
    public virtual void EnterState(){}
    public virtual void Update(){}
    public virtual void ExitState(){}
}

public class GroundMovement : MovementState
{
    public override void EnterState()
    {
        
    }
    public override void Update()
    {
        
    }
    public override void ExitState()
    {
        
    }
}
public class AirMovement : MovementState
{
    public override void EnterState()
    {
        
    }
    public override void Update()
    {
        
    }
    public override void ExitState()
    {
        
    }
}
public class WallMovement : MovementState
{
    public override void EnterState()
    {
        
    }
    public override void Update()
    {
        
    }
    public override void ExitState()
    {
        
    }
}
public class CrouchMovement : MovementState
{
    public override void EnterState()
    {
        
    }
    public override void Update()
    {
        
    }
    public override void ExitState()
    {
        
    }
}
public class SlideMovement : MovementState
{
    public override void EnterState()
    {
        
    }
    public override void Update()
    {
        
    }
    public override void ExitState()
    {
        
    }
}

public class PlayerMovement2 : MonoBehaviour
{
    public InputActionReference move;
    public InputActionReference rightStick;

    private GroundMovement Ground = new GroundMovement();
    private AirMovement Air = new AirMovement();
    private WallMovement Wallrun = new WallMovement();
    private CrouchMovement Crouch = new CrouchMovement();
    private SlideMovement Slide = new SlideMovement();
    private enum movementState { ground, air, wallrun, crouch, slide }
    // private movementState currentState = movementState.ground;
    // private movementState previousState = movementState.ground;
    private MovementState currentState;
    private MovementState previousState;
    
    private Rigidbody rb;
    new public Camera camera;

    [Header("Movement")]
    public float acceleration = 70;
    public float maxSpeed = 10;
    private Vector2 moveDirection;
    [Header("Slope movement")]
    public float maxSlopeAngle = 45;
    public bool onSlope;
    private RaycastHit slopeHit;
    private Vector3 slopeMoveDir;
    
    [Header("Crouch")]
    public float crouchAcc = 70;
    public float maxCrouchSpeed = 10;
    private bool crouching;
    
    [Header("Jump")]
    public float jumpForce = 12;
    public float landingGracePeriod = 0.2f;
    private float landingGrace;
    public bool pressingJump;
    private bool hasJumped;
    private float timeSinceJump;
    
    [Header("Wallrun")]
    public bool wallRunning;
    public int wallRunDirection;
    private bool firstWallRunCall = true;
    private Vector3 wallRunForce;
    
    [Header("Slide")]
    public float maxSlideSpeed = 30;

    public bool sliding;
    [FormerlySerializedAs("pressingSlide")] public bool pressingCrouch;
    private bool firstSlideCall;
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
    public bool underTerrain;
    public bool slamming;

    private void Update()
    {
        GroundCheck();
        Walled();
        SlopeCheck();
        RoofCheck();
        
        CameraRotation();

        moveDirection = move.action.ReadValue<Vector2>();
        // BigAssBall is real?!?!?!
        
        // Dash
        dashCoolDown -= Time.deltaTime;
        if ((grounded || wallRunning) && !dashing && dashCoolDown < 0) canDash = true;
    }

    private void FixedUpdate()
    {
        // if (grounded) { // Grounded
        //     if (pressingCrouch) {
        //         if (rb.velocity.magnitude < maxSpeed * 0.5) currentState = movementState.crouch; // Crouch
        //         else currentState = movementState.slide; // Slide
        //     }else currentState = movementState.ground; // Ground
        // }
        // else { // Not grounded
        //     if (walled) {
        //         if (pressingJump && !pressingCrouch && !slamming) currentState = movementState.wallrun; // Wallrun
        //         else currentState = movementState.air; // Air
        //     }else currentState = movementState.air;
        // }
        
        if (grounded) { // Grounded
            if (pressingCrouch) {
                if (rb.velocity.magnitude < maxSpeed * 0.5) currentState = Crouch; // Crouch
                else currentState = Slide; // Slide
            }else currentState = Ground; // Ground
        }
        else { // Not grounded
            if (walled) {
                if (pressingJump && !pressingCrouch && !slamming) currentState = Wallrun; // Wallrun
                else currentState = Air; // Air
            }else currentState = Air;
        }
        
        // State change
        if (currentState != previousState)
        {
            previousState.ExitState();
            currentState.EnterState();
        }
        previousState = currentState;
        
        currentState.Update(); // Movement update
        
        // Movement();
    }

    void Movement()
    {
        if (currentState != previousState)
        {
            previousState.ExitState();
            currentState.EnterState();
        }
        previousState = currentState;
        
        currentState.Update();
            
        // switch (currentState)
        // {
        //     case movementState.ground: // Basic ground movement
        //     {
        //         Ground.Update();
        //         break;
        //     }
        //     case movementState.air: // Aerial movement
        //     {
        //         Air.Update();
        //         break;
        //     }
        //     case movementState.crouch: // Crouching
        //     {
        //         Crouch.Update();
        //         break;
        //     }
        //     case movementState.slide: // Slide
        //     {
        //         Slide.Update();
        //         break;
        //     }
        //     case movementState.wallrun: // Wallrun
        //     {
        //         Wallrun.Update();
        //         break;
        //     }
        // }
    }

    // Call path: player (object in scene hierarchy) -> Player Input (component) -> Events -> player
    public void JumpInput(InputAction.CallbackContext action)
    {
        // Sets landingGrace to determine wheter to jump in Jump()
        if (action.performed)
        {
            landingGrace = Time.realtimeSinceStartup + landingGracePeriod;
            pressingJump = true;
        }
        else if (action.canceled)
        {
            pressingJump = false;
            if (wallRunning) WallJump();
            wallRunning = false;
        }
    }

    void Jump()
    {
        
    }
    
    void WallJump()
    {
        Vector3 velocity = rb.velocity;
        
        // Walljumping makes the player jump away from the wall regardless of directional movement inputs
        rb.velocity = new Vector3(velocity.x, 0, velocity.z) * 1.1f - dirToWall.normalized * 8 + Vector3.up * jumpForce * 0.6f;
        
        hasJumped = true;
        firstWallRunCall = true;
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
            if (Vector3.Distance(transform.position, startPos) > dashDistance || grounded || walled) rb.velocity *= 0.9f;
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
            firstWallRunCall = true;

            dashing = false;
        }
        else
        {
            grounded = false;
            hasJumped = false;
        }
    }

    void SlopeCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.3f, terrainLayer) && Vector3.Angle(Vector3.up, slopeHit.normal) < maxSlopeAngle && Vector3.Angle(Vector3.up, slopeHit.normal) > 0)
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
            dirToWall = wallColList[0].ClosestPoint(transform.position) - transform.position;

            dashing = false;
        }
    }
}
