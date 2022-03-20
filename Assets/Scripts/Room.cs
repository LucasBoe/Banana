using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] public TileData TileData;
    public List<RoomInfo> children;

    private void Awake()
    {
        children = new List<RoomInfo>(GetComponentsInChildren<RoomInfo>());
        foreach (RoomInfo info in children)
            info.Room = this;
    }

    public Vector2Int RemoveOffset(Vector3 point)
    {
        Vector2Int newPoint =  new Vector2Int(Mathf.FloorToInt(point.x - transform.position.x), Mathf.FloorToInt((point.y) - transform.position.y));
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

    [ContextMenu("RemoveEmptyTiles")]
    public void RemoveEmptyTiles()
    {
        TileData.RemoveEmptyTiles();
    }
}