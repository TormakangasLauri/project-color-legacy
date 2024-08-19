using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassMurder : MonoBehaviour
{
    private List<GameObject> enemies = new List<GameObject>();
    
    public IEnumerator massMurder()
    {
        StartCoroutine(EnemySpawner.inst.Spawn(0, 5, Vector3.zero, 1, 1));
        
        yield return null;
    }
}
