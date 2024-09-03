using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyController : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();

    public List<GameObject> allEnemies = new List<GameObject>();
    public List<GameObject> basicEnemyList = new List<GameObject>();
    public List<GameObject> sniperList = new List<GameObject>();
    public List<GameObject> MILOList = new List<GameObject>();

    public List<GameObject> allEnemies_active = new List<GameObject>();
    public List<GameObject> basicEnemyList_active = new List<GameObject>();
    public List<GameObject> sniperList_active = new List<GameObject>();
    public List<GameObject> MILOList_active = new List<GameObject>();

    public static EnemyController inst;
    private GameObject player;
    private float t;

    private void Awake() { inst = this; }

    private void Start()
    {
        player = GameObject.FindWithTag("Player");

		for (int i = 0; i < 100; i++)
            Instantiate(enemies[0], Vector3.down * 500, Quaternion.identity);
        for (int i = 0; i < 30; i++)
            Instantiate(enemies[1], Vector3.down * 500, Quaternion.identity);
        for (int i = 0; i < 10; i++)
            Instantiate(enemies[2], Vector3.down * 500, Quaternion.identity);
    }

    void Update()
    {
        BasicEnemy();
        Milo();

        allEnemies.Clear();
        allEnemies.AddRange(basicEnemyList);
        allEnemies.AddRange(sniperList);
        allEnemies.AddRange(MILOList);
        allEnemies.AddRange(basicEnemyList_active);
        allEnemies.AddRange(sniperList_active);
        allEnemies.AddRange(MILOList_active);
    }

    void BasicEnemy()
    {
        t -= Time.deltaTime;
        if (t < 0)
        { t = 0.5f;
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

                basicEnemyList[i].GetComponent<EnemyMovement>().stopDistance = stopDist;
            }
        }
    }
    
    void Milo()
    {
        foreach (GameObject milo in MILOList)
        {
            milo.GetComponent<EnemyMovement>().stopDistance = 3;
        }
    }
}
