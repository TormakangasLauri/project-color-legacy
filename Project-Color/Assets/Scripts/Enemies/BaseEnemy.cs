using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 5f;
    public enum Type
    {
        basic,
        sniper,
        hulk,
        hanging
    };
    [SerializeField] protected Type type = Type.basic;
    [SerializeField] protected GameObject target;
    protected NavMeshPath path;
    [SerializeField] protected EnemyController _enemyController;

    private void Start()
    {
        target = GameObject.FindWithTag("Player");
    }
}
