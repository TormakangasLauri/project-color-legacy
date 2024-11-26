using System;
using System.Collections;
using System.Collections.Generic;
using OpenCover.Framework.Model;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemyMovement : MonoBehaviour
{
    protected Rigidbody rb;
    protected EnemyType _enemyType;
    public NavMeshPath path;
    
    protected GameObject target;
    
    public float speed;
    [HideInInspector] public float stopDistance;

    protected LayerMask terrainLayer;
    
    public bool LOSToTarget;
    public bool grounded;

    private void Awake()
    {
        path = new NavMeshPath();
        terrainLayer = LayerMask.GetMask("Terrain");
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _enemyType = GetComponent<EnemyType>();
        target = GameObject.FindWithTag("Player");
    }
    
    private void Update()
    {
        GroundCheck();
        LOSToTarget = !Physics.Linecast(transform.position, target.transform.position, terrainLayer);

        path = _enemyType.path;
    }
    
    private void GroundCheck()
    {
        Vector3 s = transform.localScale;
        if (Physics.OverlapBox(transform.position + Vector3.down * s.y / 2, new Vector3(s.x * 0.3f, s.y * 1, s.z * 0.3f), Quaternion.identity, terrainLayer).Length > 0)
            grounded = true;
        else grounded = false;
    }
}
