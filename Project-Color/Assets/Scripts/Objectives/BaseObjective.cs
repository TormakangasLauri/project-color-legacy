using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public bool active;
    public bool completed;

    public ObjectiveType type;

    private void OnTriggerEnter(Collider other)
    {
        // Start objective on trigger collision
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(StartObjective());
            GetComponent<Collider>().enabled = false;
        }
    }

    private IEnumerator StartObjective()
    {
        active = true;
        Objectives.NewObjective(this);
        yield return ObjectiveRequirement();
        Objectives.RemoveObjective(this);
        active = false;
        completed = true;
    }

    protected virtual IEnumerator ObjectiveRequirement()
    {
        yield return null;
    }
}
