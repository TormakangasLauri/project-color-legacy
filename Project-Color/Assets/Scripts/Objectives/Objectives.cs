using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objectives : MonoBehaviour
{
    private List<GameObject> enemies = new List<GameObject>();
    
	public IEnumerator ObjectiveControl()
	{
		
		
		yield return null;
	}

    public IEnumerator Kill()
    {
        
        
        yield return null;
    }
    
    public IEnumerator Platform()
    {
        
        
        yield return null;
    }

	public IEnumerator Paint()
    {
        
        
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
