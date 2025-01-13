using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public bool active;
    public bool completed;

    protected Objectives objectives;

    private void Awake()
    {
        objectives = GameObject.Find("GameController").GetComponent<Objectives>();
    }
}
