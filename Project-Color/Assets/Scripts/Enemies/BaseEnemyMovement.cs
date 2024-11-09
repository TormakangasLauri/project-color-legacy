using System;
using System.Collections;
using System.Collections.Generic;
using OpenCover.Framework.Model;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementState : Method
{
    public Action StateAction;
    public Action EnterState;
    public Action ExitState;
    public void Update(){StateAction?.Invoke();}
    public void Enter(){EnterState?.Invoke();}
    public void Exit(){ExitState?.Invoke();}
}

public abstract class BaseEnemyMovement : MonoBehaviour
{
    protected Rigidbody rb;
    protected EnemyType _enemyType;
    public NavMeshPath path = new NavMeshPath();
    
    protected GameObject target;
    
    public float speed;

    protected LayerMask terrainLayer = LayerMask.GetMask("Terrain");
    
    public bool LOSToTarget;
    public bool grounded;
    
    protected List<EnemyMovementState> movementStates;
    protected EnemyMovementState currentState;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _enemyType = GetComponent<EnemyType>();
        target = GameObject.FindWithTag("Player");

        currentState = movementStates[0];
    }
    
    private void Update()
    {
        GroundCheck();
    }

    private void FixedUpdate()
    {
        currentState.Update();
    }
    
    protected void SwitchStates(EnemyMovementState nextState, float delay = 0f)
    {
        StartCoroutine(s());
        IEnumerator s()
        {
            yield return new WaitForSeconds(delay);
            currentState.Exit();
            currentState = nextState;
            currentState.Enter();
        }
    }
    
    private void GroundCheck()
    {
        Vector3 s = transform.localScale;
        if (Physics.OverlapBox(transform.position + Vector3.down * s.y / 2, new Vector3(s.x * 0.3f, s.y * 1, s.z * 0.3f), Quaternion.identity, terrainLayer).Length > 0)
            grounded = true;
        else grounded = false;
    }
}
