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
        for (int i = 0; i < amount; i++)
            Instantiate(enemy, transform.position, new Quaternion());
    }
}
