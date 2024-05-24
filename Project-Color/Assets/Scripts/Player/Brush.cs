using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Brush : MonoBehaviour
{
    GameObject player;
    Vector3 playerPos;
    Rigidbody rb;
    PlayerAttack PA;

    public float maxRange;
    public float speed;
    public float floatTime;
    public float radius;
    public float speedLossRadius;

    public Vector3 point;

    public float rotSpeed;
    public float knockback;

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

            float dist = Vector3.Distance(pos, point);
            if (dist <= speedLossRadius)
            {
                currentSpeed = speed * dist / speedLossRadius;
            }

            return dist < 0.08;
        });
        // Float in air
        yield return new WaitForSeconds(floatTime);

        Vector3 turnPoint = transform.position - (playerPos - transform.position).normalized * 0.08f;
        enemiesHit.Clear();

        // Move towards the player
        yield return new WaitUntil(delegate
        {
            Vector3 pos = transform.position;
            Vector3 dir = (playerPos - transform.position).normalized;

            rb.velocity = dir * currentSpeed;

            float dist = Vector3.Distance(pos, turnPoint);
            if (dist <= speedLossRadius * 2)
            {
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !enemiesHit.Contains(other.gameObject))
        {
            enemiesHit.Add(other.gameObject);
            Vector3 kb = rb.velocity.normalized * knockback;
            other.gameObject.GetComponent<EnemyHealth>().Knockback(kb, ForceMode.Force);
        }
    }
}
