using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BasePathFind : MonoBehaviour
{
    public NavMeshAgent vihu;
    public GameObject player;
    [FormerlySerializedAs("blockloslayer")] public LayerMask noloslayer;
    private float lostlostimer;
    private float wanderInterval;
    public int movementMode;
    public bool targetInSight;
    private Vector3 wanderOrigin;

    private void Start()
    {
        wanderOrigin = transform.position;
    }

    private void Update()
    {
        PlayerSeen();
        Move();
    }

    void Move()
    {
        if (movementMode == 0 && vihu.remainingDistance < 1)
        {
            wanderInterval -= Time.deltaTime;
            if (wanderInterval < 0)
            {
                if ((transform.position - wanderOrigin).magnitude < 2.5)
                    vihu.SetDestination(wanderOrigin + Quaternion.Euler(0, Random.Range(0, 360), 0) * new Vector3(5, 0, 0));
                else
                {
                    if (Random.Range(0, 10) <= 3)
                        vihu.SetDestination(wanderOrigin + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)));
                    else
                    {
                        Vector3 target = wanderOrigin + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
                        wanderOrigin = target;
                        vihu.SetDestination(target);
                    }
                }

                wanderInterval = Random.Range(2f, 5f);
            }

            if (targetInSight) movementMode = 1;
        }
        else if (movementMode == 1)
        {
            lostlostimer -= Time.deltaTime;
            if (targetInSight)
                lostlostimer = 1;
            if (lostlostimer >= 0)
                vihu.SetDestination(player.transform.position);

            if (vihu.remainingDistance < 1 && !targetInSight)
            {
                wanderOrigin = transform.position;
                movementMode = 0;
            }
        }
    }

    void PlayerSeen()
    {
        targetInSight = !Physics.Raycast(transform.position, (player.transform.position - transform.position).normalized, (player.transform.position - transform.position).magnitude, noloslayer);
    }

}
