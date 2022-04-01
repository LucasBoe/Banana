using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegenerator : MonoBehaviour
{
    [SerializeField] Health health;
    internal void Regenerate(float amount)
    {
        health.Heal(amount);
    }
}
