using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class PathFind : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject target;
    private LayerMask terrainLayer;
    private EnemyMovement EM;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        EM = GetComponentInParent<EnemyMovement>();
        target = EM.target;
        terrainLayer = EM.terrainLayer;
    }

    void Update()
    {
        transform.localPosition = Vector3.zero;
        
        RaycastHit hit;
        Physics.Raycast(target.transform.position, Vector3.down, out hit, 100, terrainLayer);
        
        NavMeshPath path = new NavMeshPath();
        agent.enabled = true;
        agent.CalculatePath(hit.point, path);
        agent.enabled = false;
        EM.path = path;
    }
}
