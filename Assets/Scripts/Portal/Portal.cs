using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, IPathTarget
{
    [SerializeField] public Transform TeleportPosition;
    [SerializeField] public Portal Target;
    [SerializeField] RoomInfo roomInfo;

    public bool Active = true;
    public System.Action Teleported;
    public Room Room => roomInfo.Room;

    public Transform TargetTransform => TeleportPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Active || Target == null) return;

        PortalUser user = collision.GetComponent<PortalUser>();

        if (user == null) return;

        Teleport(user);
    }

    private void Teleport(PortalUser user)
    {
        Target.Active = false;
        user.Teleport(this, Target);
        Target.Teleported?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PortalUser>() == null) return;

        Active = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (Target == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.back * 0.5f, Target.transform.position + Vector3.back * 0.5f);
    }
}
