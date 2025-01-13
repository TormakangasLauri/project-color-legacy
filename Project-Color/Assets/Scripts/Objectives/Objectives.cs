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
    public List<Objective> activeObjectives = new List<Objective>();

    private float t;
    private float x = 0.2f;

    public void NewObjective(Objective objective)
    {
        activeObjectives.Add(objective);
    }

    private void Update()
    {
        t += Time.deltaTime;
        if (t > x)
        {
            t = 0;
            foreach (Objective objective in activeObjectives)
            {
                if (objective.completed) RemoveObjective(objective);
            }
        }
    }

    void RemoveObjective(Objective objective)
    {
        activeObjectives.Remove(objective);
    }
}
