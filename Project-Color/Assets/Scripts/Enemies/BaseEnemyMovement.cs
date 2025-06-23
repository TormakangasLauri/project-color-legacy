using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
}
