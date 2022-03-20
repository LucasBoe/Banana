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
        if (!Active || !(IsTeleportable(collision)) || target == null) return;

        Teleport(collision.gameObject, target);
    }

    private static bool IsTeleportable(Collider2D collision)
    {
        return collision.IsPlayer() || collision.IsHelper();
    }

    private void Teleport(GameObject toTeleport, Portal target)
    {
        target.Active = false;
        toTeleport.transform.position = target.transform.position;
        target.TeleportTo();
    }

    private void TeleportTo()
    {
        Teleported?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsTeleportable(collision)) return;

        Active = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.back * 0.5f, target.transform.position + Vector3.back * 0.5f);
    }
}
