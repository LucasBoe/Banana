using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : SingletonBehaviour<RoomManager>
{
    private List<Room> rooms = new List<Room>();
    public void RegisterRoom(Room room)
    {
        rooms.Add(room);
    }

    public Portal GetPortalThatLeadsTo(Room from, Room to)
    {
        RecursiveSearch search = new RecursiveSearch(from, to);
        Debug.LogWarning("found: " ,search.Result);
        return search.Result;
    }
    private class RecursiveSearch
    {
        public Portal Result { get; private set; }

        private Queue<Room> toCheck = new Queue<Room>();
        private List<Room> visited = new List<Room>();

        public RecursiveSearch(Room from, Room to)
        {
            toCheck.Enqueue(to);

            while (toCheck.Count > 0)
            {
                Room room = toCheck.Dequeue();
                visited.Add(room);

                foreach (Portal portal in room.Portals)
                {
                    Room target = portal.Target.Room;

                    if (!visited.Contains(target))
                    {
                        Debug.Log("Checked room " + target.name);

                        if (target == from)
                        {
                            Result = portal.Target;
                            return;
                        }
                        else
                        {
                            toCheck.Enqueue(target);
                        }
                    }
                }
            }
        }

    }
}