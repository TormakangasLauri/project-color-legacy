using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hanging : EnemyType
{
    public GameObject hangPointPrefab;
    public GameObject ropeJointPrefab;
    [HideInInspector] public GameObject hangPoint;

    public GameObject targetPaintGroup;
    public Vector3 targetPoint;

    public float hangPointHeight = 10;
    
    private void Awake()
    {
        target = GameObject.FindWithTag("PlayerRoot");
        type = Type.hanging;
        //deactivateOnStart = false;
    }

    void HangPoint()
    {
        hangPoint = Instantiate(hangPointPrefab, transform.position, Quaternion.identity);
        SpringJoint joint = hangPoint.GetComponent<SpringJoint>();
        joint.maxDistance = hangPointHeight;

        joint.connectedBody = gameObject.GetComponent<Rigidbody>();
        //hangPoint.transform.position += Vector3.up * hangPointHeight;

        // GameObject lastRopeJoint = gameObject;
        
        // float gap = Vector3.Distance(transform.position, hangPoint.transform.position);
        // for (float i = 1; i <= gap - 0.5; i += 0.5f) // Create the rope with ropejoints starting from bottom
        // {
        //     Vector3 pos = transform.position + Vector3.up * i;
        //     GameObject ropeJoint = Instantiate(ropeJointPrefab, pos, Quaternion.identity);
        //     ropeJoint.GetComponent<Joint>().connectedBody = lastRopeJoint.GetComponent<Rigidbody>();

        //     lastRopeJoint = ropeJoint;
        // }
        
        // joint.connectedBody = lastRopeJoint.GetComponent<Rigidbody>();
    }

    protected override void OnActivate()
    {
        // Pick a random paint group in the paint area
        //targetPaintGroup = PaintController.paintGroupsInPaintArea[(int)(Random.Range(0, PaintController.paintGroupsInPaintArea.Count - 1))];
        targetPaintGroup = PaintController.paintGroups[(int)(Random.Range(0, PaintController.paintGroupsInPaintArea.Count - 1))];

        targetPoint = targetPaintGroup.transform.position + targetPaintGroup.transform.forward * 2;

        HangPoint();
        gameObject.transform.position = targetPoint + Vector3.up * hangPointHeight;
    }

    protected override void OnDeactivate()
    {
        Destroy(hangPoint);
    }
}
