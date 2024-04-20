using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{
    private Collider hitbox;
    public LayerMask enemyLayer;
    public List<GameObject> enemies;

    public static PlayerAttack inst;

    public bool pushIsActive;

    private void Awake()
    {
        inst = this;
    }

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
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            enemyHealth.TakeDamage(20);

            if (enemy != null)
            {
                if (enemyHealth.healthAmount <= 0)
                {
                    Destroy(enemy);
                }
            }

            if (pushIsActive) // goofy lookin' ass knockback
                enemy.GetComponent<Rigidbody>().AddForce(GetComponentInParent<Transform>().rotation * Vector3.forward * 1000 + Vector3.up * 200);
        }
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
