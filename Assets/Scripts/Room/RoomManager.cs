using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomManager : SingletonBehaviour<RoomManager>
{
    [SerializeField] List<Room> roomTemplates = new List<Room>();

    private List<Room> rooms = new List<Room>();
    public void RegisterRoom(Room room)
    {
        rooms.Add(room);
    }
    public Portal GetPortalThatLeadsTo(Room from, Room to)
    {
        RecursiveSearch search = new RecursiveSearch(from, to);
        Debug.LogWarning("found: ", search.Result);
        return search.Result;
    }
    private class RecursiveSearch
    {
        public Portal Result { get; private set; }

        private Queue<Room> rommsToVisit = new Queue<Room>();
        private List<Room> roomsVisited = new List<Room>();

        public RecursiveSearch(Room from, Room to)
        {
            rommsToVisit.Enqueue(to);

            while (rommsToVisit.Count > 0)
            {
                Room room = rommsToVisit.Dequeue();
                roomsVisited.Add(room);

                foreach (Portal portal in room.Portals)
                {
                    //not all portals are connected
                    if (portal.Target != null)
                    {
                        Room subRoom = portal.Target.Room;

                        if (subRoom != null && !roomsVisited.Contains(subRoom))
                        {
                            Debug.Log("Checked room " + subRoom.name);

                            if (subRoom == from)
                            {
                                Result = portal.Target;
                                return;
                            }
                            else
                            {
                                rommsToVisit.Enqueue(subRoom);
                            }
                        }
                    }
                }
            }
        }

    }

    internal Portal GetNewRoomsPortal(Portal inPortal)
    {
        Portal outPortal = null;

        int[] rotations = new int[] { 0, 90, 180, 270 };

        Room newRoom = Instantiate(roomTemplates[Random.Range(0, roomTemplates.Count)], (20 * rooms.Count) * Vector3.right, Quaternion.Euler(0, 0, rotations[Random.Range(0, rotations.Length)]));
        newRoom.GetComponent<RoomMeshCreator>().UpdateMesh(newRoom.TileData);
        outPortal = newRoom.Portals[Random.Range(0, newRoom.Portals.Count)];
        outPortal.Target = inPortal;
        return outPortal;
    }
    public Room[] Rooms => rooms.ToArray();
}