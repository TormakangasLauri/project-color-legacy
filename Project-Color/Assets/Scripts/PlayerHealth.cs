using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    public GameObject deathMenu;
    private DeathMenu DeathMenu;

    void Start()
    {
        DeathMenu = deathMenu.GetComponentInChildren<DeathMenu>();
        deathMenu.SetActive(false);
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
            Time.timeScale = 0;
            deathMenu.SetActive(true);
            HUDText.ClearAllImmediate();
            yield return new WaitForSecondsRealtime(1);
            HUDText.SetText(2, DeathMenu.texts[2]);
            yield return new WaitForSecondsRealtime(1);
            HUDText.SetText(3, DeathMenu.texts[3]);
            yield return new WaitForSecondsRealtime(1);

            HUDText.SetInteractableLines(new[] { 2, 3 }, true);
            int[] lines = new int[HUDText.textLines.Count];
            for (int i = 0; i < HUDText.textLines.Count; i++) lines[i] = i;
            HUDText.SetText(lines, DeathMenu.texts);
            bool c = false;
            yield return new WaitUntil(() =>
            {
                HUDText.UpdateInteractableText(DeathMenu.texts);
                int targetLine = HUDText.GetHoveredLine();
                if (Input.GetMouseButtonUp(0) && targetLine != -1)
                {
                    switch (DeathMenu.texts[targetLine])
                    {
                        case "Continue": c = true; break;
                        case "Main_menu":
                        {
                            Time.timeScale = 1;
                            GameController.StartLevel(0);
                            break;
                        }
                    }
                }
                return c;
            });

            // Continue the game
            Time.timeScale = 1;
            deathMenu.SetActive(false);
            HUDText.RetrieveAllText();
            Heal(100);
        }
    }

}
