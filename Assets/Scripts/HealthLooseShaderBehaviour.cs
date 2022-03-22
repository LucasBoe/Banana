using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthLooseShaderBehaviour : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    [SerializeField] AnimationCurve takeDamageCurve;
    private Material material;

    private void Awake()
    {
        material = new Material(meshRenderer.material);
        meshRenderer.material = material;
    }

    private void OnEnable()
    {
        health.ChangedHealth += OnChangedHealth;
    }
    private void OnDisable()
    {
        health.ChangedHealth -= OnChangedHealth;
    }
    private void OnChangedHealth(float health)
    {


        StopAllCoroutines();
        StartCoroutine(TakeDamageRoutine(takeDamageCurve, material));
    }

    IEnumerator TakeDamageRoutine(AnimationCurve curve, Material material)
    {
        PositionInterpolation interpolation = new PositionInterpolation(curve);
        while (!interpolation.Done)
        {
            material.SetFloat("damage", interpolation.Evaluate());
            yield return null;
        }
    }
}

public class PositionInterpolation
{
    private float startTime;
    private AnimationCurve curve;
    private float duration;
    public bool Done = false;

    public PositionInterpolation(AnimationCurve curve)
    {
        this.curve = curve;
        startTime = Time.time;
        duration = curve.keys[curve.length - 1].time;
    }

    public float Evaluate()
    {
        if (Done)
            return curve.Evaluate(duration);

        float time = Time.time - startTime;
        Done = time >= duration;

        return curve.Evaluate(time / duration);
    }
}
