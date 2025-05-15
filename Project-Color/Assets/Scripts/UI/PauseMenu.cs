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

    private void Start()
    {
        menu.SetActive(false); // Close pause menu at the start
    }

    private void Update()
    {
        timer -= Time.unscaledDeltaTime;
    }

    public void TogglePauseMenu(InputAction.CallbackContext action)
    {
        if (action.canceled)
        {
            if (menuOpen && timer < 0)
            {
                menuOpen = false;
                ClosePauseMenu();
            }
            else if (!menuOpen)
            {
                menuOpen = true;
                timer = menuCooldown;
                OpenPauseMenu();
            }
        }
    }

    private void OpenPauseMenu()
    {
        menu.SetActive(true);
        Time.timeScale = 0;
        GameController.paused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        HUDText.SaveAllText();
        HUDText.SetText(new[]{0,5}, new[]{"Paused"}, HUDTextReplace.Clear, HUDTextFill.Fill);
    }

    private void ClosePauseMenu()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
        GameController.paused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        HUDText.RetrieveAllText();
    }
}
