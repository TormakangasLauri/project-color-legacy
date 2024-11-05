using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyType : MonoBehaviour
{
    public enum Type
    {
        basic,
        sniper,
        hulk,
        hanging
    };
    public Type type;
    private int typeIndex;

    public GameObject target;
    public NavMeshPath path;

    private EnemyController enemyController;

    public int killGroup;

    public bool active = false;
    public float timeActive = 0;

    public bool despawnEnemies = true;
    
    void Start()
    {
        target = GameObject.Find("Player");
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

        if (despawnEnemies) DeactivateOnStart();
    }

    private void Update()
    {
        if (active) timeActive += Time.deltaTime;
    }

    protected void DeactivateOnStart()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<PaintTarget>().enabled = false;
        transform.Find("PathFinder").gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Activate(int group = -1)
    {
        enemyController.all.MoveToActive(gameObject);
        enemyController.typeLists[typeIndex].MoveToActive(gameObject);
        
        // enemyController.allEnemies.Remove(gameObject);
        // enemyController.allEnemies_active.Add(gameObject);
        // switch (type)
        // {
        //     case Type.basic:
        //         enemyController.basicEnemyList.Remove(gameObject);
        //         enemyController.basicEnemyList_active.Add(gameObject);
        //         break;
        //     case Type.sniper:
        //         enemyController.sniperList.Remove(gameObject);
        //         enemyController.sniperList_active.Add(gameObject);
        //         break;
        //     case Type.hulk:
        //         enemyController.hulkList.Remove(gameObject);
        //         enemyController.hulkList_active.Add(gameObject);
        //         break;
        // }
        
        killGroup = group;
        
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<MeshCollider>().enabled = true;
        GetComponent<PaintTarget>().enabled = true;
        transform.Find("PathFinder").gameObject.SetActive(true);
        gameObject.SetActive(true);

        active = true;
    }

    public void Deactivate()
    {
        enemyController.all.MoveToInactive(gameObject);
        enemyController.typeLists[typeIndex].MoveToInactive(gameObject);
        
        // enemyController.allEnemies_active.Remove(gameObject);
        // enemyController.allEnemies.Add(gameObject);
        // switch (type)
        // {
        //     case Type.basic:
        //         enemyController.basicEnemyList_active.Remove(gameObject);
        //         enemyController.basicEnemyList.Add(gameObject);
        //         break;
        //     case Type.sniper:
        //         enemyController.sniperList_active.Remove(gameObject);
        //         enemyController.sniperList.Add(gameObject);
        //         break;
        //     case Type.hulk:
        //         enemyController.hulkList_active.Remove(gameObject);
        //         enemyController.hulkList.Add(gameObject);
        //         break;
        // }

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<PaintTarget>().enabled = false;
        transform.Find("PathFinder").gameObject.SetActive(false);
        gameObject.SetActive(false);

        transform.position = new Vector3(0, -500, 0);

        active = false;
        timeActive = 0;
    }
}
