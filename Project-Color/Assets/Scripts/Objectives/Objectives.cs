using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Controllers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum ObjectiveType { kill, platform, paint }

public class Objectives : MonoBehaviour
{
    public static List<Objective> activeObjectives = new List<Objective>();

    public static void NewObjective(Objective objective)
    {
        activeObjectives.Add(objective);
        //objectiveDisplay.NewObjective(objective);
        ObjectiveDisplay2.NewObjective(objective);
    }

    public static void RemoveObjective(Objective objective)
    {
        activeObjectives.Remove(objective);
        //objectiveDisplay.ObjectiveComplete(objective);
        ObjectiveDisplay2.ObjectiveComplete(objective);
    }
}
