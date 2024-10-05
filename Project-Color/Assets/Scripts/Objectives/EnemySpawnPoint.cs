using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public int wave;
    [Space]
    public int basicCount;
    public int sniperCount;
    public int hulkCount;
    public int hangingCount;
    
    [HideInInspector] public List<int> enemiesToSpawn = new List<int>();

    private void Awake()
    {
        enemiesToSpawn.Add(basicCount);
        enemiesToSpawn.Add(sniperCount);
        enemiesToSpawn.Add(hulkCount);
        enemiesToSpawn.Add(hangingCount);
    }
}
