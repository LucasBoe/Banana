using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, IPathTarget
{
    [SerializeField] public Transform TeleportPosition;
    [SerializeField] Portal target;
    [SerializeField] RoomInfo roomInfo;

    public bool Active = true;
    public System.Action Teleported;
    public Room Room => roomInfo.Room;

    public Transform TargetTransform => TeleportPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Active || target == null) return;

        PortalUser user = collision.GetComponent<PortalUser>();

        if (user == null) return;

        Teleport(user);
    }

    private void Teleport(PortalUser user)
    {
        target.Active = false;
        user.Teleport(this, target);
        target.Teleported?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PortalUser>() == null) return;

        Active = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.back * 0.5f, target.transform.position + Vector3.back * 0.5f);
    }
}
