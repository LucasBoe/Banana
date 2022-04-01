using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HealingStaffItemData : PlayerItemData
{
    [SerializeField] HealingArea healingAreaPrefab;
    public override void Use(PlayerItemManager manager)
    {
        Instantiate(healingAreaPrefab, manager.PlayerTransform.position, Quaternion.identity);
    }
}
