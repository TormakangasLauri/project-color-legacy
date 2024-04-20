using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType : MonoBehaviour
{
    public enum Type
    {
        basic,
        sniper,
        MILO
    };
    public Type type;
    
    void Start()
    {
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
