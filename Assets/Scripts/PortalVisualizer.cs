using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalVisualizer : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] Color color;
    [SerializeField] SpriteRenderer[] spriteRenderers;
    [SerializeField] Portal portal;
    [SerializeField] ParticleSystem particleSystem;

    private void Awake()
    {
        material = new Material(material);
        color = material.GetColor("_BaseColor");
        foreach (SpriteRenderer renderer in spriteRenderers)
            renderer.material = material;
    }

    private void OnEnable()
    {
        portal.Teleported += OnTeleported;
    }

    private void OnDisable()
    {
        portal.Teleported -= OnTeleported;
    }
    private void OnTeleported()
    {
        StopAllCoroutines();
        StartCoroutine(TeleportEffectRoutine());
    }

    private IEnumerator TeleportEffectRoutine()
    {
        float t = 0;
        Color c = new Vector4(2f, 2f, 2f, 1.0f);
        material.SetColor("_EmissionColor", c);
        yield return new WaitForSeconds(0.1f);
        particleSystem.Emit(2);
        yield return new WaitForSeconds(0.1f);
        particleSystem.Emit(2);
        yield return new WaitForSeconds(0.1f);
        particleSystem.Emit(2);
        yield return new WaitForSeconds(0.1f);
        particleSystem.Emit(2);
        yield return new WaitForSeconds(0.1f);
        particleSystem.Emit(2);
        yield return new WaitForSeconds(0.1f);
        particleSystem.Emit(2);

        material.SetColor("_EmissionColor", Color.black);
    }

    private void Update()
    {
        color.a = 0.75f + Mathf.Sin(Time.time * Mathf.PI) * 0.25f;
        material.SetColor("_BaseColor", color);
    }
}
