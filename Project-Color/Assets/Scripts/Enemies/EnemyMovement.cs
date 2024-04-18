using System;
using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private NavMeshAgent pathFinder;
    private Rigidbody rb;
    public GameObject target;
    
    public LayerMask terrainLayer;
    public NavMeshPath path;

    public float speed;
    public float stopDistance;
    public bool LOSToPlayer;
    public bool grounded;

    private float stateSwitchTimer;

    public enum State { idle, navmesh, los, attack };
    public State state;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        pathFinder = GetComponentInChildren<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindWithTag("Player");

        agent.speed = speed;
        state = State.navmesh;
        
        EnemyController.inst.basicEnemies.Add(gameObject);
    }

    private void Update()
    {
        Grounded();
        LOSToPlayer = !Physics.Linecast(transform.position, target.transform.position, terrainLayer);

        stateSwitchTimer -= Time.deltaTime;
        
        // Debug.Log(path.corners.Length);
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
        RaycastHit hit;
        Physics.Raycast(target.transform.position, Vector3.down, out hit, 100, terrainLayer);
        
        agent.SetDestination(hit.point);
        Vector3 targetPos = target.transform.position;
        Vector3 pos = transform.position;
        Vector3 directionToPlayer = new Vector3(targetPos.x - pos.x, 0, targetPos.z - pos.z).normalized;
        rb.MoveRotation(Quaternion.LookRotation(directionToPlayer));
        
        // State change check
        if (LOSToPlayer && (path.corners.Length <= 2 || hit.point.y <= transform.position.y - 1) && stateSwitchTimer < 0)
        {
            stateSwitchTimer = 1;
            StartCoroutine(NavMeshToLOS());
        }
    }

    private IEnumerator NavMeshToLOS()
    {
        yield return new WaitForSeconds(0.5f);
        agent.enabled = false;
        state = State.los;
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
        
        // Gravity
        if (rb.velocity.y < 0) rb.AddForce(Vector3.down * 20);
        
        // State change check
        RaycastHit hit;
        Physics.Raycast(target.transform.position, Vector3.down, out hit, 100, terrainLayer);
        if ((path.corners.Length > 2 || !LOSToPlayer) && hit.point.y + 0.1 >= transform.position.y - 1 && grounded && stateSwitchTimer < 0)
        {
            stateSwitchTimer = 1;
            
            agent.enabled = true;
            state = State.navmesh;
        }
    }

    private void Grounded()
    {
        if (Physics.OverlapBox(transform.position + Vector3.down * 0.5f, new Vector3(0.3f, 1, 0.3f), Quaternion.identity, terrainLayer).Length > 0)
            grounded = true;
        else grounded = false;
    }

    private void OnDestroy()
    {
        EnemyController.inst.basicEnemies.Remove(gameObject);
    }
}
