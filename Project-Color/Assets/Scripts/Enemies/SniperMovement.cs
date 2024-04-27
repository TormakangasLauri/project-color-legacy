using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SniperMovement : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject target;
    private NavMeshAgent agent;
    
    public float speed;
    public float maxDistToTarget;
    public bool LOSToTarget;
    public bool grounded;

    private NavMeshPath path;

    private LayerMask terrainLayer;

    private SniperShooting SS;
    private EnemyType ET;
    
    public enum State { idle, find, escape, attack };
    public State state;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = GetComponent<EnemyType>().target;
        agent = GetComponent<NavMeshAgent>();
        terrainLayer = LayerMask.GetMask("Terrain");
        SS = GetComponent<SniperShooting>();
        ET = GetComponent<EnemyType>();

        agent.speed = speed;
        state = State.idle;
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
        //  find: moving with NavMeshAgent when player is not in LOS
        //  escape: moving with force away from the player when player in LOS and too close
        //  attack: shoots while staying still
        switch (state)
        {
            case State.idle:
                Idle();
                break;
            case State.find:
                Find();
                break;
            case State.escape:
                Escape();
                break;
            case State.attack:
                Attack();
                break;
        }
    }

    private void Idle()
    {
        if (LOSToTarget)
        {
            state = State.attack;
        }
    }

    private void Find()
    {
        RaycastHit hit;
        Physics.Raycast(target.transform.position, Vector3.down, out hit, 100, terrainLayer);

        agent.SetDestination(hit.point);

        // Rotate towards the next corner on path
        if (agent.path.corners.Length >= 2)
        {
            Vector3 targetPos = target.transform.position;
            Vector3 nextCorner = agent.path.corners[1];
            Vector3 pos = transform.position;
            Vector3 directionToPlayer = new Vector3(nextCorner.x - pos.x, 0, nextCorner.z - pos.z).normalized;
            rb.MoveRotation(Quaternion.LookRotation(directionToPlayer));
        }

        // State change check
        if (LOSToTarget)
        {
            StartCoroutine(FindToAttack());
        }
    }
    private IEnumerator FindToAttack()
    {
        yield return new WaitForSeconds(0.1f);
        agent.enabled = false;
        state = State.attack;
    }

    private void Escape()
    {
        Vector3 targetPos = target.transform.position;
        Vector3 pos = transform.position;

        Vector3 directionToNextCorner;
        if (path.corners.Length >= 2) directionToNextCorner = new Vector3(path.corners[1].x - pos.x, 0, path.corners[1].z - pos.z).normalized;
        else directionToNextCorner = new Vector3(pos.x - targetPos.x, 0, pos.z - targetPos.z).normalized;

        Vector3 movement = directionToNextCorner * (speed * 10);

        // Rotate to face the player
        rb.MoveRotation(Quaternion.LookRotation(directionToNextCorner));

        rb.AddForce(movement);
        if (rb.velocity.magnitude > speed) rb.AddForce(-movement);

        // State change check
        float distToTarget = Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(targetPos.x, 0, targetPos.z));
        if (distToTarget > maxDistToTarget)
        {
            state = State.attack;
        }
    }

    private void Attack()
    {
        SS.moving = false;

        Vector3 targetPos = target.transform.position;
        Vector3 pos = transform.position;
        Vector3 directionToTarget = new Vector3(targetPos.x - pos.x, 0, targetPos.z - pos.z).normalized;
        // Rotate to face the player
        rb.MoveRotation(Quaternion.LookRotation(directionToTarget));

        // State change check
        if ((targetPos - pos).magnitude < maxDistToTarget && LOSToTarget)
        {
            SS.moving = true;
            state = State.escape;
        }
        else if (!LOSToTarget)
        {
            if (Random.value > 0.5)
            {
                agent.enabled = true;
                state = State.find;
            }
            else
            {
                state = State.idle;
            }

        }
    }
    
    private void Grounded()
    {
        if (Physics.OverlapBox(transform.position + Vector3.down * 0.5f, new Vector3(0.3f, 1, 0.3f), Quaternion.identity, terrainLayer).Length > 0)
            grounded = true;
        else grounded = false;
    }
}
