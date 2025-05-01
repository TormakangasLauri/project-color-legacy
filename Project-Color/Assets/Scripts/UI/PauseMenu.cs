using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    public float textAppearSpeed = 1;

    private bool menuOpen = false;

    private void Start()
    {
        menu.SetActive(false); // Close pause menu at the start
    }

    public void TogglePauseMenu(InputAction.CallbackContext action)
    {
        if (action.canceled)
        {
            if (menuOpen)
            {
                menuOpen = false;
                ClosePauseMenu();
            }
            else
            {
                menuOpen = true;
                OpenPauseMenu();
            }
        }
    }

    private void OpenPauseMenu()
    {
        menu.SetActive(true);
        Time.timeScale = 0;
        GameController.paused = true;

        HUDText.SaveAllText();
        HUDText.SetText(new[]{0,5}, new[]{"Paused"}, HUDTextReplace.Clear, HUDTextFill.Fill);
    }

    private void ClosePauseMenu()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
        GameController.paused = false;

        HUDText.RetrieveAllText();
    }
}
