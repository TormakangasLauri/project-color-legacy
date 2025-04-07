using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float maxHealth = 100;
    float healthAmount;
    
    public TextMeshProUGUI healthText;

    void Start()
    {
        healthAmount = maxHealth;

        UpdateHealthUI();
    }

    void Update()
    {
        // Placeholder damagen aiheuttaja (Paina F, tekee X vahinkoa)
        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(20);
        }

        // Placeholder elinvoiman palauttaja (Paina G, parantaa X vahinkoa)
        if (Input.GetKeyDown(KeyCode.G))
        {
            Healing(10);
        }
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        UpdateHealthUI();

        if (healthAmount <= 0) GameOver(); // Death
    }

    public void Healing(float healPoints)
    {
        healthAmount += healPoints;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        healthText.text = "Health: " + healthAmount;
    }

    public void GameOver()
    {
        // jotain tänne idk @mkeoys
    }
}
