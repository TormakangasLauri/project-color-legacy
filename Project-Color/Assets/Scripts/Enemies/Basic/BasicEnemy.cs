using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : EnemyType
{
    void Awake()
    {
        target = GameObject.FindWithTag("PlayerRoot");
        type = Type.basic;
    }
}
