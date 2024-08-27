using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Objectives : MonoBehaviour
{
    enum objectives
    {
        kill,
        platform,
        paint
    };

    List<GameObject> enemySpawnPoints = new List<GameObject>();

    private List<GameObject> enemies = new List<GameObject>();
    private List<objectives> activeObjectives = new List<objectives>();

    EnemyController enemyController;

    public TextMeshProUGUI killObjectiveText;

    public bool platformActive = false;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        enemySpawnPoints.AddRange(GameObject.FindGameObjectsWithTag("SpawnPoint"));
    }

    public IEnumerator Kill()
    {
        activeObjectives.Add(objectives.kill);

        List<GameObject> enemiesInObjective = new List<GameObject>();
        enemiesInObjective.AddRange(enemyController.AllEnemies.GetRange(0, 10));

        yield return new WaitUntil(delegate
        {
            killObjectiveText.text = enemiesInObjective.Count.ToString();
            return enemiesInObjective.Count == 0;
        });

        activeObjectives.Remove(objectives.kill);
    }
    
    public IEnumerator Platform()
    {
        activeObjectives.Add(objectives.platform);

        platformActive = true;

        yield return new WaitWhile(delegate { return platformActive; });

        activeObjectives.Remove(objectives.platform);
    }

	public IEnumerator Paint()
    {
        activeObjectives.Add(objectives.paint);



        activeObjectives.Remove(objectives.paint);
        yield return null;
    }

	/// <summary>
    /// Spawn enemies
    /// </summary>
    /// <param name="enemy">Number code of the enemy, see the list from the script in GameController</param>
    /// <param name="amount">Amount of enemies to spawn</param>
    /// <param name="location">Spawn location</param>
    /// <param name="spread">Max distance that the enemies can spawn away from the spawn location</param>
    /// <param name="time">Total time for all enemies to be spawned in seconds</param>
    private IEnumerator Spawn(int enemyIndex, int amount, Vector3 location, float spread, float time)
    {
        GameObject e = enemies[enemyIndex];
        List<GameObject> spawnedEnemies = new List<GameObject>();

        for (int i = 0; i < amount; i++)
        {
            Vector3 random = new Vector3(Random.Range(-spread, spread), 0, Random.Range(-spread, spread));
            GameObject enemy = Instantiate(e, location + random, new Quaternion());
            spawnedEnemies.Add(enemy);
            yield return new WaitForSeconds(time / amount);
        }
    }
}
