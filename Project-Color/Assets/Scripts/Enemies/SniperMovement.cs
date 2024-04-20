using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperMovement : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject target;
    
    public float speed;
    public bool LOSToPlayer;
    public bool grounded;

    private LayerMask terrainLayer;
    
    public enum State { idle, navmesh, los, attack };
    public State state;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindWithTag("Player");
        terrainLayer = LayerMask.GetMask("Terrain");
    }
    
    void Update()
    {
        Grounded();
        LOSToPlayer = !Physics.Linecast(transform.position, target.transform.position, terrainLayer);
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.idle:
                break;
            case State.los:
                break;
            case State.attack:
                break;
        }
    }
    
    private void Grounded()
    {
        if (Physics.OverlapBox(transform.position + Vector3.down * 0.5f, new Vector3(0.3f, 1, 0.3f), Quaternion.identity, terrainLayer).Length > 0)
            grounded = true;
        else grounded = false;
    }
}
