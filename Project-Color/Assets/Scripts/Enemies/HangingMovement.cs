using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingMovement : BaseEnemyMovement
{
    private Hanging _hanging;
    private HangingCleaning _cleaning;
    private GameObject hangPoint;

    public float hEscapeSpeed = 0;
    public float vEscapeSpeed = 10;

    private enum states { inactive, idle, move, escape };
    private states currentState;
    
    void Start()
    {
        _hanging = GetComponent<Hanging>();
        _cleaning = GetComponent<HangingCleaning>();
        hangPoint = _hanging.hangPoint;

        currentState = states.move;
    }
    
    void FixedUpdate()
    {
        if (!_enemyType.active) currentState = states.inactive;
        if (_cleaning.cleaningComplete) currentState = states.escape;

        switch (currentState)
        {
            case states.inactive: Inactive(); break;
            case states.idle: Idle(); break;
            case states.move: Move(); break;
            case states.escape: Escape(); break;
        }
    }

    void Inactive()
    {
        if (_enemyType.active) currentState = states.move;
    }

    void Idle()
    {
        _cleaning.Clean(_hanging.targetPaintGroup); // Clean paint when not moving

        if (rb.velocity.magnitude > 1) currentState = states.move;
    }

    void Move()
    {
        _cleaning.StopCleaning(); // Stop cleaning when moving

        if (rb.velocity.magnitude <= 1 && Vector3.Distance(transform.position, _hanging.targetPoint) < 2) currentState = states.idle;
    }

    void Escape()
    {
        hangPoint = _hanging.hangPoint;
        hangPoint.transform.position += Vector3.up * vEscapeSpeed/1000 - _hanging.targetDirection * hEscapeSpeed/1000;

        if (transform.position.y > _hanging.spawnHeight) _enemyType.Deactivate();
    }
}
