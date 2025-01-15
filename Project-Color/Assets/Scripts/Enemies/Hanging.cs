using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hanging : EnemyType
{
    public GameObject hangPointPrefab;
    [HideInInspector] public GameObject hangPoint;
    
    private void Awake()
    {
        target = GameObject.FindWithTag("PlayerRoot");
        type = Type.hanging;
        deactivateOnStart = false;
        
        HangPoint();
    }

    void HangPoint()
    {
        hangPoint = Instantiate(hangPointPrefab, transform.position + Vector3.up * 15, Quaternion.identity);
    }

    protected override void OnActivate()
    {
        
    }
}
