using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : SingletonBehaviour<RoomManager>
{
    private List<Room> rooms = new List<Room>();
    internal void RegisterRoom(Room room)
    {
        rooms.Add(room);
    }
}
