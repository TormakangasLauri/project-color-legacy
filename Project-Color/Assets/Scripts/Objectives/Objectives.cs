using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Objectives : MonoBehaviour
{
    enum objectives
    {
        kill,
        platform,
        paint
    };

    public List<GameObject> enemySpawnPointGroups = new List<GameObject>();

    private List<objectives> activeObjectives = new List<objectives>();

    EnemyController enemyController;

    public TextMeshProUGUI killObjectiveText;

    public bool platformActive = false;
    bool spawning = false;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
    }

    public IEnumerator Kill(int killGroup)
    {
        activeObjectives.Add(objectives.kill);

        List<GameObject> enemiesInObjective = new List<GameObject>();

        // Starts the spawning system
        StartCoroutine(StartWaves(killGroup));

        // Wait until all enemies in the objective are killed
        yield return new WaitUntil(delegate
        {
            // Update list
            enemiesInObjective.Clear();
            foreach (GameObject enemy in enemyController.allEnemies_active)
                if (enemy.GetComponent<EnemyType>().killGroup == killGroup)
                    enemiesInObjective.Add(enemy);

            killObjectiveText.text = enemiesInObjective.Count.ToString();
            return enemiesInObjective.Count == 0 && !spawning;
        });

        activeObjectives.Remove(objectives.kill);
        Debug.Log("Kill objective completed");
    }
    
    public IEnumerator Platform()
    {
        activeObjectives.Add(objectives.platform);

        platformActive = true;

        yield return new WaitWhile(delegate { return platformActive; });

        activeObjectives.Remove(objectives.platform);
        Debug.Log("Platform objective completed");
    }

	public IEnumerator Paint()
    {
        activeObjectives.Add(objectives.paint);

        // Paint objective requirements

        activeObjectives.Remove(objectives.paint);
        yield return null;
        Debug.Log("Paint objective completed");
    }

    IEnumerator StartWaves(int killGroup)
    {
        spawning = true;

        int wave = 0;
        while (true)
        {
            bool spawned = false;
            for (int i = 0; i < enemySpawnPointGroups[killGroup].transform.childCount; i++)
            {
                Transform spawnPoint = enemySpawnPointGroups[killGroup].transform.GetChild(i);
                if (spawnPoint.gameObject.GetComponent<EnemySpawnPoint>().wave == wave)
                {
                    StartCoroutine(Spawn(spawnPoint.gameObject, killGroup));
                    spawned = true;
                }
            }

            wave++;
            yield return new WaitForSeconds(5);
            if (!spawned) break;
        }

        spawning = false;
    }

    private IEnumerator Spawn(GameObject spawnPoint, int killGroup)
    {
        EnemySpawnPoint esp = spawnPoint.GetComponent<EnemySpawnPoint>();

        float spread = 2;
        float wait = 0.05f;
        // Basic enemy
        for (int i = 0; i < esp.basicCount; i++)
        {
            Vector3 random = new Vector3(Random.Range(-spread, spread), 0, Random.Range(-spread, spread));
            GameObject enemy = enemyController.basicEnemyList[0];
            
            // Spawn
            enemy.transform.position = spawnPoint.transform.position + random;
            enemy.GetComponent<EnemyType>().Activate(killGroup);
            enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;

            yield return new WaitForSeconds(wait);
        }

        // Sniper
        for (int i = 0; i < esp.sniperCount; i++)
        {
            Vector3 random = new Vector3(Random.Range(-spread, spread), 0, Random.Range(-spread, spread));
            GameObject enemy = enemyController.sniperList[0];

            // Spawn
            enemy.transform.position = spawnPoint.transform.position + random;
            enemy.GetComponent<EnemyType>().Activate(killGroup);
            enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;

            yield return new WaitForSeconds(wait);
        }

        // Milo
        for (int i = 0; i < esp.MILOCount; i++)
        {
            Vector3 random = new Vector3(Random.Range(-spread, spread), 0, Random.Range(-spread, spread));
            GameObject enemy = enemyController.MILOList[0];
            
            // Spawn
            enemy.transform.position = spawnPoint.transform.position + random;
            enemy.GetComponent<EnemyType>().Activate(killGroup);
            enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;

            yield return new WaitForSeconds(wait);
        }
    }
}
