using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerItemData : ScriptableObject
{
    [SerializeField] public GameObject VisulizationPrefab;
    [SerializeField] public Sprite Icon;
    [SerializeField] public Color BackgroundColor;

    public virtual void Use(PlayerItemManager manager)
    {

    }
}
