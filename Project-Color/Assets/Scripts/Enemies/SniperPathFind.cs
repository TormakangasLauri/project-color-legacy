using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class SniperPathFind : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject target;
    private LayerMask terrainLayer;
    private EnemyType ET;
    private SniperMovement SM;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ET = GetComponentInParent<EnemyType>();
        SM = GetComponentInParent<SniperMovement>();
        terrainLayer = LayerMask.GetMask("Terrain");
    }

    void Update()
    {
        target = ET.target;

        transform.localPosition = Vector3.zero;

        Vector3 destination = transform.position + (transform.position - target.transform.position).normalized * 10;
        destination.y = transform.position.y;

        NavMeshPath path = new NavMeshPath();
        agent.enabled = true;
        agent.CalculatePath(destination, path);
        agent.enabled = false;
        ET.path = path;
    }
}
