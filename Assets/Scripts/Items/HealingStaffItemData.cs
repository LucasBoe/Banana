using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HealingStaffItemData : PlayerItemData
{
    [SerializeField] HealingArea healingAreaPrefab;
    public override void Use(PlayerItemManager manager)
    {
        ManaManager.Instance.RemoveMana(25f);
        Instantiate(healingAreaPrefab, manager.PlayerTransform.position, Quaternion.identity);
    }
}
