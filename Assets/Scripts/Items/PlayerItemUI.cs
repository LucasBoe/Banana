using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemUI : MonoBehaviour
{
    [SerializeField] Image backgroundImage, itemIconImage;

    public void Init(PlayerItemData data)
    {
        backgroundImage.color = data.BackgroundColor;
        itemIconImage.sprite = data.Icon;
    }

    internal void SetSelected(bool selected)
    {
        itemIconImage.transform.localScale = Vector3.one * (selected ? 1.5f : 1f);
    }
}
