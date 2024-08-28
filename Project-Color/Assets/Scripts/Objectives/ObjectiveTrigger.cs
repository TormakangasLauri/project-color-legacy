using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    public enum Objective
    {
        kill,
        platform_start,
        platform_end,
        paint
    };

    public int killGroup;

    public Objective objective;

    public Objectives Objectives;

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            switch (objective)
            {
                case Objective.kill:
                    StartCoroutine(Objectives.Kill(killGroup));
                    break;
                case Objective.platform_start:
                    StartCoroutine(Objectives.Platform());
                    break;
                case Objective.platform_end:
                    Objectives.platformActive = false;
                    break;
                case Objective.paint:
                    StartCoroutine(Objectives.Paint());
                    break;
            }
        }
    }
}
