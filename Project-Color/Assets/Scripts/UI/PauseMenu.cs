using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    public float menuCooldown = 0.2f;
    private float timer = 0;
    private bool menuOpen = false;

    private int[] lines = new int[15];
    public string[] texts = new string[15];

    private void Awake()
    {
        for (int i = 0; i < lines.Length; i++)
            lines[i] = i;
    }

    private void Start()
    {
        ClosePauseMenu(false); // Close pause menu at the start
    }

    private void Update()
    {
        timer -= Time.unscaledDeltaTime;
        if (GameController.paused)
        {
            HUDText.UpdateInteractableText(texts);

            int targetLine = HUDText.GetHoveredLine();
            
            if (Input.GetMouseButtonUp(0) && targetLine != -1)
            {
                switch (texts[targetLine])
                {
                    case "Resume": ClosePauseMenu(); break;
                    case "Main_menu":
                    {
                        //ClosePauseMenu();
                        //GameController.StartLevel(0);
                        GameController.LoadLevel(0);
                        break;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.M)) GameController.LoadLevel(0);
    }

    public void TogglePauseMenu(InputAction.CallbackContext action)
    {
        if (action.canceled)
        {
            if (menuOpen && timer < 0)
            {
                ClosePauseMenu();
            }
            else if (!menuOpen)
            {
                timer = menuCooldown;
                OpenPauseMenu();
            }
        }
    }

    private void OpenPauseMenu()
    {
        menuOpen = true;
        menu.SetActive(true);
        Time.timeScale = 0;
        GameController.paused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        HUDText.SaveAllText();
        HUDText.SetInteractableLines(new[]{0}, true); // Exclude first line
        HUDText.SetText(lines, texts, HUDTextReplace.Clear);
    }

    private void ClosePauseMenu(bool retrieveText = true)
    {
        menuOpen = false;
        menu.SetActive(false);
        Time.timeScale = 1;
        GameController.paused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (retrieveText) HUDText.RetrieveAllText();
    }
}
