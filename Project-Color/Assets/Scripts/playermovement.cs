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
    
    public Rigidbody rb;
    new public Camera camera;

    private Vector2 moveDirection; //moveDirecton
    [Header("Values")]
    public float speed;
    public float maxSpeed = 50;
    public float jumpForce;
    public float gravity;
    public float landingGracePeriod = 0.2f;
    private float landingGrace;
    
    [Header("Camera")]
    public float mouseSensitivity = 2f;
    private float cameraVerticalRotation;
    private float cameraHorizontalRotation;
    
    [Header("Checks")]
    public GameObject groundCheck;
    public LayerMask groundLayer;
    public bool grounded;
    private bool hasJumped;
    private float timeSinceJump;


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        GroundCheck();
        Jump();

        moveDirection = move.action.ReadValue<Vector2>();
        // BigAssBall() making a comeback 2024
        
        CameraRotation();
    }

    private void FixedUpdate()
    {
        Movement();
        // Extra gravity
        rb.AddRelativeForce(0 , -gravity,0);
    }

    void Movement()
    {
        // Apparently is more efficient using this var
        Vector3 velocity = rb.velocity;
        
        // Adds force to move the player
        Vector3 movement = new Vector3(moveDirection.x * speed, 0, moveDirection.y * speed);
        if (grounded)
        {
            rb.AddRelativeForce(movement);
            // Normal speed limit
            if (velocity.magnitude > maxSpeed) rb.AddRelativeForce(-movement);
            // Slows the player when there are no movement inputs
            if (movement == Vector3.zero && velocity.magnitude > 2) rb.velocity = new Vector3(velocity.x * 0.7f, velocity.y, velocity.z * 0.7f);
        }
        else
        {
            // Movement and speed limit in air
            if (velocity.magnitude < 10) rb.AddRelativeForce(movement / ((velocity.magnitude+1)/3));
            
        }

        Debug.Log(velocity.magnitude);
    }
    
    // Call path: player -> Player Input -> Events -> player
    public void JumpInput(InputAction.CallbackContext action)
    {
        // Sets landingGrace to determine wheter to jump in Jump()
        if (action.performed) landingGrace = Time.realtimeSinceStartup + landingGracePeriod;
    }
    void Jump()
    {
        Vector3 velocity = rb.velocity;
        Quaternion localRotation = transform.localRotation;
        // Calculates the x and z components of the velocity added to the player when jumping
        float velX = moveDirection.x * velocity.magnitude * Mathf.Cos(localRotation.eulerAngles.y * Mathf.Deg2Rad) + moveDirection.y * velocity.magnitude * Mathf.Sin(localRotation.eulerAngles.y * Mathf.Deg2Rad);
        float velZ = -moveDirection.x * velocity.magnitude * Mathf.Sin(localRotation.eulerAngles.y * Mathf.Deg2Rad) + moveDirection.y * velocity.magnitude * Mathf.Cos(localRotation.eulerAngles.y * Mathf.Deg2Rad);

        Vector2 velocity2 = new Vector2(velocity.x, velocity.z);
        Vector2 direction2 = new Vector2(velX, velZ);
        float velAngle = Mathf.Rad2Deg * Mathf.Acos((velocity.x * velX + velocity.z * velZ) / (velocity2.magnitude * direction2.magnitude));
        float scale = -((velAngle / 180f) * (1f + 0.2f) - 0.2f) + 0.8f;

        if (landingGrace > Time.realtimeSinceStartup && !hasJumped && grounded) //Jump if player has landed within the grace period and has not yet jumped
        {
            if (moveDirection.x == 0 && moveDirection.y == 0)
                rb.velocity = new Vector3(0, jumpForce, 0);
            else
                rb.velocity = new Vector3(velX * scale, jumpForce, velZ * scale);
            hasJumped = true;
            timeSinceJump = Time.time;
        }
        if (timeSinceJump + 0.2 < Time.time) hasJumped = false;
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
            grounded = true;
        else
        {
            grounded = false;
            hasJumped = false;
        }
    }
}
