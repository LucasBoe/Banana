using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public Room Room;
    private PortalUser portalUser;

    private void OnEnable()
    {
        portalUser = GetComponent<PortalUser>();

        if (portalUser != null)
            portalUser.ChangeRoom += OnChangeRoom;
    }

    private void OnDisable()
    {
        if (portalUser != null)
            portalUser.ChangeRoom -= OnChangeRoom;
    }
    private void OnChangeRoom(Room from, Room to)
    {
        Debug.Log("OnChangeRoom is: " + from + " to " + to);

        Room = to;

        from.UnregisterInfo(this);
        to.RegisterInfo(this);
    }
}
