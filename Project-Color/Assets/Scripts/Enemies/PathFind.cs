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
    private LayerMask terrainLayer;
    private EnemyType enemyType;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyType = GetComponentInParent<EnemyType>();
        terrainLayer = LayerMask.GetMask("Terrain");
    }

    void Update()
    {
        transform.localPosition = Vector3.zero;
        
        RaycastHit hit;
        Physics.Raycast(enemyType.target.transform.position + Vector3.up, Vector3.down, out hit, 100, terrainLayer);
        
        NavMeshPath path = new NavMeshPath();
        agent.enabled = true;
        try { agent.CalculatePath(hit.point, path); }
        catch {  }
        agent.enabled = false;
        enemyType.path = path;

        // Path visualization in scene view
        Vector3[] cl = path.corners;
        if (cl.Length >= 2)
        {
            float y = GetComponentInParent<Transform>().position.y;
            for (int i = 0; i < cl.Length - 1; i++)
            {
                Debug.DrawLine(new Vector3(cl[i].x, y, cl[i].z), new Vector3(cl[i+1].x, y, cl[i+1].z), Color.magenta);
            }
        }
    }
}
