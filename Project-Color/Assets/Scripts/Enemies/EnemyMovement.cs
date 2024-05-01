using System;
using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem.HID;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody rb;
    private EnemyType ET;

    private GameObject target;
    
    [HideInInspector] public LayerMask terrainLayer;
    public NavMeshPath path;

    public float speed;
    [HideInInspector] public float stopDistance;
    public bool LOSToTarget;
    public bool grounded;
    
    private float stateSwitchTimer;

    public enum State { idle, navmesh, los, attack };
    public State state;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ET = GetComponent<EnemyType>();
        target = GetComponent<EnemyType>().target;
        terrainLayer = LayerMask.GetMask("Terrain");

        state = State.navmesh;
        path = new NavMeshPath();
    }

    private void Update()
    {
        Grounded();
        LOSToTarget = !Physics.Linecast(transform.position, target.transform.position, terrainLayer);

        stateSwitchTimer -= Time.deltaTime;

        path = ET.path;
    }

    private void FixedUpdate()
    {
        // States
        //  idle: not moving / wandering, will finish later
        //  navmesh: moving using a NavMeshPath when player is not in LOS
        //  los: moving with force towards the player when player is in LOS
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
        }

        // Gravity
        if (rb.velocity.y < 0) rb.AddForce(Vector3.down * 20);
    }

    private void NavMeshMovement()
    {
        if (path.corners.Length >= 2)
        {
            Vector3 targetPos = target.transform.position;
            Vector3 cornerPos = path.corners[1];
            Vector3 pos = transform.position;
            Vector3 directionToTarget = new Vector3(cornerPos.x - pos.x, 0, cornerPos.z - pos.z).normalized;
            Vector3 movement = directionToTarget * (speed * 10);

            // Rotate to face the player
            rb.MoveRotation(Quaternion.LookRotation(directionToTarget));

            float distOnXZ = Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(targetPos.x, 0, targetPos.z));

            // Moving when not in stopping distance of the target
            if (distOnXZ > stopDistance + stopDistance / 2) rb.AddForce(movement);
            // Slow down enemy when in stopping distance
            else if (rb.velocity.magnitude > 0.5) rb.AddForce(-rb.velocity * 2);
            // Speed limit
            if (rb.velocity.magnitude > speed) rb.AddForce(-movement);
            if (distOnXZ < 2)
            {
                rb.AddForce(-movement / 3);
                if (rb.velocity.magnitude > speed) rb.AddForce(movement);
            }
        }

        RaycastHit hit;
        Physics.Raycast(target.transform.position, Vector3.down, out hit, 100, terrainLayer);

        // State change check
        if (LOSToTarget && (path.corners.Length <= 2 || hit.point.y <= transform.position.y - transform.localScale.y / 2) && stateSwitchTimer < 0)
        {
            stateSwitchTimer = 1;
            state = State.los;
        }
    }

    private void LOSMovement()
    {
        Vector3 targetPos = target.transform.position;
        Vector3 pos = transform.position;
        Vector3 directionToPlayer = new Vector3(targetPos.x - pos.x, 0, targetPos.z - pos.z).normalized;
        Vector3 movement = directionToPlayer * (speed * 10);
        
        // Rotate to face the player
        rb.MoveRotation(Quaternion.LookRotation(directionToPlayer));

        float distOnXZ = Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(targetPos.x, 0, targetPos.z));
        
        // Moving when not in stopping distance of the player
        if (distOnXZ > stopDistance + stopDistance/2) rb.AddForce(movement);
        // Slow down enemy when in stopping distance
        else if (rb.velocity.magnitude > 0.5) rb.AddForce(-rb.velocity * 2);
        // Speed limit
        if (rb.velocity.magnitude > speed) rb.AddForce(-movement);
        if (distOnXZ < 2)
        {
            rb.AddForce(-movement/3);
            if (rb.velocity.magnitude > speed) rb.AddForce(movement);
        }
        
        // State change check
        RaycastHit hit;
        Physics.Raycast(target.transform.position, Vector3.down, out hit, 100, terrainLayer);
        RaycastHit enemyHit;
        Physics.Raycast(transform.position, Vector3.down, out enemyHit, 100, terrainLayer);
        if ((path.corners.Length > 2 || !LOSToTarget) && hit.point.y + 0.1 >= enemyHit.point.y && grounded && stateSwitchTimer < 0)
        {
            stateSwitchTimer = 1;
            
            state = State.navmesh;
        }
    }

    private void Grounded()
    {
        Vector3 s = transform.localScale;
        if (Physics.OverlapBox(transform.position + Vector3.down * s.y/2, new Vector3(s.x * 0.3f, s.y * 1, s.z * 0.3f), Quaternion.identity, terrainLayer).Length > 0)
            grounded = true;
        else grounded = false;
    }
}
