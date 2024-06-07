using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    public ParticleSystem inkParticle;
    public ParticleSystem inkRemove;

    public bool pushIsActive;

    bool lmbHeld;
    bool rmbHeld;
    float holdTimer;

    public bool canAttack = true;

    Transform normalTransform;
    
    [Header("Attack")]
    public float attackDamage = 20;
    public Vector2 attackKB = new Vector2(300, 160);
    [Header("Charged Attack")]
    public float cAttackDamage = 50;
    public Vector2 cAttackKB = new Vector2(700, 200);
    [Header("Slam")]
    public float slamDamage = 20;
    public Vector2 slamKB = new Vector2(100, 600);
    [Header("Bounce")]
    public float bounceForceOnPlayer = 30;
    public float enemyBounceMult = 1;
    public float bounceForceOnEnemy = 20;
    
    private void Awake()
    {
        inst = this;
    }

    private void Start()
    {
        hitbox = GetComponent<Collider>();
        PM = transform.parent.GetComponentInParent<playermovement>();
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
        if (action.performed && canAttack)
        {
            rmbHeld = false;
            holdTimer = 0;
            lmbHeld = true;
        }
        else if (action.canceled && canAttack)
        {
            lmbHeld = false;
            if (holdTimer < 0.3 && !PM.attacking)
            {
                inkParticle.Play();
                foreach (GameObject enemy in enemies)
                {
                    Attack(enemy);
                }
            }
            else if (holdTimer >= 1)
            {
                foreach (GameObject enemy in enemies)
                {
                    ChargedAttack(enemy);
                }
            }
        }
    }

    public void SlamInput(InputAction.CallbackContext action)
    {
        if (action.performed && canAttack)
        {
            lmbHeld = false;
            holdTimer = 0;
            rmbHeld = true;
        }
        else if (action.canceled && canAttack)
        {
            rmbHeld = false;
            if (holdTimer < 0.3 && !PM.grounded && !PM.attacking)
            {
                inkRemove.Play();
                StartCoroutine(Slam());
            }
            else if (holdTimer >= 1)
            {
                Bounce();
            }
        }
    }

    private void Attack(GameObject enemy)
    {
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        enemyHealth.TakeDamage(attackDamage);

        Vector3 KBdir = GetComponentInParent<Transform>().rotation * Vector3.forward * attackKB.x + Vector3.up * attackKB.y;
        if (pushIsActive) // goofy lookin' ass knockback
            enemyHealth.Knockback(KBdir, ForceMode.Force);
    }

    void ChargedAttack(GameObject enemy)
    {
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        enemyHealth.TakeDamage(cAttackDamage);

        Vector3 KBdir = GetComponentInParent<Transform>().rotation * Vector3.forward * cAttackKB.x + Vector3.up * cAttackKB.y;
        if (pushIsActive)
            enemyHealth.Knockback(KBdir, ForceMode.Force);
    }

    private IEnumerator Slam()
    {
        PM.attacking = true;
        yield return new WaitUntil(delegate { return PM.grounded; });

        foreach (GameObject enemy in SAC.enemies)
        {
            // Damage
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(slamDamage);

            // Knockback
            Vector3 dir = (enemy.transform.position - transform.parent.position).normalized;
            if (pushIsActive) enemyHealth.Knockback(dir * slamKB.x + Vector3.up * slamKB.y, ForceMode.Force);
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
            // Add force to player when hitting an enemy
            Vector3 dir = -cam.forward;
            if (dir.y < 0)
            {
                dir.y = 0;
                dir.Normalize();
            }
            transform.parent.parent.GetComponent<Rigidbody>().AddForce(dir * bounceForceOnPlayer * enemyBounceMult, ForceMode.Impulse);
            
            // Add force to targeted enemy
            dir = cam.forward;
            if (dir.y < 0)
            {
                dir.y = 0;
                dir.Normalize();
            }
            enemyHit.collider.GetComponent<EnemyHealth>().Knockback(dir * bounceForceOnEnemy, ForceMode.Impulse);

            // Add force to all other enemies in attack range
            foreach (GameObject enemy in enemies)
            {
                Vector3 dir2 = (enemy.transform.position - transform.parent.parent.position).normalized;
                enemy.GetComponent<EnemyHealth>().Knockback(dir2 * bounceForceOnEnemy/4, ForceMode.Impulse);
            }
        }
        else if (hit.collider != null)
        {
            // Add force to player when hitting terrain
            Vector3 dir = -cam.forward;
            if (dir.y < 0)
            {
                dir.y = 0;
                dir.Normalize();
            }
            transform.parent.parent.GetComponent<Rigidbody>().AddForce(dir * bounceForceOnPlayer, ForceMode.Impulse);
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
