using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaRegeneratingCollectable : MonoBehaviour
{
    [SerializeField] int manaAmount = 80;
    [SerializeField] Collider2D collider;
    [SerializeField] FlyToPlayer flyToPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.IsPlayer()) return;
        if (ManaManager.Instance.IsFull) return;

        collider.enabled = false;
        flyToPlayer.Fly(collision.transform, () =>
        {
            ManaManager.Instance.AddMana(manaAmount);
            Destroy(gameObject);
        });
    }
}
