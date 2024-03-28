using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasePathFind : MonoBehaviour
{
    public NavMeshAgent vihu;
    public GameObject player;

    private void Update()
    {
        vihu.SetDestination(player.transform.position);
    }
}
