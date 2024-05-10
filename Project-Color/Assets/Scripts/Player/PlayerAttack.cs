using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{
    private Collider hitbox;
    public LayerMask enemyLayer;
    public List<GameObject> enemies;

    private playermovement PM;
    private SlamAreaCheck SAC;
    public static PlayerAttack inst;

    public bool pushIsActive;

    private void Awake()
    {
        inst = this;
    }

    private void Start()
    {
        hitbox = GetComponent<Collider>();
        PM = GetComponentInParent<playermovement>();
        SAC = gameObject.transform.parent.transform.parent.GetComponentInChildren<SlamAreaCheck>();
    }

    public void AttackInput(InputAction.CallbackContext action)
    {
        if (action.performed && !PM.attacking)
        {
            foreach (GameObject enemy in enemies)
            {
                Attack(enemy);
            }
        }
    }

    public void SlamInput(InputAction.CallbackContext action)
    {
        if (action.performed && !PM.grounded && !PM.attacking)
        {
            StartCoroutine(Slam());
        }
    }

    private void Attack(GameObject enemy)
    {
        Health enemyHealth = enemy.GetComponent<Health>();
        enemyHealth.TakeDamage(20);

        if (pushIsActive) // goofy lookin' ass knockback
            enemy.GetComponent<Rigidbody>().AddForce(GetComponentInParent<Transform>().rotation * Vector3.forward * 400 + Vector3.up * 200);
    }

    private IEnumerator Slam()
    {
        PM.attacking = true;
        yield return new WaitUntil(delegate { return PM.grounded; });

        foreach (GameObject enemy in SAC.enemies)
        {
            // Damage
            enemy.GetComponent<Health>().TakeDamage(20);

            // Knockback
            Vector3 dir = (enemy.transform.position - transform.parent.position).normalized;
            if (pushIsActive) enemy.GetComponent<Rigidbody>().AddForce(dir * 100 + Vector3.up * 600);
        }

        PM.attacking = false;
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
