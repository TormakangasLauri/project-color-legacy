using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    void Start()
    {
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
        HUDText.SetText(14, $"Health: {healthAmount}");
    }

    protected override void OnDeath()
    {
        // jotain tänne idk @mkeoys
    }

}
