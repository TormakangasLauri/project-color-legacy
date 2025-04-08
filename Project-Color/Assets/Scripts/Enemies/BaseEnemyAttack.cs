using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyAttack : MonoBehaviour
{
    EnemyType enemyType;

    public float damage = 1;
    public float attackCooldown = 1;

    public bool attacking = false;
    public bool onCooldown = false;

    private void Awake()
    {
        enemyType = GetComponent<EnemyType>();
    }

    public virtual void Attack()
    {

    }
}
