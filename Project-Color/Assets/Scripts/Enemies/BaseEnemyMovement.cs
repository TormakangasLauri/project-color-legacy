using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static ShaderEffect_Unsync;
using static UnityEditor.PlayerSettings;

public abstract class BaseEnemyMovement : MonoBehaviour
{
    protected Rigidbody rb;
    protected EnemyType _enemyType;
    protected EnemyAttack _enemyAttack;
    public NavMeshPath path;
    
    public GameObject target;
    
    public float speed;
    public float stopDistance;

    private Vector3 velocityBeforePause = Vector3.positiveInfinity;
    private bool velocityStored = false;

    protected LayerMask terrainLayer;
    
    public bool LOSToTarget;
    public bool grounded;

    private void Awake()
    {
        path = new NavMeshPath();
        terrainLayer = LayerMask.GetMask("Terrain");
        _enemyType = GetComponent<EnemyType>();
        _enemyAttack = GetComponent<EnemyAttack>();

        rb = GetComponent<Rigidbody>();
        target = GameObject.FindWithTag("Player");
    }
    
    private void Update()
    {
        target = _enemyType.target;

        GroundCheck();
        LOSToTarget = !Physics.Linecast(transform.position, target.transform.position, terrainLayer);

        path = _enemyType.path;

        if (TimeController.paused && !velocityStored) // On pause
        {
            velocityBeforePause = rb.velocity; // Store velocity if not already
            velocityStored = true;
            rb.isKinematic = true;
        }
        else if (!TimeController.paused && velocityStored) // On unpause
        {
            rb.velocity = velocityBeforePause; // Return the stored velocity
            velocityStored = false;
            rb.isKinematic = false;
        }
    }
    
    private void GroundCheck()
    {
        Vector3 s = transform.localScale;
        if (Physics.OverlapBox(transform.position + Vector3.down * s.y / 2, new Vector3(s.x * 0.3f, s.y * 1, s.z * 0.3f), Quaternion.identity, terrainLayer).Length > 0)
            grounded = true;
        else grounded = false;
    }

    protected void Move(Vector3 movementTarget, bool seperateRotation = false, float speedMult = 1)
    {
        if (grounded)
        {
            Vector3 pos = transform.position;
            float distOnXZ = Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(movementTarget.x, 0, movementTarget.z));

            Vector3 projectedDirection;

            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, terrainLayer);

            if (!seperateRotation) // Face movement target, usually path corner
            {
                Vector3 directionToTarget = new Vector3(movementTarget.x - pos.x, 0, movementTarget.z - pos.z).normalized;

                // Calculate rotation
                Quaternion lookDirection = Quaternion.LookRotation(directionToTarget);
                // Apply rotation
                transform.rotation = Quaternion.Lerp(transform.rotation, lookDirection, 5 * _enemyType.timeScale * Time.deltaTime);

                // Calculate movement direction based on ground normal
                projectedDirection = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;
            }
            else // Face target (player)
            {
                Vector3 targetPos = target.transform.position;
                Vector3 dirToTarget = new Vector3(targetPos.x - pos.x, 0, targetPos.z - pos.z).normalized;

                projectedDirection = Vector3.ProjectOnPlane(dirToTarget, hit.normal).normalized;
            }

            Vector3 movementForce = projectedDirection * speed * speedMult;

            // Move
            rb.AddForce(movementForce);
            if (rb.velocity.magnitude > speed * _enemyType.timeScale) rb.AddForce(-movementForce); // Speed limit
        }
    }

    protected void Rotate()
    {
        // Calculate rotation to face the player
        Vector3 targetPos = target.transform.position;
        Quaternion lookDirection = Quaternion.LookRotation(new Vector3(targetPos.x - transform.position.x, 0, targetPos.z - transform.position.z).normalized);

        // Apply rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, lookDirection, 5 * _enemyType.timeScale * Time.deltaTime);

    }
}
