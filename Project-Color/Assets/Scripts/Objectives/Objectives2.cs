using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objectives2 : MonoBehaviour
{
    private List<Objective> objectives = new List<Objective>();
    private List<Objective> activeObjectives = new List<Objective>();
    
    void Start()
    {
        objectives.AddRange(GameObject.FindObjectsOfType<Objective>());
    }
    
    void Update()
    {
        
    }
}
