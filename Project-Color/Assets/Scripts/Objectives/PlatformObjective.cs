using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObjective : Objective
{
    private Collider endTrigger;
    private GameObject player;
    
    void Start()
    {
        endTrigger = transform.GetChild(0).GetComponent<Collider>();
        endTrigger.enabled = false;

        player = GameObject.FindWithTag("Player");
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    // Start objective on trigger collision
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        if (!active)
    //        {
    //            //GetComponent<Collider>().enabled = false;

    //            endTrigger.enabled = true;

    //            active = true;
    //            Objectives.NewObjective(this);
    //            Debug.Log("Platform objective started");
    //        }
    //        else
    //        {
    //            active = false;
    //            completed = true;
    //            Debug.Log("Platform objective complete");
    //            Objectives.RemoveObjective(this);
    //        }
    //    }
    //}

    protected override IEnumerator ObjectiveRequirement()
    {
        Debug.Log("Platform objective started");

        yield return new WaitUntil(() => endTrigger.bounds.Contains(player.transform.position));

        Debug.Log("Platform objective complete");
    }
}
