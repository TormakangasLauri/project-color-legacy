using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;
    public GameObject target;
    private Vector3 targetPos;
    public LayerMask terrainLayer;
    public GameObject pathFind;

    private void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        pathFind.transform.position = transform.position;
    }

    private void FixedUpdate()
    {
        targetPos = GetComponentInChildren<PathFind>().targetPos;
        rb.AddForce((targetPos - transform.position).normalized * 50);
        if (rb.velocity.magnitude > 5) rb.AddForce((targetPos - transform.position).normalized * -50);
    }
}
