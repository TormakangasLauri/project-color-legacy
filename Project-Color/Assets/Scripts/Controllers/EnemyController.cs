using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyController : MonoBehaviour
{
    public List<GameObject> AllEnemies = new List<GameObject>();
    public List<GameObject> basicEnemyList = new List<GameObject>();
    public List<GameObject> SniperList = new List<GameObject>();
    public List<GameObject> MILOList = new List<GameObject>();
    public static EnemyController inst;
    private GameObject player;
    private float t;

    private void Awake() { inst = this; }

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        t -= Time.deltaTime;
        if (t < 0)
        { t = 0.5f;
            
            // BASIC ENEMY

            // Sort enemies based on the distance to player
            basicEnemyList.Sort((obj1, obj2) =>
            {
                return Vector3.Distance(obj1.transform.position, player.transform.position).CompareTo(Vector3.Distance(obj2.transform.position, player.transform.position));
            });

            // Assign stopping distances for enemies making some enemies get close and other stay furter away
            float stopDist = 2;
            float enemiesOnLayer = 5;
            for (int i = 0; i < basicEnemyList.Count; i++)
            {
                if (i % enemiesOnLayer == 0)
                {
                    stopDist += 1;
                    enemiesOnLayer += stopDist;
                }

                basicEnemyList[i].GetComponent<NavMeshAgent>().stoppingDistance = stopDist;
                basicEnemyList[i].GetComponent<EnemyMovement>().stopDistance = stopDist;
            }
        }

        // M.I.L.O

        foreach (GameObject milo in MILOList)
        {
            milo.GetComponent<EnemyMovement>().stopDistance = 3;
        }
        
        AllEnemies.Clear();
        AllEnemies.AddRange(basicEnemyList);
        AllEnemies.AddRange(SniperList);
        AllEnemies.AddRange(MILOList);
    }
}
