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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(damageTag))
        {
            IDamager damager = collision.gameObject.GetComponent<IDamager>();

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
}

internal interface IDamager
{
    bool IsEnabled { get; }
    float Amount { get; }
    void Disable();
}