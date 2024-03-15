using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class playermovement : MonoBehaviour
{
    private Vector2 moveDirection; //moveDirecton

    public InputActionReference move;

    public Rigidbody rb;

    public float speed;
    
    public Transform player;
    public float mouseSensitivity = 2f;
    private float cameraVerticalRotation;
    private float cameraHorizontalRotation;

    public Camera camera;

    bool lockedCursor = true;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();
        // BigAssBall() making a comeback 2024

        // Rotate the Camera around its local X axis
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        camera.transform.localRotation = Quaternion.Euler(cameraVerticalRotation, 0f, 0f);

        // Rotate the Player Object around its Y axis
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        cameraHorizontalRotation -= inputX;
        transform.localRotation = Quaternion.Euler(0f, cameraHorizontalRotation, 0f);
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(moveDirection.x * speed, 0, moveDirection.y * speed);
    }
}
