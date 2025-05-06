using System;
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
        private string typeAsString;
        public Objective obj;
        public static List<ObjectiveText> objTexts = new List<ObjectiveText>();
        private static bool completitionInProgress = false;
        public bool completed = false;

        public ObjectiveText(Objective objective)
        {
            obj = objective;
            line = objectives.Count;
            type = objective.type;
            typeAsString = objective.type.ToString().FirstCharacterToUpper();

            text = GetObjectiveText();

            objectives.Add(this);
            objTexts.Add(this);
            HUDText.SetText(line, text);
        }

        private string GetObjectiveText()
        {
            switch (type)
            {
                case ObjectiveType.kill: return $"{typeAsString} - Enemies:{(obj as KillObjective).enemiesInObjective.Count}";
                case ObjectiveType.platform: return $"{typeAsString} - Distance:{(obj as PlatformObjective).distance}";
                case ObjectiveType.paint: return $"{typeAsString} - Painted:{(obj as PaintObjective).paintPercentage}%";
                default: return null;
            }
        }

        public static void UpdateAllText()
        {
            List<int> linesToUpdate = new List<int>();
            List<string> textsToUpdate = new List<string>();
            foreach (ObjectiveText objText in objTexts)
            {
                if (objText.text != objText.GetObjectiveText() && !objText.completed)
                {
                    objText.text = objText.GetObjectiveText();
                    linesToUpdate.Add(objText.line);
                    textsToUpdate.Add(objText.text);
                }
            }
            if (linesToUpdate.Count > 0) HUDText.SetText(linesToUpdate.ToArray(), textsToUpdate.ToArray(), HUDTextUpdate.Single);
        }

        public void SetText(string text)
        {
            this.text = text;
            HUDText.SetText(line, text, HUDTextUpdate.Single);
        }

        public IEnumerator Complete(float time)
        {
            SetText($"{typeAsString}: Completed");
            completed = true;
            yield return new WaitUntil(() => { return !completitionInProgress; }); // Complete objectives one at a time to avoid issues
            completitionInProgress = true;
            for (int i = 0; i < 3; i++)
            {
                if (i == 0) SetText($"! {text} !");
                else SetText($"!{text}!");
                yield return new WaitForSeconds(time / 3);
            }
            objectives.Remove(this);
            objTexts.Remove(this);
            List<int> linesToMove = new List<int>();
            foreach (ObjectiveText objText in objectives)
                if (objText.line > line)
                {
                    linesToMove.Add(objText.line);
                    objText.line--;
                }
            if (linesToMove.Count > 0) HUDText.MoveTextUp(linesToMove.ToArray());
            else HUDText.SetText(line, "");
                completitionInProgress = false;
        }
    }

    static List<ObjectiveText> objectives = new List<ObjectiveText>();
    static ObjectiveDisplay2 inst;

    private void Awake() { inst = this; }

    public static void NewObjective(Objective objective)
    {
        new ObjectiveText(objective);
    }

    public static void ObjectiveComplete(Objective objective)
    {
        foreach (ObjectiveText objText in objectives)
            if (objText.obj == objective) inst.StartCoroutine(objText.Complete(2.5f));
    }

    float timer = 0;
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 1;
            ObjectiveText.UpdateAllText();
        }
    }
}
