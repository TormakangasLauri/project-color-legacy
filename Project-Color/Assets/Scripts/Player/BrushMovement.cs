using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BrushMovement : MonoBehaviour
{
    GameObject player;
    Vector3 playerPos;
    Rigidbody rb;
    PlayerAttack PA;

    public float speed;
    public float floatTime;
    public float radius;
    public float speedLossRadius;

    public Vector3 point;

    public float rotSpeed;
    public float knockback;
    public float damage;

    List<GameObject> enemiesHit = new List<GameObject>();

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();
        PA = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerAttack>();

        StartCoroutine(Move());
    }

    private void Update()
    {
        playerPos = player.transform.position;
    }

    IEnumerator Move()
    {
        PA.canAttack = false;

        Vector3 startPos = transform.position;
        float currentSpeed = speed;
        Vector3 dir = (point - startPos).normalized;

        // Move towards path end point
        float t = floatTime;
        yield return new WaitUntil(delegate
        {
            Vector3 pos = transform.position;

            rb.velocity = dir * currentSpeed;

            // Decrease speed when close enough to the target point
            float dist = Vector3.Distance(pos, point);
            if (dist <= speedLossRadius)
            {
                // Speed scales according to the distance to target point
                currentSpeed = speed * dist / speedLossRadius;
            }

            return dist < 0.08;
        });
        // Float in air
        yield return new WaitForSeconds(floatTime);

        // New point used to scale the speed when returning to the player
        Vector3 turnPoint = transform.position - (playerPos - transform.position).normalized * 0.08f;
        // Clear enemies from the list so they can get hit again
        enemiesHit.Clear();

        // Move towards the player
        yield return new WaitUntil(delegate
        {
            Vector3 pos = transform.position;
            Vector3 dir = (playerPos - transform.position).normalized;

            rb.velocity = dir * currentSpeed;

            // Increase speed when starting to return to the player
            float dist = Vector3.Distance(pos, turnPoint);
            if (dist <= speedLossRadius * 2)
            {
                // Speed scales according to the distance to target point
                currentSpeed = speed * dist / speedLossRadius;
            }

            return Vector3.Distance(transform.position, playerPos) < radius;
        });

        Destroy(gameObject);
        PA.canAttack = true;
    }

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * rotSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hits the enemy only if it's not hit yet on the current movement path
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !enemiesHit.Contains(other.gameObject))
        {
            enemiesHit.Add(other.gameObject);
            Vector3 kb = rb.velocity.normalized * knockback;
            EnemyHealth EH = other.gameObject.GetComponent<EnemyHealth>();
            
            EH.TakeDamage(damage);
            EH.Knockback(kb, ForceMode.Force);
        }
    }
}
