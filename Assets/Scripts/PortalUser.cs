using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalUser : MonoBehaviour
{
    public System.Action<Room, Room> ChangeRoom;
    public System.Action<Room> TeleportFinished;

    public void Teleport(Portal from, Portal to)
    {
        ChangeRoom?.Invoke(from.Room, to.Room);
        transform.position = to.TeleportPosition.position;
        TeleportFinished?.Invoke(to.Room);
    }
}
