using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Timers;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = System.Numerics.Vector4;

public class PlayerMovement : MonoBehaviour
{
    public InputActionReference move;
    
    [Header("Objects/Components")]
    private Rigidbody rb;
    new public Camera camera;

    [FormerlySerializedAs("speed")] [Header("Movement")]
    public float acceleration = 100;
    public float maxSpeed = 10;
    private Vector2 moveDirection;
    [Header("Slope movement")]
    public float maxSlopeAngle = 45;
    public bool onSlope;
    private RaycastHit slopeHit;
    private Vector3 slopeMoveDir;
    
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
    public bool pressingSlide;
    private bool firstSlideCall;
    private float slideForceTimer;
    private Vector3 slideDirection;

    [Header("Camera")]
    public float mouseSensitivity = 2f;
    private float cameraVerticalRotation;
    private float cameraHorizontalRotation;
    
    [Header("Checks")]
    public GameObject groundCheck;
    public LayerMask groundLayer;
    public bool grounded;
    public LayerMask wallLayer;
    public bool walled;
    [HideInInspector] public List<Collider> wallColList = new List<Collider>();
    private Vector3 dirToWall;

    // Other
    public float gravity = 20;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = gameObject.GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        GroundCheck();
        Walled();
        Jump();
        Slide();
        onSlope = SlopeCheck();

        moveDirection = move.action.ReadValue<Vector2>();
        // BigAssBall() making a comeback 2024
        
        if (wallRunning) rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        
        CameraRotation();

        if (Input.GetMouseButtonDown(1)) rb.AddForce(transform.rotation * Vector3.forward * 60, ForceMode.Impulse);
        if (Input.GetMouseButtonDown(0)) rb.AddForce(camera.transform.rotation * Vector3.forward * 60, ForceMode.Impulse);

