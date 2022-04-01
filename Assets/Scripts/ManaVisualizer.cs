using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaVisualizer : MonoBehaviour
{
    [SerializeField] Material bottleMat;
    [SerializeField] ParticleSystem remove;
    ParticleSystem.EmissionModule removeEmit;

    [SerializeField] ParticleSystem add;
    ParticleSystem.EmissionModule addEmit;

    [SerializeField] float manaAmount = 1;
    [SerializeField, Range(0, 1)] float manaAmountDelayed = 0;
    [SerializeField, Range(0, 1)] float manaAmountDifferenceDelayed = 0;

    private void Awake()
    {
        removeEmit = remove.emission;
        addEmit = add.emission;
    }

    private void Update()
    {
        manaAmountDelayed = Mathf.MoveTowards(manaAmountDelayed, manaAmount, Time.deltaTime / 3f);
        manaAmountDifferenceDelayed = Mathf.MoveTowards(manaAmountDifferenceDelayed, manaAmount, Time.deltaTime * 3f);

        float difference = Mathf.Abs(manaAmountDelayed - manaAmountDifferenceDelayed) * 500f;

        bool increasing = manaAmount > manaAmountDelayed;

        removeEmit.rateOverTime = new ParticleSystem.MinMaxCurve(increasing ? 0 : difference);
        addEmit.rateOverTime = new ParticleSystem.MinMaxCurve(increasing ? difference : 0);

        bottleMat.SetFloat("fill_value", manaAmountDelayed);
        bottleMat.SetFloat("fill_highlight_value", manaAmountDifferenceDelayed);
    }

    private void OnEnable()
    {
        ManaManager.RemovedMana += OnChangedMana;
    }
    private void OnDisable()
    {
        ManaManager.RemovedMana -= OnChangedMana;
    }

    private void OnChangedMana(float before, float after)
    {
        manaAmount = after;
    }
}
