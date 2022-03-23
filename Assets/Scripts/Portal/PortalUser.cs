using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalUser : MonoBehaviour
{
    public System.Action<Room, Room> ChangeRoom;
    public System.Action TeleportFinished;

    [SerializeField] SkinnedMeshRenderer meshRenderer;

    PortalTeleportation active;

    public bool IsTeleporting => active != null;

    public void Teleport(Portal from, Portal to)
    {
        ChangeRoom?.Invoke(from.Room, to.Room);
        transform.position = to.TeleportPosition.position;
        TeleportFinished?.Invoke();
    }

    public PortalTeleportation StartTeleportation(Portal from, Portal to)
    {
        Material mat = meshRenderer.material;
        active = new PortalTeleportation(this, from, to, mat);
        active.OnExit += OnExit;
        return active;
    }

    private void OnExit()
    {
        active.OnExit -= OnExit;
        active = null;
    }

    private void FixedUpdate()
    {
        if (active == null) return;
        active.Update();
    }
}
