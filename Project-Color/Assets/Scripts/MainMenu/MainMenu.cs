using System.Collections;
using System.Collections.Generic;
using AASave;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject saveSystemObject;
    public GameObject levelLoaderObject;

    public SaveSystem saveSystem;
    public string[] menuTexts;
    public string[] levelsTexts;

    private List<Transform> textLines = new List<Transform>();

    private bool updateText = false;
    private static bool firstLoad = true;

    enum MenuScreen { main, levels }
    MenuScreen menuScreen;

    private void Awake()
    {
        if (firstLoad && !GameObject.Find("Save System(Clone)") && !GameObject.Find("LevelLoader(Clone)"))
        {
            firstLoad = false;

            GameObject s = Instantiate(saveSystemObject);
            saveSystem = s.GetComponent<SaveSystem>();
            GameController.saveSystem = saveSystem;
            GameData.saveSystem = saveSystem;

            GameObject l = Instantiate(levelLoaderObject);
            GameController.levelLoader = l.GetComponent<LevelLoader>();
        }
    }

    private void Start()
    {
        StartCoroutine(Wait());
        IEnumerator Wait()
        {
            yield return new WaitForSecondsRealtime(1);
            Debug.Log("Main menu Start");
            textLines.Clear();
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

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            HUDText.SetInteractableLines(new[]{0}, true);

            GameData.LoadData(1);

            updateText = true;
        }
    }

    private void Update()
    {
        if (updateText)
        {
            if (menuScreen == MenuScreen.main) // Main menu
            {
                int targetLine = HUDText.GetHoveredLine();
                HUDText.UpdateInteractableText(menuTexts);

                if (Input.GetMouseButtonUp(0) && targetLine != -1)
                {
                    switch (menuTexts[targetLine])
                    {
                        case "Play": Play(); Cursor.visible = false; Cursor.lockState = CursorLockMode.Locked; break;
                        case "Levels": ChangeMenu(MenuScreen.levels); break; // Change to levels menu
                        case "test": Debug.Log("test"); break;
                    }
                }
            }
            else if (menuScreen == MenuScreen.levels) // Levels menu
            {
                int targetLine = HUDText.GetHoveredLine();
                HUDText.UpdateInteractableText(levelsTexts);

                if (Input.GetMouseButtonUp(0) && targetLine != -1)
                {
                    switch (levelsTexts[targetLine])
                    {
                        case "Back to main menu": ChangeMenu(MenuScreen.main); break; // Change to main menu
                        case "Level 1": StartLevel(1); break;
                        case "Level 2": StartLevel(2); break;
                        case "Level 3": StartLevel(3); break;
                    }
                }

            }
        }
    }

    void Play()
    {
        Debug.Log("Play");
        updateText = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        HUDText.ClearAllText();
        StartCoroutine(Wait());
        IEnumerator Wait()
        {
            yield return new WaitForSecondsRealtime(1);
            GameController.LoadLevel(1);
        }
    }

    void StartLevel(int level)
    {
        updateText = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        HUDText.ClearAllText();
        StartCoroutine(Wait());
        IEnumerator Wait()
        {
            yield return new WaitForSecondsRealtime(1);
            GameController.LoadLevel(level);
        }
    }

    void ChangeMenu(MenuScreen changeTo)
    {
        StartCoroutine(Wait());
        IEnumerator Wait()
        {
            updateText = false;
            int[] lines = new int[textLines.Count];
            for (int i = 0; i < menuTexts.Length; i++) lines[i] = i;
            switch (changeTo) {
                case MenuScreen.main: HUDText.SetText(lines, menuTexts); break;
                case MenuScreen.levels: HUDText.SetText(lines, levelsTexts); break;
            }
            menuScreen = changeTo;
            yield return new WaitForSecondsRealtime(0.1f);
            updateText = true;
        }
    }
}
