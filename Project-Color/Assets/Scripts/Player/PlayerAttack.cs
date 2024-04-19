using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private Collider hitbox;
    public LayerMask enemyLayer;
    public List<GameObject> enemies;

    private void Start()
    {
        hitbox = GetComponent<Collider>();
    }

    public void AttackInput(InputAction.CallbackContext action)
    {
        if (action.performed)
        {
            foreach (GameObject enemy in enemies)
            {
                Attack(enemy);
            }
        }
    }

    private void Attack(GameObject enemy)
    {
        enemy.GetComponent<NavMeshAgent>().enabled = false;
        enemy.GetComponent<Rigidbody>().AddForce(GetComponentInParent<Transform>().rotation * Vector3.forward * 2000 + Vector3.up * 500);
        //Destroy(enemy);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemyLayer == (enemyLayer | (1 << other.gameObject.layer))) enemies.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemies.Contains(other.gameObject)) enemies.Remove(other.gameObject);
    }
}
