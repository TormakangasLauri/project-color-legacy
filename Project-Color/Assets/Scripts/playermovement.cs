using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class playermovement : MonoBehaviour
{
    public InputActionReference move;

    public Transform player;
    public Rigidbody rb;
    public Camera camera;

    private Vector2 moveDirection; //moveDirecton
    [Header("Values")]
    public float speed;
    public float maxSpeed = 50;
    public float jumpForce;
    public float gravity;
    public float landinggraceperiod = 1f;
    private float landinggrace;
    
    [Header("Camera")]
    public float mouseSensitivity = 2f;
    private float cameraVerticalRotation;
    private float cameraHorizontalRotation;
    
    [Header("Checks")]
    public GameObject groundCheck;
    public LayerMask groundLayer;
    public bool grounded;
    private bool hasjumped;


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        GroundCheck();
        if (landinggrace > Time.realtimeSinceStartup && !hasjumped && grounded) //Jump if player has landed within the grace period and has not yet jumped
        {
            rb.AddForce(Vector3.up * jumpForce);
            hasjumped = true;
        }
        moveDirection = move.action.ReadValue<Vector2>();
        // BigAssBall() making a comeback 2024
        
        CameraRotation();
    }

    private void FixedUpdate()
    {
        // Adds force to move the player
        Vector3 movement = new Vector3(moveDirection.x * speed, 0, moveDirection.y * speed);
        rb.AddRelativeForce(movement);
        // Normal speed limit
        if (rb.velocity.magnitude > maxSpeed) rb.AddRelativeForce(-movement);

        // Extra gravity
        rb.AddRelativeForce(0 , -gravity,0);
    }

    // Call path: player -> Player Input -> Events -> player
    public void Jump(InputAction.CallbackContext action)
    {
        if (action.performed) landinggrace = Time.realtimeSinceStartup + landinggraceperiod;
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
            hasjumped = false;
        }
    }
}
