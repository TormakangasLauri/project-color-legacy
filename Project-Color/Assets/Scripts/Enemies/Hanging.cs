using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hanging : EnemyType
{
    public GameObject hangPoint;
    
    private void Awake()
    {
        target = GameObject.FindWithTag("PlayerRoot");
        type = Type.hanging;
        deactivateOnStart = false;
    }

    void HangPoint()
    {
        
    }

    protected override void OnActivate()
    {
        
    }
}
