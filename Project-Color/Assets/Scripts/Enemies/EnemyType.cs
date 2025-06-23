using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyType : MonoBehaviour
{
    protected enum Type
    {
        basic,
        sniper,
        hulk,
        hanging,
        copter
    };
    protected Type type;
    private int typeIndex;
    
    private Rigidbody rb;

    public GameObject target;
    public NavMeshPath path;

    private EnemyController enemyController;

    public int killGroup;

    public bool active = false;
    public float timeActive = 0;
    public float timeScale = 1;

    public bool deactivateOnStart = true;

    void Start()
    {
        enemyController = GameObject.Find("GameController").GetComponent<EnemyController>();
        rb = GetComponent<Rigidbody>();
        target = GameController.player;

        switch (type)
        {
            case Type.basic: typeIndex = 0; break;
            case Type.sniper: typeIndex = 1; break;
            case Type.hulk: typeIndex = 2; break;
            case Type.hanging: typeIndex = 3; break;
            case Type.copter: typeIndex = 4; break;
        }
        
        // Add enemy to correct lists in enemycontroller
        EnemyController.all.inactiveList.Add(gameObject);
        EnemyController.typeLists[typeIndex].inactiveList.Add(gameObject);

        if (deactivateOnStart) DeactivateOnStart();
        else Activate();
    }

    private void Update()
    {
        if (active) timeActive += Time.deltaTime;
        if (target == null) target = GameController.player;
    }

    protected virtual void DeactivateOnStart()
    {
        // GetComponent<MeshRenderer>().enabled = false;
        // GetComponent<MeshCollider>().enabled = false;
        // GetComponent<PaintTarget>().enabled = false;
        // transform.Find("PathFinder").gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Activate(int group = -1) // Activate
    {
        rb.velocity = Vector3.zero;
        killGroup = group;
        
        EnemyController.all.MoveToActive(gameObject);
        EnemyController.typeLists[typeIndex].MoveToActive(gameObject);
        
        gameObject.SetActive(true);
        active = true;
        
        OnActivate();
    }
    public void Activate(Vector3 position, int group = -1) // Activate and move to the specified position
    {
        transform.position = position;
        rb.velocity = Vector3.zero;
        killGroup = group;
        
        EnemyController.all.MoveToActive(gameObject);
        EnemyController.typeLists[typeIndex].MoveToActive(gameObject);
        
        gameObject.SetActive(true);
        active = true;
        
        OnActivate();
    }

    public void Deactivate()
    {
        transform.position = new Vector3(0, -500, 0);

        EnemyController.all.MoveToInactive(gameObject);
        EnemyController.typeLists[typeIndex].MoveToInactive(gameObject);

        gameObject.SetActive(false);
        active = false;
        timeActive = 0;
        
        OnDeactivate();
    }

    protected virtual void OnActivate(){}
    protected virtual void OnDeactivate(){}
}
