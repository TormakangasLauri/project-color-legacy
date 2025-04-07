using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveDisplay : MonoBehaviour
{
    public class ObjectiveBlock
    {
        public GameObject gameObject;
        public Objective objective;
        public ObjectiveType type;

        public Color backgroundColor;
        public TextMeshProUGUI text;

        public int order;
        public static int objectives;

        public ObjectiveBlock(Objective objective)
        {
            gameObject = Instantiate(objectiveBlockPrefab);
            gameObject.transform.parent = _transform;
            text = gameObject.GetComponentInChildren<TextMeshProUGUI>();

            this.objective = objective;
            type = objective.type;
            order = objectives + 1;

            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, objectives * -25);

            objectives++;
        }


    }

    public GameObject objectiveBlock;
    static GameObject objectiveBlockPrefab;
    static Transform _transform;

    private static List<ObjectiveBlock> objectiveBlocks = new List<ObjectiveBlock>();

    private void Awake()
    {
        objectiveBlockPrefab = objectiveBlock;
        _transform = transform;
    }

    public void NewObjective(Objective objective) // Add a new objective to the display
    {
        ObjectiveBlock block = new ObjectiveBlock(objective);
        objectiveBlocks.Add(block);
    }

    public void ObjectiveComplete(Objective objective) // Remove an objective from the display after completion
    {
        foreach (ObjectiveBlock block in objectiveBlocks)
            if (block.objective == objective)
            {
                objectiveBlocks.Remove(block);
                break;
            }

        foreach (ObjectiveBlock block in objectiveBlocks) // Reorder blocks in the display
            block.gameObject.GetComponent<RectTransform>().position = new Vector2(0, -25 * objectiveBlocks.IndexOf(block));
    }

    private void Update()
    {
        foreach (ObjectiveBlock block in objectiveBlocks)
            switch (block.type)
            {
                case ObjectiveType.kill: Kill(block);  break;
                case ObjectiveType.platform: Platform(block); break;
                case ObjectiveType.paint: Paint(block); break;
            }
    }

    void Kill(ObjectiveBlock block)
    {
        KillObjective objective = block.objective as KillObjective;

        block.text.text = $"Kill - Enemies: {objective.enemiesInObjective.Count}";
    }

    void Platform(ObjectiveBlock block)
    {
        PlatformObjective objective = block.objective as PlatformObjective;

        block.text.text = "Platform";
    }

    void Paint(ObjectiveBlock block)
    {
        PaintObjective objective = block.objective as PaintObjective;

        block.text.text = $"Paint - Painted: {objective.paintPercentage}%";

    }
}
