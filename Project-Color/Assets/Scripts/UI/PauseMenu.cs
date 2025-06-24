using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject keybindInfo;
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
        if (TimeController.paused)
        {
            HUDText.UpdateInteractableText(texts);

            int targetLine = HUDText.GetHoveredLine();
            
            if (Input.GetMouseButtonUp(0) && targetLine != -1)
            {
                switch (texts[targetLine])
                {
                    case "Resume": ClosePauseMenu(); break;
                    case "Main_menu": GameController.LoadLevel(0); break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.M)) GameController.LoadLevel(0);
        if (Input.GetKeyDown(KeyCode.H)) keybindInfo.SetActive(!keybindInfo.activeSelf);
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

    public void OpenPauseMenu()
    {
        menuOpen = true;
        menu.SetActive(true);

        TimeController.Pause();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        HUDText.SaveAllText();
        HUDText.SetInteractableLines(new[]{0}, true); // Exclude first line
        HUDText.SetText(lines, texts, HUDTextReplace.Clear);
    }

    public void ClosePauseMenu(bool retrieveText = true)
    {
        menuOpen = false;
        menu.SetActive(false);

        TimeController.Unpause();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (retrieveText) HUDText.RetrieveAllText();
    }
}
