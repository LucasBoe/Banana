using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerItemManager : SingletonBehaviour<PlayerItemManager>
{
    [SerializeField] public Transform PlayerTransform;
    [SerializeField] List<PlayerItemData> owned;
    [SerializeField] PlayerItemData selected;
    public static System.Action<PlayerItemData> GetNewItem;
    public static System.Action<PlayerItemData> SelectedItem;

    internal void AddItem(PlayerItemData loot)
    {
        owned.Add(loot);
        GetNewItem?.Invoke(loot);
        SelectItem(owned.Last());
    }

    private void SelectItem(PlayerItemData playerItemData)
    {
        selected = playerItemData;
        transform.DestroyAllChildren();
        Instantiate(playerItemData.VisulizationPrefab, transform);
        SelectedItem?.Invoke(playerItemData);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
            selected?.Use(this);
    }
}
