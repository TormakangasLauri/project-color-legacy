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

    protected override IEnumerator ObjectiveRequirement()
    {
        Debug.Log("Platform objective started");

        endTrigger.enabled = true;
        yield return new WaitUntil(() => endTrigger.bounds.Contains(player.transform.position));

        Debug.Log("Platform objective complete");
    }
}
