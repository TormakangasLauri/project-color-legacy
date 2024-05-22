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

    public float maxRange;
    public float speed;
    public float floatTime;
    public float radius;
    public float speedLossRadius;

    public Vector3 point;

    public float rotSpeed;
    public float knockback;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();

        StartCoroutine(Move());
    }

    private void Update()
    {
        playerPos = player.transform.position;
    }

    IEnumerator Move()
    {
        Vector3 startPos = transform.position;
        float currentSpeed = speed;
        Vector3 dir = (point - startPos).normalized;

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
        yield return new WaitForSeconds(floatTime);
        Vector3 turnPoint = transform.position - (playerPos - transform.position).normalized * 0.08f;
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
    }

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * rotSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Vector3 kb = rb.velocity.normalized * knockback;
            other.gameObject.GetComponent<EnemyHealth>().Knockback(kb, ForceMode.Force);
        }
    }
}
