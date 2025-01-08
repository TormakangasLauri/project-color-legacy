using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObjective : Objective
{
    private Collider endTrigger;
    
    void Start()
    {
        endTrigger = transform.GetChild(0).GetComponent<Collider>();
        endTrigger.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Start objective on trigger collision
        if (other.gameObject.CompareTag("Player"))
        {
            if (!active)
            {
                GetComponent<Collider>().enabled = false;
                endTrigger.enabled = true;

                active = true;
                Debug.Log("Platform objective started");
            }
            else
            {
                active = false;
                completed = true;
                Debug.Log("Platform objective complete");
            }
        }
    }
}
