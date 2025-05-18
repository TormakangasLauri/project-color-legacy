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
        for (int i = 0; i < menuTexts.Length; i++)
        {
            menuTexts[i] = menuTextContents[i];
        }

        int[] lines = new int[textLines.Count];
        for (int i = 0; i < menuTexts.Length; i++) lines[i] = i;
        HUDText.SetText(lines, menuTexts);
    }

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        HUDText.SetInteractableLines(new[]{0}, true);
    }

    private void Update()
    {
        int targetLine = HUDText.GetHoveredLine();
        HUDText.UpdateInteractableText(menuTexts);

        if (Input.GetMouseButtonUp(0) && targetLine != -1)
        {
            switch (menuTexts[targetLine])
            {
                case "Play": Play(); break;
                case "Levels": Levels(); break;
                case "test": Debug.Log("test"); break;
            }
        }
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
