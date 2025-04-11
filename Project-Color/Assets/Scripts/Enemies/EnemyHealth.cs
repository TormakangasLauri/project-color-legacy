using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyHealth : Health
{
    public float KBmult = 1;
    public Color damageFlash = new Color(116f / 255f, 18f / 255f, 27f / 255f);
    private Renderer enemyRenderer;
    Rigidbody rb;

    [SerializeField] private FloatingHealthBar healthBar;

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        healthAmount = maxHealth;
        healthBar = GetComponentInChildren<FloatingHealthBar>();   // kommentoi tämä rivi jos health barit halutaan pois
    }

    protected override void OnDamaged()
    {
        if (healthBar != null) healthBar.UpdateHealthBar(healthAmount, maxHealth);
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

    public void Knockback(Vector3 kb, ForceMode fm)
    {
        rb.AddForce(kb * KBmult, fm);
    }

    protected override void OnHealed()
    {
        
    }

    protected override void OnDeath()
    {
        gameObject.GetComponent<EnemyType>().Deactivate();
        PlayerAttack.inst.enemies.Remove(gameObject);
        healthAmount = maxHealth;
    }
}
