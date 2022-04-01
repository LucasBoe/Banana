using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] PlayerItemData loot;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.enabled = true;
        if (loot != null)
            PlayerItemManager.Instance.AddItem(loot);

        loot = null;
    }
}
