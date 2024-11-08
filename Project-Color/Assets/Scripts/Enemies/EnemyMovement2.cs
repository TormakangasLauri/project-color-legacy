using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement2 : BaseEnemyMovement
{
    private EnemyMovementState idle;
    EnemyMovementState x;
    
    private void Awake()
    {
        movementStates = new List<EnemyMovementState>()
        {
            idle,
            x
        };
    }
}
