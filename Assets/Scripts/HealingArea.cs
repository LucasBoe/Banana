using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingArea : MonoBehaviour
{
    [SerializeField] float healAmount = 10f;
    List<HealthRegenerator> regenerators = new List<HealthRegenerator>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthRegenerator regenerator = collision.GetComponent<HealthRegenerator>();
        if (regenerators != null) regenerators.Add(regenerator);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        HealthRegenerator regenerator = collision.GetComponent<HealthRegenerator>();
        if (regenerators != null) regenerators.Remove(regenerator);
    }
    private void Update()
    {
        if (regenerators == null || regenerators.Count == 0) return;

        foreach (HealthRegenerator regenerator in regenerators)
        {
            if (regenerator != null)
                regenerator.Regenerate(Time.deltaTime * healAmount);
        }
    }

    void Start()
    {
        StartCoroutine(HealingAreaRoutine());
    }

    IEnumerator HealingAreaRoutine()
    {
        Light light = GetComponentInChildren<Light>();

        float t = 0;
        transform.localScale = Vector3.zero;

        while (t < 1)
        {
            t += Time.deltaTime;
            UpdateVisualsByT(light, t);
            yield return null;
        }

        yield return new WaitForSeconds(4);

        foreach (ParticleSystem system in GetComponentsInChildren<ParticleSystem>())
        {
            var emit = system.emission;
            emit.rateOverTime = 0;
        }

        while (t > 0)
        {
            t -= Time.deltaTime * 0.125f;
            UpdateVisualsByT(light, t);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void UpdateVisualsByT(Light light, float t)
    {
        transform.localScale = Vector3.one * t;
        light.intensity = t * 6 - 3;
    }
}
