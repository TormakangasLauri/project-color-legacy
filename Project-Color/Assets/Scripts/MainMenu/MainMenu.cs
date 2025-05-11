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
    private string[] highlightedMenuTexts;

    private RectTransform rectT;
    private List<Transform> textLines = new List<Transform>();
    private Transform lastTransform;
    private int lastLine;

    private void OnValidate()
    {
        textLines.Clear();
        rectT = GetComponent<RectTransform>();
        foreach (Transform textTransform in GetComponentInChildren<HUDText2>().transform)
            textLines.Add(textTransform);

        string[] menuTextContents = menuTexts; // Save text contents and move them to the new array
        highlightedMenuTexts = new string[menuTexts.Length];
        if (menuTexts == null || menuTexts.Length != textLines.Count) menuTexts = new string[textLines.Count];
        for (int i = 0; i < menuTexts.Length; i++)
        {
            menuTexts[i] = menuTextContents[i];
            highlightedMenuTexts[i] = "> " + menuTexts[i];
        }

        int[] lines = new int[textLines.Count];
        for (int i = 0; i < menuTexts.Length; i++) lines[i] = i;
        HUDText2.SetText(lines, menuTexts);
    }

    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        int targetLine = 0;

        if (mousePos.x < rectT.sizeDelta.x * 0.3f)
        {
            for (int i = 1; i < textLines.Count; i++)
                if (RectTransformUtility.RectangleContainsScreenPoint(HUDText2.textLines[i].GetComponent<RectTransform>(), mousePos, cam))
                {
                    targetLine = i;
                    break;
                }
        }

        // Check if any text needs updating and update them accordingly
        List<int> linesToSet = new List<int>();
        List<string> textsToSet = new List<string>();
        for (int i = 1; i < textLines.Count; i++)
        {
            if (HUDText2.textContents[i].Length > 0)
            {
                if (i == targetLine && HUDText2.textContents[i] != highlightedMenuTexts[i])
                {
                    linesToSet.Add(i);
                    textsToSet.Add(highlightedMenuTexts[i]);
                }
                else if (i != targetLine && HUDText2.textContents[i] != menuTexts[i])
                {
                    linesToSet.Add(i);
                    textsToSet.Add(menuTexts[i]);
                }
            }
        }
        if (linesToSet.Count > 0) HUDText2.SetText(linesToSet.ToArray(), textsToSet.ToArray(), HUDTextUpdate.Single);

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
