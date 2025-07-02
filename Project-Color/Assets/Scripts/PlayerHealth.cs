using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : Health
{
    public GameObject deathMenuObject;
    public DeathMenu deathMenu;

    void Start()
    {
        deathMenuObject.SetActive(false);
        StartCoroutine(Wait());
        IEnumerator Wait()
        {
            yield return new WaitForFixedUpdate();
            UpdateHealthUI();
        }
    }

    void Update()
    {
        // Placeholder damagen aiheuttaja (Paina F, tekee X vahinkoa)
        if (Input.GetKeyDown(KeyCode.F))
        {
            Damage(20);
        }

        // Placeholder elinvoiman palauttaja (Paina G, parantaa X vahinkoa)
        if (Input.GetKeyDown(KeyCode.G))
        {
            Heal(10);
        }
    }

    protected override void OnDamaged()
    {
        UpdateHealthUI();
    }

    protected override void OnHealed()
    {
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        //healthText.text = "Health: " + healthAmount;
        HUDText.SetText(-1, $"Health: {healthAmount}");
    }

    protected override void OnDeath()
    {
        HUDText.SaveAllText();
        StartCoroutine(Death());
        IEnumerator Death()
        {
            TimeController.Pause();
            deathMenuObject.SetActive(true);
            HUDText.ClearAllImmediate();
            yield return new WaitForSecondsRealtime(1); // Set texts
            HUDText.SetText(2, deathMenu.texts[2]);
            yield return new WaitForSecondsRealtime(1);
            HUDText.SetText(3, deathMenu.texts[3]);
            yield return new WaitForSecondsRealtime(1);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            HUDText.SetInteractableLines(new[] { 2, 3 }, true); // Setup interactable lines
            int[] lines = new int[HUDText.textLines.Count];
            for (int i = 0; i < HUDText.textLines.Count; i++) lines[i] = i;
            HUDText.SetText(lines, deathMenu.texts);
            bool c = false;
            yield return new WaitUntil(() => // Wait for player input
            {
                HUDText.UpdateInteractableText(deathMenu.texts);
                int targetLine = HUDText.GetHoveredLine();
                if (Input.GetMouseButtonUp(0) && targetLine != -1)
                {
                    switch (deathMenu.texts[targetLine])
                    {
                        case "Restart": Debug.Log("Restart"); c = true; break;
                        case "Main_menu":
                        {
                            Debug.Log("Main menu");
                            GameController.levelLoader.Load(0);
                            break;
                        }
                    }
                }
                return c;
            });

            // Restart the level
            GameController.LoadLevel(SceneManager.GetActiveScene().buildIndex); // Reload the level

            // Continue the game
            //Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked;
            //TimeController.Unpause();
            //deathMenuObject.SetActive(false);
            //HUDText.RetrieveAllText();
            //Heal(100);
        }
    }

}
