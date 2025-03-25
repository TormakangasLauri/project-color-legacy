using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingMovement : BaseEnemyMovement
{
    private Hanging _hanging;
    private GameObject hangPoint;

    private enum states { inactive, idle, move };
    private states currentState;
    
    void Start()
    {
        _hanging = GetComponent<Hanging>();

        hangPoint = GetComponent<Hanging>().hangPoint;
    }
    
    void FixedUpdate()
    {
        if (!_enemyType.active) currentState = states.inactive;

        switch (currentState)
        {
            case states.inactive: Inactive(); break;
            case states.idle: Idle(); break;
            case states.move: Move(); break;
        }
    }

    void Inactive()
    {
        if (_enemyType.active) currentState = states.move;
    }

    void Idle()
    {
        if (rb.velocity.magnitude > 1) currentState = states.move;
    }

    void Move()
    {
        if (rb.velocity.magnitude <= 1 && Vector3.Distance(transform.position, _hanging.targetPoint) < 2) currentState = states.idle;
    }
}
