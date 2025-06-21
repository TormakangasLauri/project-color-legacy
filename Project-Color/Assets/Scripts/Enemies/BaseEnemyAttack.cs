using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    protected EnemyType enemyType;
    protected Rigidbody rb;

    protected GameObject target;

    public float damage = 1;
    public float attackCooldown = 1;
    public float cooldownTimer = 0;

    public bool attacking = false;
    public bool onCooldown = false;

    private void Awake()
    {
        enemyType = GetComponent<EnemyType>();
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        if (target == null) target = enemyType.target;
        cooldownTimer -= Time.deltaTime * enemyType.timeScale;
        if (cooldownTimer <= 0) onCooldown = false;
        if (enemyType.timeScale == 0) onCooldown = true; // Prevent attacks when paused
    }

    protected virtual void StartCooldown(float cooldown = -1)
    {
        cooldownTimer = cooldown == -1 ? attackCooldown : cooldown; // Use parameter value if it is set
        onCooldown = true;
    }

    public virtual void Attack() { }
}
