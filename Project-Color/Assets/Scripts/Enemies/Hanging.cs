using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

public class Hanging : EnemyType
{
    public GameObject hangPointPrefab;
    public GameObject ropeJointPrefab;
    [HideInInspector] public GameObject hangPoint;

    public GameObject targetPaintGroup;
    public Vector3 targetPoint;
    public Vector3 targetDirection;

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
        if (PaintController.paintGroupsInPaintArea.Count > 0) // Paint objective active
        {
            targetPaintGroup = PaintController.paintGroupsInPaintArea[(int)(Random.Range(0, PaintController.paintGroupsInPaintArea.Count - 1))];

            targetPoint = targetPaintGroup.transform.position + targetPaintGroup.transform.forward * 2;
            targetDirection = -targetPaintGroup.transform.forward;
            gameObject.transform.position = targetPoint + Vector3.up * hangPointHeight;
            HangPoint();
        }
        else if (PaintController.paintGroups.Count > 0) // No objective
        {
            targetPaintGroup = PaintController.paintGroups[(int)(Random.Range(0, PaintController.paintGroups.Count - 1))];

            targetPoint = targetPaintGroup.transform.position + targetPaintGroup.transform.forward * 2;
            targetDirection = -targetPaintGroup.transform.forward;
            gameObject.transform.position = targetPoint + Vector3.up * hangPointHeight;
            HangPoint();
        }
        else // No paint
        {
            float maxDist = 15;
            float minDist = 8;
            bool spawned = false;

            for (int i = 0; i < 100; i++)
            {
                Vector3 direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                float distance = Random.Range(minDist, maxDist);
                Vector3 spawnPoint = target.transform.position + direction * distance;

                if (!Physics.Raycast(spawnPoint + Vector3.up * (hangPointHeight + 1), Vector3.down, hangPointHeight, LayerMask.GetMask("Terrain")))
                {
                    gameObject.transform.position = spawnPoint + Vector3.up * hangPointHeight;
                    HangPoint();
                    spawned = true;
                    break;
                }
            }
            if (!spawned) Deactivate();
        }
    }

    protected override void OnDeactivate()
    {
        Destroy(hangPoint);
    }
}
