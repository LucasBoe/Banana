using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemUIManager : MonoBehaviour
{
    [SerializeField] PlayerItemUI itemUI;
    Dictionary<PlayerItemData, PlayerItemUI> instances = new Dictionary<PlayerItemData, PlayerItemUI>();


    private void OnEnable()
    {
        PlayerItemManager.GetNewItem += OnGetNewItem;
        PlayerItemManager.SelectedItem += OnSelectedItem;
    }

    private void OnDisable()
    {
        PlayerItemManager.GetNewItem -= OnGetNewItem;
        PlayerItemManager.SelectedItem -= OnSelectedItem;
    }
    private void OnGetNewItem(PlayerItemData data)
    {
        PlayerItemUI instance = Instantiate(itemUI, transform);
        instance.Init(data);
        instance.gameObject.SetActive(true);
        instances.Add(data,instance);
    }
    private void OnSelectedItem(PlayerItemData item)
    {
        foreach (KeyValuePair<PlayerItemData, PlayerItemUI> instance in instances)
        {
            instance.Value.SetSelected(instance.Key == item);
        }
    }
}
