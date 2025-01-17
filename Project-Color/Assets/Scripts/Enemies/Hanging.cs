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
        hangPoint = Instantiate(hangPointPrefab, transform.position, Quaternion.identity); // Replace with some kind of rope or whatever
        Joint joint = hangPoint.GetComponent<Joint>();
        joint.connectedBody = GetComponent<Rigidbody>();
        hangPoint.transform.position += Vector3.up * 10;
    }

    protected override void OnActivate()
    {
        
    }
}
