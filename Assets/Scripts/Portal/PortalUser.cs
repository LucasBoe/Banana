using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalUser : MonoBehaviour
{
    public System.Action<Room, Room> ChangeRoom;
    public System.Action<Transform, Vector3> TeleportingPlayer;
    public System.Action TeleportFinished;

    [SerializeField] MaterialInstantiator MaterialInstantiator;

    PortalTeleportation active;

    public bool IsTeleporting => active != null;

    public void Teleport(Portal from, Portal to)
    {
        Vector2 target = from.TransformPointToTarget(transform.position);
        Vector2 before = transform.position;

        transform.position = from.TransformPointToTarget(transform.position);
        TeleportingPlayer?.Invoke(transform, target - before);

        ChangeRoom?.Invoke(from.Room, to.Room);
    }

    public PortalTeleportation StartTeleportation(Portal from, Portal to)
    {
        active = new PortalTeleportation(this, from, to, MaterialInstantiator);
        active.OnExit += OnExit;
        return active;
    }

    private void OnExit()
    {
        active.OnExit -= OnExit;
        TeleportFinished?.Invoke();
        active = null;
    }

    private void FixedUpdate()
    {
        if (active == null) return;
        active.Update();
    }
}
