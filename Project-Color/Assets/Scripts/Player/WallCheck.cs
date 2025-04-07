using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class WallCheck : MonoBehaviour
{
    private PlayerMovement PM;
    private LayerMask wallLayer;

    private void Start()
    {
        PM = transform.parent.GetComponent<PlayerMovement>();
        wallLayer = PM.wallLayer;
    }

    private void Update()
    {
        if (PM.wallColList.Count > 0) PM.walled = true;
        else
        {
            PM.walled = false;
            PM.wallRunning = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (wallLayer == (wallLayer | (1 << other.gameObject.layer)))
        {
            if (PM.wallColList.Count > 0) PM.wallColList.Clear();
            PM.wallColList.Add(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (wallLayer == (wallLayer | (1 << other.gameObject.layer)))
        {
            PM.wallColList.Remove(other);
        }
    }
}
