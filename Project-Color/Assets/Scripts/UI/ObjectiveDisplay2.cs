using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectiveDisplay2 : MonoBehaviour
{
    class ObjectiveText
    {
        public int line;
        public string text;
        public ObjectiveType type;

        public ObjectiveText(Objective objective)
        {
            line = objectives.Count;
            type = objective.type;
            objectives.Add(this);

            if (type == ObjectiveType.kill) text = $"{objective.type.ToString().FirstCharacterToUpper()}: {(objective as KillObjective).enemiesInObjective.Count}";
            else if (type == ObjectiveType.platform) text = $"{objective.type.ToString().FirstCharacterToUpper()}: Ongoing";
            else if (type == ObjectiveType.paint) text = $"{objective.type.ToString().FirstCharacterToUpper()}: {(objective as PaintObjective).paintPercentage}%";

            HUDText.SetText(line, text);
        }
    }

    int lastObjectiveCount = 0;

    static List<ObjectiveText> objectives = new List<ObjectiveText>();

    public static void NewObjective(Objective objective)
    {
        new ObjectiveText(objective);
    }

    public static void ObjectiveComplete(Objective objective)
    {

    }

    private void Update()
    {
        foreach (ObjectiveText obj in objectives)
        {

        }
    }
}
