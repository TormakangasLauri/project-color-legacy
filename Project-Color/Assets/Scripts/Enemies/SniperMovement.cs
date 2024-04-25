using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SniperMovement : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject target;
    
    public float speed;
    public float maxDistToTarget;
    public bool LOSToTarget;
    public bool grounded;

    private NavMeshPath path;

    private LayerMask terrainLayer;

    private SniperShooting SS;
    private EnemyType ET;
    
    public enum State { idle, navmesh, los, attack };
    public State state;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = GetComponent<EnemyType>().target;
        terrainLayer = LayerMask.GetMask("Terrain");
        SS = GetComponent<SniperShooting>();
        ET = GetComponent<EnemyType>();
    }
    
    void Update()
    {
        Grounded();
        LOSToTarget = !Physics.Linecast(transform.position, target.transform.position, terrainLayer);
        path = ET.path;
    }

    private void FixedUpdate()
    {
        // States
        //  idle: not moving / wandering (?)
        //  navmesh: moving with NavMeshAgent when player is not in LOS
        //  los: moving with force away from the player when player in LOS and too close
        //  attack: attacking
        switch (state)
        {
            case State.idle:
                break;
            case State.navmesh:
                NavMeshMovement();
                break;
            case State.los:
                LOSMovement();
                break;
            case State.attack:
                Attack();
                break;
        }
    }

    private void NavMeshMovement()
    {

    }

    private void LOSMovement()
    {
        Vector3 targetPos = target.transform.position;
        Vector3 pos = transform.position;
        Vector3 directionToPlayer = new Vector3(targetPos.x - pos.x, 0, targetPos.z - pos.z).normalized;
        Vector3 movement = directionToPlayer * (speed * 10);

        // Rotate to face the player
        rb.MoveRotation(Quaternion.LookRotation(directionToPlayer));
    }

    private void Attack()
    {
        SS.moving = false;

        // State change check
        if ((target.transform.position - transform.position).magnitude < maxDistToTarget && LOSToTarget)
        {
            SS.moving = true;
            state = State.los;
        }
    }
    
    private void Grounded()
    {
        if (Physics.OverlapBox(transform.position + Vector3.down * 0.5f, new Vector3(0.3f, 1, 0.3f), Quaternion.identity, terrainLayer).Length > 0)
            grounded = true;
        else grounded = false;
    }
}
