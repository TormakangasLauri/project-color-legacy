using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RunAnimationOnObjectiveComplete : MonoBehaviour
{
    public Objective objective;
    public Animation animation;
    private float timer;
    private bool hasplayed = false;
    void Update()
    {
        // Timer and check if objective is complete, then run animation attached to object if it has not been run before.
        timer += Time.deltaTime;
        if (timer > 0.5)
        {
            timer = 0;
            if (objective.completed && !hasplayed)
            {
                animation.Play();
                hasplayed = true;
            }
        }
    }
}