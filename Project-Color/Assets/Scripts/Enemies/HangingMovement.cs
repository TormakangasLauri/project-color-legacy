using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingMovement : BaseEnemyMovement
{
    private GameObject hangPoint;
    
    void Start()
    {
        hangPoint = GetComponent<Hanging>().hangPoint;
    }
    
    void Update()
    {
        
    }
}
