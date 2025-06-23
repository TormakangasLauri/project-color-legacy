using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100;
    protected float healthAmount;

    public Rigidbody rb;

    void Awake()
    {
        healthAmount = maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Damage(float damage, Vector3 knockback = default)
    {
        healthAmount -= damage;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);

        if (knockback != default) Knockback(knockback);

        if (healthAmount <= 0) OnDeath();
        else OnDamaged();
    }

    public virtual void Heal(float healPoints)
    {
        healthAmount += healPoints;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);
        OnHealed();
    }

    protected virtual void Knockback(Vector3 kb)
    {
        if (rb != null) rb.AddForce(kb, ForceMode.Impulse);
    }

    protected virtual void OnDamaged() { }
    protected virtual void OnHealed() { }
    protected virtual void OnDeath() { }
}
