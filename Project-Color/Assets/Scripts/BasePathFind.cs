using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class BasePathFind : MonoBehaviour
{
    public NavMeshAgent vihu;
    public GameObject player;
    [FormerlySerializedAs("blockloslayer")] public LayerMask noloslayer;
    private float lostlostimer;
    
    
    private void Update()
    {
        lostlostimer -= Time.deltaTime;
        if (!Physics.Raycast(transform.position, (player.transform.position - transform.position).normalized, (player.transform.position - transform.position).magnitude, noloslayer))
            lostlostimer = 1;
        if (lostlostimer >= 0)
            vihu.SetDestination(player.transform.position);
        
    }
}
