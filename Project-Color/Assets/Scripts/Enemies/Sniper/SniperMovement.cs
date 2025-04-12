using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SniperMovement : BaseEnemyMovement
{
    public float maxDistToTarget;

    private SniperAttack attack;
    
    public enum State { idle, find, escape, attack };
    public State state;
    
    void Start()
    {
        attack = GetComponent<SniperAttack>();
    }

    private void FixedUpdate()
    {
        // States
        //  idle: not moving
        //  find: moving using NavMeshPath when player is not in LOS
        //  escape: moving with force away from the player when player in LOS and too close
        //  attack: shoots while staying still
        switch (state)
        {
            case State.idle: Idle(); break;
            case State.find: Find(); break;
            case State.escape: Escape(); break;
            case State.attack: Attack(); break;
        }

        // Gravity
        if (rb.velocity.y < 0) rb.AddForce(Vector3.down * 20);
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
        if (path.corners.Length >= 2)
        {
            Vector3 cornerPos = path.corners[1];
            Vector3 pos = transform.position;
            Vector3 directionToTarget = new Vector3(cornerPos.x - pos.x, 0, cornerPos.z - pos.z).normalized;
            Vector3 movement = directionToTarget * (speed * 10);

            // Rotate to face the player
            rb.MoveRotation(Quaternion.LookRotation(directionToTarget));

            rb.AddForce(movement);
            if (rb.velocity.magnitude > speed) rb.AddForce(-movement);
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
        attack.moving = false;

        Vector3 targetPos = target.transform.position;
        Vector3 pos = transform.position;
        Vector3 directionToTarget = new Vector3(targetPos.x - pos.x, 0, targetPos.z - pos.z).normalized;
        // Rotate to face the player
        rb.MoveRotation(Quaternion.LookRotation(directionToTarget));

        // State change check
        if ((targetPos - pos).magnitude < maxDistToTarget && targetPos.y > pos.y - 1 && LOSToTarget)
        {
            attack.moving = true;
            state = State.escape;
            GetComponentInChildren<SniperPathFind>().enabled = true;
            GetComponentInChildren<PathFind>().enabled = false;
        }
        else if (!LOSToTarget)
        {
            if (Random.value > -1)
            {
                state = State.find;
                GetComponentInChildren<SniperPathFind>().enabled = false;
                GetComponentInChildren<PathFind>().enabled = true;
            }
            else
            {
                state = State.idle;
            }

        }
    }
}
