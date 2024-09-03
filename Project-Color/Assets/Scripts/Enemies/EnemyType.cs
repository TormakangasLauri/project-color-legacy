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

    public int killGroup;
    
    void Start()
    {
        target = GameObject.FindWithTag("Player");

        switch (type)
        {
            case Type.basic:
                EnemyController.inst.basicEnemyList.Add(gameObject);
                break;
            case Type.sniper:
                EnemyController.inst.sniperList.Add(gameObject);
                break;
            case Type.MILO:
                EnemyController.inst.MILOList.Add(gameObject);
                break;
        }

        Deactivate();
    }

    public void Activate()
    {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<MeshCollider>().enabled = true;
        GetComponent<PaintTarget>().enabled = true;
        transform.Find("PathFinder").gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<PaintTarget>().enabled = false;
        transform.Find("PathFinder").gameObject.SetActive(false);
        gameObject.SetActive(false);

        transform.position = new Vector3(0, -500, 0);
    }

    private void OnDestroy()
    {
        switch (type)
        {
            case Type.basic:
                EnemyController.inst.basicEnemyList.Remove(gameObject);
                break;
            case Type.sniper:
                EnemyController.inst.sniperList.Remove(gameObject);
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
