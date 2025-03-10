using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
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

    public GameObject target;
    public NavMeshPath path;

    public int killGroup;

    public bool active = false;
    public float timeActive = 0;

    public bool despawnEnemies = true;

    void Start()
    {
        target = GameObject.Find("Player");

        switch (type)
        {
            case Type.basic: typeIndex = 0; break;
            case Type.sniper: typeIndex = 1; break;
            case Type.hulk: typeIndex = 2; break;
            case Type.hanging: typeIndex = 3; break;
        }

        // Add enemy to correct lists in enemycontroller
        EnemyController.all.inactiveList.Add(gameObject);
        EnemyController.typeLists[typeIndex].inactiveList.Add(gameObject);

        // Deactivate
        if (despawnEnemies)
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<MeshCollider>().enabled = false;
            GetComponent<PaintTarget>().enabled = false;
            transform.Find("PathFinder").gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        // Deactivate on start
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<PaintTarget>().enabled = false;
        transform.Find("PathFinder").gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (active) timeActive += Time.deltaTime;
    }

    public void Activate(int group = -1)
    {
        EnemyController.all.MoveToActive(gameObject);
        EnemyController.typeLists[typeIndex].MoveToActive(gameObject);

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
        EnemyController.all.MoveToInactive(gameObject);
        EnemyController.typeLists[typeIndex].MoveToInactive(gameObject);

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
