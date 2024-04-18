using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public int amount;
    
    void Start()
    {
        // for (int i = 0; i < amount; i++)
        //     Instantiate(enemy, transform.position, new Quaternion());

        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            Instantiate(enemy, transform.position, new Quaternion());
            yield return new WaitForSeconds(0.05f);
        }
    }
}
