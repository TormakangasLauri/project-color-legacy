using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObjective : Objective
{
    private Collider endTrigger;
    public int distance;
    
    void Start()
    {
        endTrigger = transform.GetChild(0).GetComponent<Collider>();
        endTrigger.enabled = false;

        type = ObjectiveType.platform;
    }

    private void Update()
    {
        distance = (int)Vector3.Distance(GameController.player.transform.position, endTrigger.transform.position);
    }

    protected override IEnumerator ObjectiveRequirement()
    {
        Debug.Log("Platform objective started");

        endTrigger.enabled = true;
        yield return new WaitUntil(() => endTrigger.bounds.Contains(GameController.player.transform.position));

        Debug.Log("Platform objective complete");
    }
}
