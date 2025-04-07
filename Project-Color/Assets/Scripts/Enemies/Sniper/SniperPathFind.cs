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

        // Path visualization in scene view
        Vector3[] cl = path.corners;
        if (cl.Length >= 2)
        {
            float y = GetComponentInParent<Transform>().position.y;
            for (int i = 0; i < cl.Length - 1; i++)
            {
                Debug.DrawLine(new Vector3(cl[i].x, y, cl[i].z), new Vector3(cl[i + 1].x, y, cl[i + 1].z), Color.magenta);
            }
        }
    }
}
