using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Image healthBar;
    public float healthAmount = 100;
    public Color damageFlash = new Color(116f / 255f, 18f / 255f, 27f / 255f);
    private Renderer enemyRenderer;
    
    private TextMeshProUGUI healthText;


    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
    }
    void Update()
    {
        if (healthAmount <= 0)
        {
            GameOver();
        }

        // Placeholder damagen aiheuttaja (Paina E, tekee X vahinkoa)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(20);
        }

        // Placeholder elinvoiman palauttaja (Paina T, parantaa X vahinkoa)
        if (Input.GetKeyDown(KeyCode.T))
        {
            Healing(10);
        }
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        if (gameObject.CompareTag("Player"))
        {
            healthBar.fillAmount = healthAmount / 100;
        }
        if (healthAmount <= 0)
        {
            Destroy(gameObject);
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
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);
        
        if (gameObject.CompareTag("Player"))
        {
            healthBar.fillAmount = healthAmount / 100;
        }
    }

    public void GameOver()
    {
        // jotain tänne idk @mkeoys
    }
}
