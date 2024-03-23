using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    public PlayerMovement PM;
    private LayerMask wallLayer;

    private void Start()
    {
        wallLayer = PM.wallLayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (wallLayer == (wallLayer | (1 << other.gameObject.layer)))
        {
            PM.walled = true;
            PM.wallColList.Add(other);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (wallLayer == (wallLayer | (1 << other.gameObject.layer)))
        {
            PM.walled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (wallLayer == (wallLayer | (1 << other.gameObject.layer)))
        {
            PM.walled = false;
            PM.wallColList.Remove(other);
        }
    }
}
