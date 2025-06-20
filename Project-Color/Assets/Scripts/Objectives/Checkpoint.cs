using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int index;
    public Vector3 spawnPosition;

    public void SetSpawnPosition()
    {
        Physics.Raycast(transform.position, Vector3.down, out var hit, 10, LayerMask.GetMask("Terrain"));
        spawnPosition = hit.point + Vector3.up * 0.1f;
    }

    private void Awake()
    {
        Physics.Raycast(transform.position, Vector3.down, out var hit, 10, LayerMask.GetMask("Terrain"));
        spawnPosition = hit.point /*+ Vector3.up * 0.1f*/;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponentInParent<GameController>().currentCheckpoint = index;
            GameData.levelData[GameData.currentLevel].checkpoint = index;
            GameData.SaveAllData();
        }
    }
}
