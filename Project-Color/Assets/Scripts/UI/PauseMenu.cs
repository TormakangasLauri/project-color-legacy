using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    private bool menuOpen = false;

    private void Start()
    {
        menu.SetActive(false); // Close pause menu at the start
    }

    public void TogglePauseMenu()
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

    private void OpenPauseMenu()
    {
        menu.SetActive(true);
        Time.timeScale = 0;
        GameController.paused = true;
    }

    private void ClosePauseMenu()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
        GameController.paused = false;
    }
}
