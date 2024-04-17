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
        //  los: moving with force when player in LOS
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
        if (LOSToPlayer)
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
        
        rb.angularVelocity = Vector3.up * (-5 * Mathf.Deg2Rad * Vector3.SignedAngle(directionToPlayer, transform.forward, Vector3.up));
        transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);

        rb.AddForce(movement);
        if (rb.velocity.magnitude > speed) rb.AddForce(-movement);
        
        // State change check
        if (!LOSToPlayer)
        {
            agent.enabled = true;
            state = State.navmesh;
        }
    }
}
