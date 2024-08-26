using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    public enum Objective
    {
        kill,
        platform,
        paint
    };

    public Objective objective;

    public Objectives Objectives;

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            switch (objective)
            {
                case Objective.kill:
                    StartCoroutine(Objectives.Kill());
                    break;
                case Objective.platform:
                    StartCoroutine(Objectives.Platform());
                    break;
                case Objective.paint:
                    StartCoroutine(Objectives.Paint());
                    break;
            }
        }
    }
}
