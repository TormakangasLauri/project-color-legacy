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
        hanging
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

    public bool deactivateOnStart = true;

    private void Awake()
    {
        // target = GameObject.Find("Player 2");
        target = GameObject.FindWithTag("PlayerRoot");
    }

    void Start()
    {
        enemyController = GameObject.Find("GameController").GetComponent<EnemyController>();

        switch (type)
        {
            case Type.basic: typeIndex = 0; break;
            case Type.sniper: typeIndex = 1; break;
            case Type.hulk: typeIndex = 2; break;
            case Type.hanging: typeIndex = 3; break;
        }
        
        // Add enemy to correct lists in enemycontroller
        enemyController.all.inactiveList.Add(gameObject);
        enemyController.typeLists[typeIndex].inactiveList.Add(gameObject);

        if (deactivateOnStart) DeactivateOnStart();
    }

    private void Update()
    {
        if (active) timeActive += Time.deltaTime;
    }

    protected virtual void DeactivateOnStart()
    {
        // GetComponent<MeshRenderer>().enabled = false;
        // GetComponent<MeshCollider>().enabled = false;
        // GetComponent<PaintTarget>().enabled = false;
        // transform.Find("PathFinder").gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Activate(Vector3 position, int group = -1)
    {
        transform.position = position;
        rb.velocity = Vector3.zero;
        
        enemyController.all.MoveToActive(gameObject);
        enemyController.typeLists[typeIndex].MoveToActive(gameObject);
        
        killGroup = group;
        
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<MeshCollider>().enabled = true;
        GetComponent<PaintTarget>().enabled = true;
        transform.Find("PathFinder").gameObject.SetActive(true);
        gameObject.SetActive(true);

        active = true;
        
        OnActivate();
    }

    public void Deactivate()
    {
        enemyController.all.MoveToInactive(gameObject);
        enemyController.typeLists[typeIndex].MoveToInactive(gameObject);

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<PaintTarget>().enabled = false;
        transform.Find("PathFinder").gameObject.SetActive(false);
        gameObject.SetActive(false);

        transform.position = new Vector3(0, -500, 0);

        active = false;
        timeActive = 0;
        
        OnDeactivate();
    }

    protected virtual void OnActivate()
    {
        
    }

    protected virtual void OnDeactivate()
    {
        
    }
}
