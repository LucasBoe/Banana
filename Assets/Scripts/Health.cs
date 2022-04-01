using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    float health;
    [SerializeField] float maxHealth;
    [SerializeField] string damageTag;

    public Action<float> ChangedHealth;
    public Action Die;

    private void Awake()
    {
        health = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckForDamage(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckForDamage(collision.gameObject);
    }

    private void CheckForDamage(GameObject go)
    {
        if (go.CompareTag(damageTag))
        {
            IDamager damager = go.GetComponent<IDamager>();

            if (damager.IsEnabled)
            {
                DoDamage(damager.Amount);
                damager.Disable();
            }
        }
    }

    private void DoDamage(float amount)
    {
        health -= amount;
        ChangedHealth?.Invoke(health / maxHealth);

        if (health < 0)
            Die?.Invoke();
    }

    public void Heal(float amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        ChangedHealth?.Invoke(health / maxHealth);
    }
}

internal interface IDamager
{
    bool IsEnabled { get; }
    float Amount { get; }
    void Disable();
}