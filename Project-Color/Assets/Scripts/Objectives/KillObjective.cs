using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using Random = UnityEngine.Random;

public class KillObjective : Objective
{
    private EnemyController enemyController;
    
    private List<GameObject> spawnpoints = new List<GameObject>();
    private List<GameObject> enemiesInObjective = new List<GameObject>();

    private bool spawning;
    private int waves;
    
    private void Start()
    {
        enemyController = GameObject.Find("GameController").GetComponent<EnemyController>();
        spawnpoints.AddRange(GetComponentsInChildren<GameObject>());
        foreach (GameObject sp in spawnpoints)
        {
            EnemySpawnPoint x = sp.GetComponent<EnemySpawnPoint>();
            if (x.wave > waves) waves = x.wave;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            StartCoroutine(Objective());
        GetComponent<Collider>().enabled = false;
    }

    // private void Update()
    // {
    //     if (active)
    //     {
    //         if (enemyController.all.activeList.Count == 0 && !spawning)
    //         {
    //             activeObjectives.Remove(objectives.kill);
    //             killObjectiveText.text = "OBJECTIVE COMPLETE";
    //             StartCoroutine(ClearKillText());
    //
    //             IEnumerator ClearKillText()
    //             {
    //                 yield return new WaitForSeconds(3);
    //                 killObjectiveText.text = "";
    //             }
    //         }
    //         killObjectiveText.text = enemyController.all.activeList.Count.ToString();
    //
    //         if (enemyController.all.activeList.Count == 0 && !spawning) activeObjectives.Remove(objectives.kill);
    //     }
    // }

    IEnumerator Objective()
    {
        active = true;

        StartCoroutine(StartSpawnWaves());
        yield return new WaitUntil(()=>!spawning);
        
        active = false;
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
            yield return new WaitForSeconds(5);
        }

        spawning = false;
    }

    // private IEnumerator Spawn(GameObject spawnPoint)
    // {
    //     EnemySpawnPoint esp = spawnPoint.GetComponent<EnemySpawnPoint>();
    //
    //     float spread = 2;
    //     float wait = 0.05f;
    //     
    //     for (int type = 0; type < enemyController.typeLists.Count; type++)
    //     for (int j = 0; j < esp.enemiesToSpawn[type]; j++)
    //     {
    //         Vector3 random = new Vector3(Random.Range(-spread, spread), 0, Random.Range(-spread, spread));
    //         GameObject enemy = enemyController.typeLists[type].inactiveList[0];
    //             
    //         // Spawn
    //         enemy.transform.position = spawnPoint.transform.position + random;
    //         enemy.GetComponent<EnemyType>().Activate(killGroup);
    //         enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //
    //         yield return new WaitForSeconds(wait);
    //     }
    // }

    private IEnumerator Spawn(GameObject spawnpoint)
    {
        EnemySpawnPoint esp = spawnpoint.GetComponent<EnemySpawnPoint>();
        
        float spread = 2;
        float wait = 0.05f;
        
        for (int type = 0; type < enemyController.typeLists.Count; type++)
            for (int j = 0; j < esp.enemiesToSpawn[type]; j++)
            {
                Vector3 random = new Vector3(Random.Range(-spread, spread), 0, Random.Range(-spread, spread));
                GameObject enemy = enemyController.typeLists[type].inactiveList[0];
                    
                // Spawn
                enemy.transform.position = spawnpoint.transform.position + random;
                enemy.GetComponent<EnemyType>().Activate();
                enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;

                yield return new WaitForSeconds(wait);
            }
    }
}

