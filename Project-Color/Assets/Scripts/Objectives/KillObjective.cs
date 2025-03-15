using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class KillObjective : Objective
{
    private EnemyController enemyController;
    
    public List<GameObject> spawnpoints = new List<GameObject>();
    private List<GameObject> enemiesInObjective = new List<GameObject>();

    private bool spawning;
    [HideInInspector] public int waves;

    [HideInInspector] public int group;
    
    private void Start()
    {
        enemyController = GameObject.Find("GameController").GetComponent<EnemyController>();
        // Add all spawnpoints to the list
        foreach (Transform child in transform)
            spawnpoints.Add(child.gameObject);
        
        foreach (GameObject sp in spawnpoints)
        {
            EnemySpawnPoint x = sp.GetComponent<EnemySpawnPoint>();
            if (x.wave > waves) waves = x.wave;
        }
    }

    protected override IEnumerator ObjectiveRequirement()
    {
        Debug.Log("Kill objective started");

        StartCoroutine(StartSpawnWaves());
        yield return new WaitUntil(() => !spawning);

        Debug.Log("Kill objective complete");
    }
    
    IEnumerator StartSpawnWaves()
    {
        spawning = true;

        int currentWave = 0;
        while (true)
        {
            bool spawned = false;
            foreach (GameObject sp in spawnpoints)
            {
                if (sp.GetComponent<EnemySpawnPoint>().wave == currentWave)
                {
                    StartCoroutine(Spawn(sp));
                    spawned = true;
                }
            }

            currentWave++;
            if (!spawned) break;
            
            // Condition to start the next wave 
            yield return new WaitUntil(delegate
            {
                // Update enemy list
                enemiesInObjective.Clear();
                foreach (GameObject enemy in EnemyController.all.activeList)
                    if (enemy.GetComponent<EnemyType>().killGroup == group)
                        enemiesInObjective.Add(enemy);
                
                return enemiesInObjective.Count == 0;
            });
        }

        spawning = false;
    }

    private IEnumerator Spawn(GameObject spawnpoint)
    {
        EnemySpawnPoint esp = spawnpoint.GetComponent<EnemySpawnPoint>();
        
        float spread = 2;
        float wait = 0.05f;
        
        for (int type = 0; type < esp.enemiesToSpawn.Count; type++)
            for (int j = 0; j < esp.enemiesToSpawn[type]; j++)
            {
                Vector3 random = new Vector3(Random.Range(-spread, spread), 0, Random.Range(-spread, spread));
                GameObject enemy = EnemyController.typeLists[type].inactiveList[0];
                    
                // Spawn
                enemy.GetComponent<EnemyType>().Activate(spawnpoint.transform.position + random, group);
  
                yield return new WaitForSeconds(wait);
            }
    }
}

