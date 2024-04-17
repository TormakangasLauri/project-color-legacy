using System;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;

    public GameObject target;
    public LayerMask terrainLayer;

    public float speed;
    public float stopDistance;
    
    public bool LOSToPlayer;

    public enum State { idle, navmesh, los, attack };
    public State state;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindWithTag("Player");

        agent.speed = speed;
        state = State.navmesh;
        
        EnemyController.inst.basicEnemies.Add(gameObject);
    }

    private void Update()
    {
        LOSToPlayer = !Physics.Linecast(transform.position, target.transform.position, terrainLayer);
    }

    private void FixedUpdate()
    {
        // States
        //  idle: not moving / wandering, will finish later
        //  navmesh: moving with NavMeshAgent when player is not in LOS
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
    }

    private void NavMeshMovement()
    {
        agent.SetDestination(target.transform.position);

        // State change check
        if (LOSToPlayer && agent.path.corners.Length <= 2)
        {
            agent.enabled = false;
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
        //rb.angularVelocity = Vector3.up * (-5 * Mathf.Deg2Rad * Vector3.SignedAngle(directionToPlayer, transform.forward, Vector3.up));
        rb.MoveRotation(Quaternion.LookRotation(directionToPlayer));
        // Rotation limiter
        //transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);

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
        agent.enabled = true;
        agent.SetDestination(targetPos);
        if (agent.path.corners.Length > 2 || !LOSToPlayer)
        {
            Debug.Log("!");
            state = State.navmesh;
        }
        else agent.enabled = false;
    }

    private void OnDestroy()
    {
        EnemyController.inst.basicEnemies.Remove(gameObject);
    }
}
