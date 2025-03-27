using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Controllers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    private Collider hitbox;
    private Rigidbody rb;
    public LayerMask enemyLayer;
    public List<GameObject> enemies;

    private PlayerMovement2 PM;
    private EnemyController enemyController;
    private SlamAreaCheck SAC;
    public AttackIndicator AI;
    public static PlayerAttack inst;

    public CollisionPainter CP1;
    public CollisionPainter CP2;

    public int paintChannel;
    public ParticleSystem attackParticle1;
    public ParticleSystem attackParticle2;
    public ParticleSystem attackParticle3;
    public ParticleSystem attackParticle4;
    public ParticleSystem attackParticle5;
    public AttackParticle AP;

    public ParticleSystem slamParticle1;
    public ParticleSystem slamParticle2;
    public ParticleSystem slamParticle3;
    public ParticleSystem slamParticle4;
    public ParticleSystem slamParticle5;
    public GameObject slamParticle0;

    public ParticleSystem bounceParticle;
    public ParticleSystem bounceTrail;

    public bool pushIsActive;

    bool lmbHeld;
    bool rmbHeld;
    float holdTimer;

    public bool canAttack = true;
    bool attackFromRight = true;
    float sideSwitchTimer = 0;

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
    public float bounceRange = 2;
    public float bounceForceOnPlayer = 30;
    public float enemyBounceMult = 1;
    public float bounceForceOnEnemy = 20;

    public Brush rayPaint;
    
    private void Awake()
    {
        inst = this;
    }

    private void Start()
    {
        hitbox = GetComponent<Collider>();
        rb = gameObject.transform.parent.GetComponentInParent<Rigidbody>();
        PM = gameObject.transform.parent.GetComponentInParent<PlayerMovement2>();
        enemyController = GameObject.Find("GameController").GetComponent<EnemyController>();
        SAC = gameObject.transform.parent.transform.parent.GetComponentInChildren<SlamAreaCheck>();
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

        // Paint color picker
        if (Input.GetKeyDown(KeyCode.Alpha1)) paintChannel = 0; ChangeBrushColor();
        if (Input.GetKeyDown(KeyCode.Alpha2)) paintChannel = 1; ChangeBrushColor();
        if (Input.GetKeyDown(KeyCode.Alpha3)) paintChannel = 2; ChangeBrushColor();
        if (Input.GetKeyDown(KeyCode.Alpha4)) paintChannel = 3; ChangeBrushColor();
        if (Input.GetKeyDown(KeyCode.Alpha5)) paintChannel = 4; ChangeBrushColor();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            rayPaint.splatChannel = paintChannel;
            Ray ray = new Ray(transform.parent.parent.position, transform.forward);
            PaintTarget.PaintRay(ray, rayPaint);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            EnemyController.Activate("hanging", Vector3.zero);
        }

        // Attack side reset
        if (sideSwitchTimer < 0) attackFromRight = true;
        sideSwitchTimer -= Time.deltaTime;
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
            if (holdTimer < 0.3 && !PM.slamming)
            {
                foreach (GameObject enemy in enemies)
                {
                    Attack(enemy, false);
                }
                
                PLayAttackParticle(false);
            }
            else if (holdTimer >= 1)
            {
                List<GameObject> enemies1 = new List<GameObject>();
                enemies1.AddRange(enemies);
                foreach (GameObject enemy in enemies1)
                {
                    Attack(enemy, true);
                }

                PLayAttackParticle(true);
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
            if (holdTimer < 0.3 && !PM.grounded && !PM.slamming)
            {
                StartCoroutine(Slam());
            }
            else if (holdTimer >= 1)
            {
                Bounce();
            }
        }
    }

    private void Attack(GameObject enemy, bool charged)
    {
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        enemyHealth.TakeDamage(!charged ? attackDamage: cAttackDamage);

        Vector2 KB = !charged ? attackKB : cAttackKB;
        Vector3 KBdir = GetComponentInParent<Transform>().rotation * Vector3.forward * KB.x + Vector3.up * KB.y;
        if (pushIsActive) // goofy lookin' ass knockback
            enemyHealth.Knockback(KBdir, ForceMode.Force);
    }

    private IEnumerator Slam()
    {
        PM.slamming = true;
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

        PlaySLamParticle();

        PM.slamming = false;
    }

    void Bounce()
    {
        PM.wallRunning = false;

        RaycastHit hit;
        Transform cam = transform.parent;
        Physics.Raycast(cam.position, cam.forward, out hit, bounceRange, LayerMask.GetMask("Terrain"));

        RaycastHit enemyHit;
        Physics.Raycast(cam.position, cam.forward, out enemyHit, bounceRange, LayerMask.GetMask("Enemy"));

        // If bounce hits an enemy, add force to enemy and the player away from each other
        // Hitting terrain only adds force to the player but more than when hitting enemies
        if (enemyHit.collider != null)
        {
            StartCoroutine(PlayBounceTrail());
            rb.velocity = new Vector3(rb.velocity.x / 2, 0, rb.velocity.z / 2);

            // Add force to player when hitting an enemy
            Vector3 dir = -cam.forward;
            if (dir.y < 0)
            {
                dir.y = 0;
                dir.Normalize();
            }
            rb.AddForce(dir * bounceForceOnPlayer * enemyBounceMult, ForceMode.Impulse);
            
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
                enemy.GetComponent<EnemyHealth>().Knockback(dir2 * bounceForceOnEnemy/2, ForceMode.Impulse);
            }
        }
        else if (hit.collider != null)
        {
            PlayBounceParticle();
            StartCoroutine(PlayBounceTrail());
            rb.velocity = new Vector3(rb.velocity.x / 2, 0, rb.velocity.z / 2);
            
            // Add force to player when hitting terrain
            Vector3 dir = -cam.forward;
            if (dir.y < 0)
            {
                dir.y = 0;
                dir.Normalize();
            }
            rb.AddForce(dir * bounceForceOnPlayer, ForceMode.Impulse);
        }
    }

    void PLayAttackParticle(bool charged)
    {
        StartCoroutine(AP.Rotate(attackFromRight, charged));
        attackFromRight = !attackFromRight;

        // Time before automatically switching attack side
        sideSwitchTimer = 1f;

        switch (paintChannel)
        {
            case 0: attackParticle1.Play(); break;
            case 1: attackParticle2.Play(); break;
            case 2: attackParticle3.Play(); break;
            case 3: attackParticle4.Play(); break;
            case 4: attackParticle5.Play(); break;
        }
    }

    void PlaySLamParticle()
    {
        slamParticle0.GetComponent<ParticlePainter>().brush.splatChannel = paintChannel;
        slamParticle0.GetComponent<ParticleSystem>().Play();

        switch (paintChannel)
        {
            case 0: slamParticle1.Play(); break;
            case 1: slamParticle2.Play(); break;
            case 2: slamParticle3.Play(); break;
            case 3: slamParticle4.Play(); break;
            //case 4: slamParticle5.Play(); break;
        }
    }

    void PlayBounceParticle()
    {
        bounceParticle.GetComponent<ParticlePainter>().brush.splatChannel = paintChannel;
        bounceParticle.GetComponent<ParticleSystem>().Play();
    }
    
    IEnumerator PlayBounceTrail()
    {
        bounceTrail.GetComponent<ParticlePainter>().brush.splatChannel = paintChannel;
        
        bounceTrail.Play();
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => PM.grounded || PM.walled);
        bounceTrail.Stop();
    }

    void ChangeBrushColor()
    {
        CP1.brush.splatChannel = paintChannel;
        CP2.brush.splatChannel = paintChannel;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemyLayer == (enemyLayer | (1 << other.gameObject.layer))) enemies.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        enemies.Remove(other.gameObject);
    }
}
