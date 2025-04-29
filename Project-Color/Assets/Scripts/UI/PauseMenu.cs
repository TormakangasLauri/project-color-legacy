using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private TextMeshProUGUI menuText;
    private bool menuOpen = false;

    [TextArea] public string textContent;
    public float textAppearSpeed = 1;

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

        HUDText.SaveAllText();
        HUDText.ClearAllText();
        HUDText.SetText(1, "Pause");
    }

    private void ClosePauseMenu()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
        GameController.paused = false;

        HUDText.RetreiveAllText();
    }
}