        Debug.Log(rb.velocity.magnitude);
    }

    private void FixedUpdate()
    {
        if (!wallRunning && !sliding) Movement(); // If wallrunning, disable base movement
        else if (!sliding) WallRun();
        
        // Extra gravity
        if (!wallRunning) rb.AddForce(0, -gravity, 0);
        else rb.useGravity = false;
        if (onSlope)
        {
            rb.useGravity = false;
            rb.AddForce(-slopeHit.normal * gravity);
        }
        else
            rb.useGravity = true;
    }

    void WallRun()
    {
        Vector3 dirToWall90 = Quaternion.Euler(0, 90, 0) * dirToWall;

        float camAngle90 = Vector3.Angle(dirToWall90, transform.rotation * Vector3.forward);

        if (firstWallRunCall)
        {
            if (camAngle90 < 90)
            {
                wallRunForce = dirToWall90.normalized * acceleration;
                wallRunDirection = 1;
            }
            else
            {
                wallRunForce = dirToWall90.normalized * -acceleration;
                wallRunDirection = -1;
            }
        }

        wallRunForce = dirToWall90.normalized * (acceleration * wallRunDirection);

        firstWallRunCall = false;
        
        // Apply force to ride the wall
        if (new Vector2(rb.velocity.x, rb.velocity.z).magnitude < maxSpeed) rb.AddForce(wallRunForce, ForceMode.Force);
    }

    

    void Movement()
    {
        Vector3 velocity = rb.velocity;
        float xzSpeed = new Vector2(velocity.x, velocity.z).magnitude;
        
        // Movement force
        Vector3 movement = (moveDirection.y * transform.forward + moveDirection.x * transform.right) * acceleration;
        
        // Angle between the direction of where the player is going and the wall
        float wallAngle = Vector3.Angle(dirToWall, movement);
        float wallAngle90 = Vector3.Angle(Quaternion.Euler(0, 90, 0) * dirToWall, movement);

        float scale = wallAngle / 90f;

        // Redirects the player when moving towards walls to prevent the player from sticking to them
        if (walled && !wallRunning && Mathf.Abs(wallAngle) <= 90)
        {
            if (wallAngle90 < 90) movement = Quaternion.Euler(0, wallAngle90,0) * movement * scale;
            else movement = Quaternion.Euler(0, -(90 - wallAngle),0) * movement * scale;
        }
        
        if (grounded)
        {
            if (!onSlope && xzSpeed < maxSpeed) rb.AddForce(movement, ForceMode.Force);
            
            // Slows the player when there are no movement inputs
            if (movement == Vector3.zero && xzSpeed < maxSpeed) rb.velocity = new Vector3(velocity.x * 0.6f, velocity.y, velocity.z * 0.6f);
            
            // Normal speed limit on the ground
            if (xzSpeed > maxSpeed * 1.2) rb.AddForce(xzSpeed - maxSpeed + 1 < 3 ? -velocity * (xzSpeed - maxSpeed + 1): -velocity * 3);
        }
        else if (xzSpeed < maxSpeed)
        {
            // Movement in air
            rb.AddForce(movement / ((velocity.magnitude+1)/3), ForceMode.Force);
        }
        if (onSlope)
        {
            Vector3 slopeMovement = Vector3.ProjectOnPlane(movement, slopeHit.normal);
            
            if (velocity.magnitude < maxSpeed)
            {
                rb.AddForce(slopeMovement, ForceMode.Force);
                if (velocity.y > 0) rb.AddForce(slopeMovement * (Vector3.Angle(slopeHit.normal, Vector3.up) * 0.1f), ForceMode.Force);
            }
            
            // Angle between the moving direction and direction to the left of the slope
            float moveAngle = Vector3.Angle(velocity, Vector3.Cross(slopeHit.normal, new Vector3(slopeHit.normal.x, 0, slopeHit.normal.z)));
            float slopeScale1 = -(moveAngle / 90) + 1;
            float slopeScale2 = (moveAngle - 90) / 90;
            
            // Limits the relative left and right movement on slope when moving diagonally
            if (moveAngle < 70 && moveAngle > 20) rb.AddForce(-Vector3.Cross(slopeHit.normal, new Vector3(slopeHit.normal.x, 0, slopeHit.normal.z)).normalized * (50 * slopeScale1));
            else if (moveAngle > 110 && moveAngle < 160) rb.AddForce(Vector3.Cross(slopeHit.normal, new Vector3(slopeHit.normal.x, 0, slopeHit.normal.z)).normalized * (50 * slopeScale2));
        }
        
        // Start wallride
        if (!grounded && walled && pressingJump && moveDirection.y > 0)
            wallRunning = true;
    }
    
    
    // Call path: player -> Player Input -> Events -> player
    public void JumpInput(InputAction.CallbackContext action)
    {
        // Sets landingGrace to determine wheter to jump in Jump()
        if (action.performed)
        {
            landingGrace = Time.realtimeSinceStartup + landingGracePeriod;
            pressingJump = true;
        }

        if (action.canceled)
        {
            pressingJump = false;
            if (wallRunning) WallJump();
            wallRunning = false;
        }
    }
    void Jump()
    {
        Vector3 velocity = rb.velocity;
        Quaternion localRotation = transform.localRotation;
        // Calculates the x and z components of the velocity added to the player when jumping
        float moveX = moveDirection.x * velocity.magnitude * Mathf.Cos(localRotation.eulerAngles.y * Mathf.Deg2Rad) + moveDirection.y * velocity.magnitude * Mathf.Sin(localRotation.eulerAngles.y * Mathf.Deg2Rad);
        float moveZ = -moveDirection.x * velocity.magnitude * Mathf.Sin(localRotation.eulerAngles.y * Mathf.Deg2Rad) + moveDirection.y * velocity.magnitude * Mathf.Cos(localRotation.eulerAngles.y * Mathf.Deg2Rad);

        Vector2 velocity2 = new Vector2(velocity.x, velocity.z);
        Vector2 direction2 = new Vector2(moveX, moveZ);
        float velAngle = Mathf.Rad2Deg * Mathf.Acos((velocity.x * moveX + velocity.z * moveZ) / (velocity2.magnitude * direction2.magnitude));
        float scale = -((velAngle / 180f) * (1f + 0.2f) - 0.2f) + 0.8f;

        if (landingGrace > Time.realtimeSinceStartup && !hasJumped && grounded) //Jump if player has landed within the grace period and has not yet jumped
        {
            if (moveDirection.magnitude == 0 || velocity.magnitude > maxSpeed * 1.5f) rb.velocity = new Vector3(velocity.x, jumpForce, velocity.z);
            if (moveDirection.magnitude > 0 && velocity.magnitude < maxSpeed * 1.5f)rb.velocity = new Vector3(moveX * scale, jumpForce, moveZ * scale);

            hasJumped = true;
            timeSinceJump = Time.time;
        }
        if (timeSinceJump + 0.2 < Time.time) hasJumped = false;
    }

    void WallJump()
    {
        Vector3 velocity = rb.velocity;
        
        // Walljumping throws the player away from the wall
        rb.velocity = new Vector3(velocity.x, jumpForce, velocity.z) - dirToWall.normalized * 5;
        
        hasJumped = true;
        firstWallRunCall = true;
    }

    public void SlideInput(InputAction.CallbackContext action)
    {
        if (action.performed) pressingSlide = true;
        else if (action.canceled)
        {
            pressingSlide = false;
            sliding = false;
        }
    }

    void Slide()
    {
        // Start sliding if grounded or as soons as the player touches groung while pressing the slide key
        if (pressingSlide && grounded && !pressingJump)
        {
            sliding = true;
            slideDirection = rb.velocity;
        }

        if (sliding)
        {
            float velMagnitude = rb.velocity.magnitude;
            if (firstSlideCall)
            {
                transform.localScale = new Vector3(1, 0.5f, 1);
                rb.AddForce(slopeHit.normal * -50, ForceMode.Impulse);
                slideForceTimer = Time.time + Mathf.Lerp(0, 0.4f, velMagnitude / maxSpeed);
            }
            firstSlideCall = false;

            Vector3 movement = transform.rotation * new Vector3(moveDirection.x, 0, moveDirection.y) * acceleration;
            // Add force towards movement direction for a brief moment after starting the slide
            if (Time.time < slideForceTimer && velMagnitude < maxSpeed * 1.2f) rb.AddForce(slideDirection / 10 * acceleration / 2, ForceMode.Force);
            // Slideforce downhill
            if (Time.time > slideForceTimer && Vector3.Angle(slopeHit.normal, Vector3.up) > 0 && velMagnitude < maxSlideSpeed) rb.AddForce(new Vector3(slopeHit.normal.x, 0, slopeHit.normal.z).normalized * (acceleration * 0.5f), ForceMode.Force);
            // "Crouching" movement
            if (velMagnitude < maxSpeed * 0.5f || onSlope) rb.AddForce(Vector3.ProjectOnPlane(movement, slopeHit.normal) / 3, ForceMode.Force);
            if (velMagnitude > maxSlideSpeed) rb.AddForce(new Vector3(slopeHit.normal.x, 0, slopeHit.normal.z).normalized * -(acceleration * 0.5f), ForceMode.Force);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            firstSlideCall = true;
        }
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
    }
    
    void GroundCheck()
    {
        if (Physics.OverlapBox(groundCheck.transform.position, groundCheck.transform.localScale, Quaternion.identity, groundLayer).Length > 0)
        {
            grounded = true;
            wallRunning = false;
            firstWallRunCall = true;
        }
        else
        {
            grounded = false;
            hasJumped = false;
        }
    }

    bool SlopeCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.3f, groundLayer) && Vector3.Angle(Vector3.up, slopeHit.normal) < maxSlopeAngle && Vector3.Angle(Vector3.up, slopeHit.normal) > 0)
            return true;
        return false;
    }

    void Walled()
    {
        if (walled) dirToWall = wallColList[0].ClosestPoint(transform.position) - transform.position;
    }
}
