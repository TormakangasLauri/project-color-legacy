using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyType : MonoBehaviour
{
    public enum Type
    {
        basic,
        sniper,
        MILO
    };
    public Type type;

    public GameObject target;
    public NavMeshPath path;

    private EnemyController enemyController;

    public int killGroup;
    
    void Start()
    {
        target = GameObject.Find("Player");
        enemyController = GameObject.Find("GameController").GetComponent<EnemyController>();

        switch (type)
        {
            case Type.basic:
                enemyController.basicEnemyList.Add(gameObject);
                break;
            case Type.sniper:
                enemyController.sniperList.Add(gameObject);
                break;
            case Type.MILO:
                enemyController.MILOList.Add(gameObject);
                break;
        }
        enemyController.allEnemies.Add(gameObject);

        // Deactivate
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<PaintTarget>().enabled = false;
        transform.Find("PathFinder").gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Activate(int group = -1)
    {
        enemyController.allEnemies.Remove(gameObject);
        enemyController.allEnemies_active.Add(gameObject);
        switch (type)
        {
            case Type.basic:
                enemyController.basicEnemyList.Remove(gameObject);
                enemyController.basicEnemyList_active.Add(gameObject);
                break;
            case Type.sniper:
                enemyController.sniperList.Remove(gameObject);
                enemyController.sniperList_active.Add(gameObject);
                break;
            case Type.MILO:
                enemyController.MILOList.Remove(gameObject);
                enemyController.MILOList_active.Add(gameObject);
                break;
        }
        killGroup = group;
        
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<MeshCollider>().enabled = true;
        GetComponent<PaintTarget>().enabled = true;
        transform.Find("PathFinder").gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        enemyController.allEnemies_active.Remove(gameObject);
        enemyController.allEnemies.Add(gameObject);
        switch (type)
        {
            case Type.basic:
                enemyController.basicEnemyList_active.Remove(gameObject);
                enemyController.basicEnemyList.Add(gameObject);
                break;
            case Type.sniper:
                enemyController.sniperList_active.Remove(gameObject);
                enemyController.sniperList.Add(gameObject);
                break;
            case Type.MILO:
                enemyController.MILOList_active.Remove(gameObject);
                enemyController.MILOList.Add(gameObject);
                break;
        }
        
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<PaintTarget>().enabled = false;
        transform.Find("PathFinder").gameObject.SetActive(false);
        gameObject.SetActive(false);

        transform.position = new Vector3(0, -500, 0);
    }
}
