using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hanging : EnemyType
{
    public GameObject hangPointPrefab;
    public GameObject ropeJointPrefab;
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
        hangPoint.transform.position += Vector3.up * 10;

        GameObject lastRopeJoint = gameObject;
        
        float gap = Vector3.Distance(transform.position, hangPoint.transform.position);
        for (float i = 1; i <= gap - 0.5; i += 0.5f) // Create the rope with ropejoints starting from bottom
        {
            Vector3 pos = transform.position + Vector3.up * i;
            GameObject ropeJoint = Instantiate(ropeJointPrefab, pos, Quaternion.identity);
            ropeJoint.GetComponent<Joint>().connectedBody = lastRopeJoint.GetComponent<Rigidbody>();

            lastRopeJoint = ropeJoint;
        }
        
        joint.connectedBody = lastRopeJoint.GetComponent<Rigidbody>();
    }

    protected override void OnActivate()
    {
        
    }
}
