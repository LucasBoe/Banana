using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] Portal target;

    public bool Active = true;
    public System.Action Teleported;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Active || !collision.IsPlayer() || target == null) return;

        Teleport(collision.gameObject,target);
    }

    private void Teleport(GameObject player, Portal target)
    {
        target.Active = false;
        player.transform.position = target.transform.position;
        target.TeleportTo();
    }

    private void TeleportTo()
    {
        Teleported?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.IsPlayer()) return;

        Active = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.back * 0.5f, target.transform.position + Vector3.back * 0.5f);
    }
}
