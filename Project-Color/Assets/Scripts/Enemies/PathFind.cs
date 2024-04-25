using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class PathFind : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject target;
    private LayerMask terrainLayer;
    private EnemyType ET;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ET = GetComponentInParent<EnemyType>();
        terrainLayer = LayerMask.GetMask("Terrain");
    }

    void Update()
    {
        target = ET.target;

        transform.localPosition = Vector3.zero;
        
        RaycastHit hit;
        Physics.Raycast(target.transform.position, Vector3.down, out hit, 100, terrainLayer);
        
        NavMeshPath path = new NavMeshPath();
        agent.enabled = true;
        agent.CalculatePath(hit.point, path);
        agent.enabled = false;
        ET.path = path;
    }
}
