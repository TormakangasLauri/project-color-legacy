using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlamAreaCheck : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();

    private void Update()
    {
        foreach (GameObject obj in enemies)
        {
            if (obj.IsDestroyed()) enemies.Remove(obj);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) enemies.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemies.Contains(other.gameObject)) enemies.Remove(other.gameObject);
    }
}
