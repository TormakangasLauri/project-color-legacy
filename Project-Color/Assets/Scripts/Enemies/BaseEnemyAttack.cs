using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyAttack : MonoBehaviour
{
    EnemyType enemyType;

    public float damage = 1;

    private void Awake()
    {
        enemyType = GetComponent<EnemyType>();
    }
}
