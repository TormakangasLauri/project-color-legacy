using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Scene level;
    public Camera cam;
    public string[] menuTexts;

    private RectTransform rectT;
    private List<Transform> textLines = new List<Transform>();
    private Transform lastTransform;
    private int lastLine;

    private void OnValidate()
    {
        textLines.Clear();
        rectT = GetComponent<RectTransform>();
        foreach (Transform textTransform in GetComponentInChildren<HUDText>().transform)
            textLines.Add(textTransform);

        string[] menuTextContents = menuTexts; // Save text contents and move them to the new array
        if (menuTexts == null || menuTexts.Length != textLines.Count) menuTexts = new string[textLines.Count];
        for (int i = 0; i < menuTexts.Length; i++) menuTexts[i] = menuTextContents[i];

        int[] lines = new int[textLines.Count];
        for (int i = 0; i < menuTexts.Length; i++) lines[i] = i;
        HUDText.SetText(lines, menuTexts);
    }

    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        int targetLine = 0;

        if (mousePos.x < rectT.sizeDelta.x * 0.3f)
        {
            targetLine = 0;
            for (int i = 1; i < textLines.Count; i++)
                if (RectTransformUtility.RectangleContainsScreenPoint(HUDText.textColumns[i].GetComponent<RectTransform>(), mousePos, cam))
                {
                    targetLine = i;
                    break;
                }
        }

        // Check if any text needs updating and update them accordingly
        List<int> linesToSet = new List<int>();
        List<string> textsToSet = new List<string>();
        for (int i = 0; i < textLines.Count; i++)
        {
            if (i != 0 && HUDText.textContents[i] != "")
            {
                if (i == targetLine && HUDText.textContents[i] != $"> {menuTexts[i]}")
                {
                    linesToSet.Add(i);
                    textsToSet.Add($"> {menuTexts[i]}");
                }
                else if (i != targetLine && HUDText.textContents[i] != menuTexts[i])
                {
                    linesToSet.Add(i);
                    textsToSet.Add(menuTexts[i]);
                }
            }
        }
        if (linesToSet.Count > 0) HUDText.SetText(linesToSet.ToArray(), textsToSet.ToArray(), HUDTextUpdate.Single);

        lastLine = targetLine;
    }

    public void Play()
    {
        Debug.Log("Play");
        GameController.StartLevel(1);
    }

    public void Levels()
    {
        Debug.Log("Levels");
    }
}
