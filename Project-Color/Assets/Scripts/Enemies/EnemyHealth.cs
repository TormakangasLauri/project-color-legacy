using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100;
    float healthAmount;
    public Color damageFlash = new Color(116f / 255f, 18f / 255f, 27f / 255f);
    private Renderer enemyRenderer;
    
    private TextMeshProUGUI healthText;


    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        healthAmount = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        if (healthAmount <= 0)
        {
            Destroy(gameObject);
            OnDeath();
        }

        StartCoroutine(FlashColor());
    }

    IEnumerator FlashColor()
    {
        // Get the material of the renderer
        Material enemyMaterial = enemyRenderer.material;

        // Store the original color
        Color originalColor = enemyMaterial.color;

        // Set the flash color
        enemyMaterial.color = damageFlash;

        // Wait for a short duration
        yield return new WaitForSeconds(0.1f); // Adjust the duration as needed

        // Reset to the original color
        enemyMaterial.color = originalColor;
    }


    public void Healing(float healPoints)
    {
        healthAmount += healPoints;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);
    }

    public void OnDeath()
    {
        // Death
    }
}
