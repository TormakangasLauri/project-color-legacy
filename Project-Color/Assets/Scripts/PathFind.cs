using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class PathFind : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject target;
    [HideInInspector] public Vector3 targetPos;
    private LayerMask terrainLayer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GetComponentInParent<EnemyMovement>().target;
        terrainLayer = GetComponentInParent<EnemyMovement>().terrainLayer;
    }

    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(target.transform.position, Vector3.down, out hit, terrainLayer);
        
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(hit.point, path);
        agent.path = path;
        targetPos = agent.steeringTarget;
    }
}
