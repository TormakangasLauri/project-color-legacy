using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnEnemies_test : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Single.PositiveInfinity, LayerMask.GetMask("Terrain"));
            StartCoroutine(EnemySpawner.inst.Spawn(0, 5, hit.point + Vector3.up, 2, 1));
        }
    }
}
