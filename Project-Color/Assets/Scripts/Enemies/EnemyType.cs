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
    
    void Start()
    {
        target = GameObject.FindWithTag("Player");

        switch (type)
        {
            case Type.basic:
                EnemyController.inst.basicEnemyList.Add(gameObject);
                break;
            case Type.sniper:
                EnemyController.inst.SniperList.Add(gameObject);
                break;
            case Type.MILO:
                EnemyController.inst.MILOList.Add(gameObject);
                break;
        }

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<PaintTarget>().enabled = false;
        transform.Find("PathFinder").gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<MeshCollider>().enabled = true;
        GetComponent<PaintTarget>().enabled = true;
        transform.Find("PathFinder").gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        switch (type)
        {
            case Type.basic:
                EnemyController.inst.basicEnemyList.Remove(gameObject);
                break;
            case Type.sniper:
                EnemyController.inst.SniperList.Remove(gameObject);
                break;
            case Type.MILO:
                EnemyController.inst.MILOList.Remove(gameObject);
                break;
        }
        
        if (PlayerAttack.inst.enemies.Contains(gameObject))
        {
            PlayerAttack.inst.enemies.Remove(gameObject);
        }
    }
}
