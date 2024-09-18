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

    public Objective objective;

    public bool destroyOnTrigger = true;
    public int killGroup;

    public Collider paintArea;

    Objectives Objectives;

    private void Start()
    {
        Objectives = GameObject.Find("GameController").GetComponent<Objectives>();
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            switch (objective)
            {
                case Objective.kill:
                    Objectives.KillObj(killGroup);
                    break;
                case Objective.platform_start:
                    StartCoroutine(Objectives.Platform());
                    break;
                case Objective.platform_end:
                    Objectives.platformActive = false;
                    break;
                case Objective.paint:
                    Objectives.PaintObj(paintArea);
                    break;
            }

            if (destroyOnTrigger) Destroy(gameObject);
        }
    }
}
