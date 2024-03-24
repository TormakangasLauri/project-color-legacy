using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public InputActionReference move;
    
    private Rigidbody rb;
    new public Camera camera;

    private Vector2 moveDirection; //moveDirecton
    [Header("Values")]
    public float speed = 100;
    public float maxSpeed = 10;
    public float jumpForce = 12;
    public float gravity = 20;
    public float landingGracePeriod = 0.2f;
    private float landingGrace;
    private Vector3 wallRunForce;
    
    [Header("Camera")]
    public float mouseSensitivity = 2f;
    private float cameraVerticalRotation;
    private float cameraHorizontalRotation;
    
    [Header("Checks")]
    public GameObject groundCheck;
    public LayerMask groundLayer;
    public bool grounded;
    public GameObject wallCheck;
    public LayerMask wallLayer;
    public bool walled;
    [HideInInspector] public List<Collider> wallColList = new List<Collider>();
    private Vector3 dirToWall;

    [Header("Bools")]
    public bool wallRiding;
    private bool firstWallRideCall = true;
    public bool pressingJump;
    private bool hasJumped;
    private float timeSinceJump;

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

        moveDirection = move.action.ReadValue<Vector2>();
        // BigAssBall() making a comeback 2024
        
        if (wallRiding) rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        
        CameraRotation();
    }

    private void FixedUpdate()
    {
        if (!wallRiding) Movement(); // If wallrunning, disable base movement
        else WallRide();
        
        // Extra gravity
        if (!wallRiding) rb.AddRelativeForce(0 , -gravity,0);
    }

    void WallRide()
    {
        Vector2 dirToWall2 = new Vector2(dirToWall.x, dirToWall.z);
        Vector3 dirToWall90 = Quaternion.Euler(0, 90, 0) * dirToWall;

        Vector3 forward = transform.rotation * Vector3.forward;
        Vector3 forward90 = Quaternion.Euler(0, 90, 0) * forward;
        
        float camAngle90 = Mathf.Rad2Deg * Mathf.Acos((forward90.x * dirToWall.x + forward90.z * dirToWall.z) / (dirToWall2.magnitude * forward90.magnitude));
        
        if (firstWallRideCall) wallRunForce = dirToWall90.normalized * (Mathf.Sign(camAngle90 - 90) * speed);
        firstWallRideCall = false;
        
        // Apply force to ride the wall
        rb.velocity = (wallRunForce / 10);
    }

    

    void Movement()
    {
        Vector3 velocity = rb.velocity;
        // Movement force
        Vector3 movement = new Vector3(moveDirection.x * speed, 0, moveDirection.y * speed);
        
        Vector3 worldMovement = transform.rotation * movement;
        Vector2 dirToWall2 = new Vector2(dirToWall.x, dirToWall.z);
        Vector2 direction2 = new Vector2(worldMovement.x, worldMovement.z);
        // Angle between the direction of where the player is going and the wall
        float angle = Mathf.Rad2Deg * Mathf.Acos((worldMovement.x * dirToWall.x + worldMovement.z * dirToWall.z) / (dirToWall2.magnitude * direction2.magnitude));

        float scale = (angle / 90f);
        float scale2 = -scale + 1; 
        
        if (grounded)
        {
            rb.AddRelativeForce(movement);
            // Normal speed limit
            if (velocity.magnitude > maxSpeed) rb.AddRelativeForce(-movement);
            // Slows the player when there are no movement inputs
            if (movement == Vector3.zero && velocity.magnitude > 2) rb.velocity = new Vector3(velocity.x * 0.6f, velocity.y, velocity.z * 0.6f);
        }
        else
        {
            // Movement and speed limit in air
            rb.AddRelativeForce(movement / ((velocity.magnitude+1)/3));
        }

        if (walled && Mathf.Abs(angle) <= 90 && velocity.magnitude < maxSpeed)
            rb.AddForce(-dirToWall * (movement.magnitude * 0.75f * scale2)); // This is stupid. It is so stupid, that it works, therefore it is OK 👍.

        if (!grounded && pressingJump && movement.z > 0 && walled)
            wallRiding = true;
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
            if (wallRiding) WallJump();
            wallRiding = false;
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
            if (moveDirection.x == 0 && moveDirection.y == 0)
                rb.velocity = new Vector3(velocity.x, jumpForce, velocity.z);
            else
                rb.velocity = new Vector3(moveX * scale, jumpForce, moveZ * scale);
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
        firstWallRideCall = true;
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
            wallRiding = false;
        }
            
        else
        {
            grounded = false;
            hasJumped = false;
        }
    }

    void Walled()
    {
        foreach (Collider col in wallColList)
        {
            dirToWall = col.ClosestPoint(transform.position) - transform.position;
        }
    }
}
