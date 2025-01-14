using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Controllers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Objectives : MonoBehaviour
{
    private ObjectiveDisplay objectiveDisplay;
    
    public List<Objective> activeObjectives = new List<Objective>();

    private void Start()
    {
        objectiveDisplay = GameObject.FindWithTag("PlayerRoot").transform.GetChild(6).GetChild(1).gameObject.GetComponent<ObjectiveDisplay>(); // Player > HUD > Objectives
    }

    public void NewObjective(Objective objective)
    {
        activeObjectives.Add(objective);
        objectiveDisplay.NewObjective(objective);
    }

    public void RemoveObjective(Objective objective)
    {
        activeObjectives.Remove(objective);
        objectiveDisplay.ObjectiveComplete(objective);
    }
}
