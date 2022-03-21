using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;

    Material material;
    Health health;
    private void Awake()
    {
        material = new Material(meshRenderer.material);
        meshRenderer.material = material;
        health = GetComponentInParent<Health>();

        if (health != null)
            OnChangedHealth(1);
    }

    private void OnEnable()
    {
        if (health != null)
            health.ChangedHealth += OnChangedHealth;
    }

    private void OnDisable()
    {
        if (health != null)
            health.ChangedHealth -= OnChangedHealth;
    }
    private void OnChangedHealth(float relative)
    {
        material.SetFloat("health", relative);
    }
}
