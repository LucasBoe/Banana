using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] public TileData TileData;

    public List<Portal> Portals;
    public List<Enemy> Enemys;
    public List<Player> Players;

    private void Awake()
    {
        List<RoomInfo> children = new List<RoomInfo>(GetComponentsInChildren<RoomInfo>());
        foreach (RoomInfo info in children)
        {
            info.Room = this;
            RegisterInfo(info);
        }

        RoomManager.Instance.RegisterRoom(this);
    }
    public void RegisterInfo(RoomInfo info)
    {
        Portals = AddIfMatchesTag(Portals, info, "Portal");
        Enemys = AddIfMatchesTag(Enemys, info, "Enemy");
        Players = AddIfMatchesTag(Players, info, "Player");
    }

    public void UnregisterInfo(RoomInfo info)
    {
        Portals = RemoveIfMatchesTag(Portals, info, "Portal");
        Enemys = RemoveIfMatchesTag(Enemys, info, "Enemy");
        Players = RemoveIfMatchesTag(Players, info, "Player");
    }

    public Vector2Int RemoveOffset(Vector3 point)
    {
        Vector2Int newPoint = new Vector2Int(Mathf.FloorToInt(point.x - transform.position.x), Mathf.FloorToInt((point.y) - transform.position.y));
        return TileData.RemoveOffset(newPoint);
    }
    internal Vector2 AdaptVector2ToRoomPerimeter(Vector2 origin, Vector2 target)
    {
        if (IsInside(target))
            return target;
        else if (IsInside(new Vector2(target.x, origin.y)))
            return new Vector2(target.x, origin.y);
        else if (IsInside(new Vector2(origin.x, target.y)))
            return new Vector2(origin.x, target.y);

        return origin;
    }

    public bool IsInside(Vector2 pos)
    {
        return TileData.GetAirAt(RemoveOffset(pos));
    }

    private List<T> AddIfMatchesTag<T>(List<T> list, RoomInfo info, string tag)
    {
        if (info.CompareTag(tag))
            list.Add(info.GetComponent<T>());

        return list;
    }

    private List<T> RemoveIfMatchesTag<T>(List<T> list, RoomInfo info, string tag)
    {
        if (info.CompareTag(tag))
            list.Remove(info.GetComponent<T>());

        return list;
    }
}