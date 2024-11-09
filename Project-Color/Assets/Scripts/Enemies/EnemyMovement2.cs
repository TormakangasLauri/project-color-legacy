using System;
using System.Collections;
using System.Collections.Generic;
using OpenCover.Framework.Model;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class EnemyMovement2 : BaseEnemyMovement
{
    private EnemyMovementState inactive = new EnemyMovementState()
    {
        StateAction = () =>
        {
            Debug.Log("");
        },
        EnterState = () =>
        {
            
        },
        ExitState = () =>
        {
            
        }
    };
    private EnemyMovementState idle;
    private EnemyMovementState navmesh;
    private EnemyMovementState los;
    
    private void Awake()
    {
        movementStates = new List<EnemyMovementState>() { inactive, idle, navmesh, los };
    }
}
