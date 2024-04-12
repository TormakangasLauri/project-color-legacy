using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Fly_test : MonoBehaviour
{
    public InputActionReference moveInput;
    private Vector2 moveDirection;
    private Rigidbody rb;
    public float baseSpeed;
    private float speed;
    public float mouseSensitivity;
    public Camera cam;
    private float cameraHorizontalRotation;
    private float cameraVerticalRotation;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Update()
    {
        moveDirection = moveInput.action.ReadValue<Vector2>();
        
        // Movement
        if (Input.GetKey(KeyCode.LeftShift)) speed = 2 * baseSpeed;
        else speed = baseSpeed;
        rb.velocity = transform.rotation * new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.y * speed);
        if (Input.GetKey(KeyCode.Space)) rb.velocity = new Vector3(rb.velocity.x, speed, rb.velocity.z);
        else if (Input.GetKey(KeyCode.LeftControl)) rb.velocity = new Vector3(rb.velocity.x, -speed, rb.velocity.z);
        else rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        
        // Camera
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(cameraVerticalRotation, 0f, 0f);
        
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        cameraHorizontalRotation += inputX;
        transform.rotation = Quaternion.Euler(0f, cameraHorizontalRotation, 0f); 
    }
}
