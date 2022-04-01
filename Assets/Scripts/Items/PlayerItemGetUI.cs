using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemGetUI : MonoBehaviour
{
    [SerializeField] GameObject ui;
    [SerializeField] Image itemImage;

    private void OnEnable()
    {
        PlayerItemManager.GetNewItem += OnGetNewItem;
    }

    private void OnDisable()
    {
        PlayerItemManager.GetNewItem -= OnGetNewItem;
    }

    private void OnGetNewItem(PlayerItemData data)
    {
        itemImage.sprite = data.Icon;
        ui.SetActive(false);
        ui.SetActive(true);
    }
}
