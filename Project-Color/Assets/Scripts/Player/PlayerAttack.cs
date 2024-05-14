using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public AttackIndicator AI;
    public static PlayerAttack inst;

    public bool pushIsActive;

    public bool lmbHeld;
    public bool rmbHeld;
    public float holdTimer;

    Transform normalTransform;

    private void Awake()
    {
        inst = this;
    }

    private void Start()
    {
        hitbox = GetComponent<Collider>();
        PM = GetComponentInParent<playermovement>();
        SAC = gameObject.transform.parent.transform.parent.GetComponentInChildren<SlamAreaCheck>();

        normalTransform = transform;
    }

    private void Update()
    {
        if (lmbHeld || rmbHeld) holdTimer += Time.deltaTime;
        else holdTimer = 0;
        AI.SetValue(holdTimer);

        if (holdTimer >= 1 && lmbHeld)
        {
            transform.localPosition = new Vector3(0, 0, 2.5f);
            transform.localScale = new Vector3(2, 1.5f, 2);
        }
        else
        {
            transform.localPosition = new Vector3(0, 0, 1.5f);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void AttackInput(InputAction.CallbackContext action)
    {
        if (action.performed)
        {
            rmbHeld = false;
            holdTimer = 0;
            lmbHeld = true;
        }
        else if (action.canceled)
        {
            lmbHeld = false;
            if (holdTimer < 0.3 && !PM.attacking)
            {
                Debug.Log("Attack");
                foreach (GameObject enemy in enemies)
                {
                    Attack(enemy);
                }
            }
            else if (holdTimer >= 1)
            {
                Debug.Log("Charged Attack");
                foreach (GameObject enemy in enemies)
                {
                    ChargedAttack(enemy);
                }
            }
        }
    }

    public void SlamInput(InputAction.CallbackContext action)
    {
        if (action.performed)
        {
            lmbHeld = false;
            holdTimer = 0;
            rmbHeld = true;
        }
        else if (action.canceled)
        {
            rmbHeld = false;
            if (holdTimer < 0.3 && !PM.grounded && !PM.attacking)
            {
                Debug.Log("Slam");
                StartCoroutine(Slam());
            }
            else if (holdTimer >= 1)
            {
                Debug.Log("Bounce");
                Bounce();
            }
        }
    }

    private void Attack(GameObject enemy)
    {
        Health enemyHealth = enemy.GetComponent<Health>();
        enemyHealth.TakeDamage(20);

        if (pushIsActive) // goofy lookin' ass knockback
            enemy.GetComponent<Rigidbody>().AddForce(GetComponentInParent<Transform>().rotation * Vector3.forward * 300 + Vector3.up * 160);
    }

    void ChargedAttack(GameObject enemy)
    {
        Health enemyHealth = enemy.GetComponent<Health>();
        enemyHealth.TakeDamage(20);

        if (pushIsActive)
            enemy.GetComponent<Rigidbody>().AddForce(GetComponentInParent<Transform>().rotation * Vector3.forward * 700 + Vector3.up * 200);
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

    void Bounce()
    {
        RaycastHit hit;
        Transform cam = transform.parent;
        Physics.Raycast(cam.position, cam.forward, out hit, 2, LayerMask.GetMask("Terrain"));

        RaycastHit enemyHit;
        Physics.Raycast(cam.position, cam.forward, out enemyHit, 2, LayerMask.GetMask("Enemy"));

        // If bounce hits an enemy, add force to enemy and the player away from each other
        // Hitting terrain only adds force to the player but more than when hitting enemies
        if (enemyHit.collider != null)
        {
            Debug.Log("enemy hit");
            Vector3 dir = -cam.forward;
            if (dir.y < 0)
            {
                dir.y = 0;
                dir.Normalize();
            }
            transform.parent.parent.GetComponent<Rigidbody>().AddForce(dir * 15, ForceMode.Impulse);

            dir = cam.forward;
            if (dir.y < 0)
            {
                dir.y = 0;
                dir.Normalize();
            }
            enemyHit.collider.GetComponent<Rigidbody>().AddForce(dir * 20, ForceMode.Impulse);

            foreach (GameObject enemy in enemies)
            {
                Vector3 dir2 = (enemy.transform.position - transform.parent.parent.position).normalized;
                enemy.GetComponent<Rigidbody>().AddForce(dir2 * 5, ForceMode.Impulse);
            }
        }
        else if (hit.collider != null)
        {
            Debug.Log("terrain hit");
            Vector3 dir = -cam.forward;
            if (dir.y < 0)
            {
                dir.y = 0;
                dir.Normalize();
            }
            transform.parent.parent.GetComponent<Rigidbody>().AddForce(dir * 30, ForceMode.Impulse);
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
