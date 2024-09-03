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

    private List<GameObject> enemies = new List<GameObject>();
    private List<objectives> activeObjectives = new List<objectives>();

    EnemyController enemyController;

    public TextMeshProUGUI killObjectiveText;

    public bool platformActive = false;

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
            return enemiesInObjective.Count == 0;
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
        while (true)
        {
            bool spawned = false;
            foreach (GameObject spawnPoint in enemySpawnPointGroups[killGroup].GetComponentsInChildren<GameObject>())
            {
                Spawn(spawnPoint, killGroup);
                spawned = true;
            }

            yield return new WaitForSeconds(10);
            if (!spawned) break;
        }
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

            // Change lists
            enemyController.allEnemies_active.Add(enemy);
            enemyController.basicEnemyList_active.Add(enemy);
            enemyController.basicEnemyList.RemoveAt(0);

            // Spawn
            enemy.transform.position = spawnPoint.transform.position + random;
            enemy.GetComponent<EnemyType>().Activate();
            enemy.GetComponent<EnemyType>().killGroup = killGroup;

            yield return new WaitForSeconds(wait);
        }

        // Sniper
        for (int i = 0; i < esp.sniperCount; i++)
        {
            Vector3 random = new Vector3(Random.Range(-spread, spread), 0, Random.Range(-spread, spread));
            GameObject enemy = enemyController.sniperList[0];

            // Change lists
            enemyController.allEnemies_active.Add(enemy);
            enemyController.sniperList_active.Add(enemy);
            enemyController.sniperList.RemoveAt(0);

            // Spawn
            enemy.transform.position = spawnPoint.transform.position + random;
            enemy.GetComponent<EnemyType>().Activate();
            enemy.GetComponent<EnemyType>().killGroup = killGroup;

            yield return new WaitForSeconds(wait);
        }

        // Milo
        for (int i = 0; i < esp.MILOCount; i++)
        {
            Vector3 random = new Vector3(Random.Range(-spread, spread), 0, Random.Range(-spread, spread));
            GameObject enemy = enemyController.MILOList[0];

            // Change lists
            enemyController.allEnemies_active.Add(enemy);
            enemyController.MILOList_active.Add(enemy);
            enemyController.MILOList.RemoveAt(0);

            // Spawn
            enemy.transform.position = spawnPoint.transform.position + random;
            enemy.GetComponent<EnemyType>().Activate();
            enemy.GetComponent<EnemyType>().killGroup = killGroup;

            yield return new WaitForSeconds(wait);
        }
    }
}
