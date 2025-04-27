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

        StartCoroutine(WriteText());
    }

    private void ClosePauseMenu()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
        GameController.paused = false;

        menuText.text = "";
    }

    IEnumerator WriteText()
    {
        float timeFor1Char = textAppearSpeed / textContent.Length;
        float elapsedTime = 0;

        yield return new WaitUntil(() =>
        {
            elapsedTime += Time.unscaledDeltaTime;
            // Get a substring from textContent with the amount of characters calculated from the time it should take for one char to appear and time elapsed in the process
            menuText.text = textContent[..Mathf.Clamp((int)(elapsedTime/timeFor1Char), 0, textContent.Length)];
            return !GameController.paused || menuText.text.Length == textContent.Length;
        });
    }
}
