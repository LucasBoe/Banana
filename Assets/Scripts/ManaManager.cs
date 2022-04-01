using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManager : SingletonBehaviour<ManaManager>
{
    [SerializeField] float manaMax;
    float manaCurrent;

    private void Start()
    {
        manaCurrent = manaMax;
    }

    public static System.Action<float, float> RemovedMana;

    public bool IsFull => manaCurrent == manaMax;
    public void AddMana(int change)
    {
        float before = manaCurrent / manaMax;
        manaCurrent = Mathf.Clamp(manaCurrent + change, 0, manaMax * 1.1f);
        float after = manaCurrent / manaMax;

        RemovedMana?.Invoke(before, after);
    }

    public void RemoveMana(float change)
    {
        float before = manaCurrent / manaMax;
        manaCurrent = Mathf.Clamp(manaCurrent - change, 0, manaMax * 1.1f);
        float after = manaCurrent / manaMax;

        RemovedMana?.Invoke(before,after);
    }
}
