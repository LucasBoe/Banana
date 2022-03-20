using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] public MapData MapData;

    public Vector2Int RemoveOffset(Vector3 point)
    {
        Vector2Int newPoint =  new Vector2Int(Mathf.FloorToInt(point.x - transform.position.x), Mathf.FloorToInt((point.y) - transform.position.y));
        return MapData.RemoveOffset(newPoint);
    }
    internal Vector2 AddaptToRoomPerimeter(Vector2 origin, Vector2 target)
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
        return MapData.GetAirAt(RemoveOffset(pos));
    }

    [ContextMenu("RemoveEmptyTiles")]
    public void RemoveEmptyTiles()
    {
        MapData.RemoveEmptyTiles();
    }
}

public class NeightbourResult
{
    public bool Right, Left, Top, Bottom;
    public override string ToString()
    {
        return $"r:{Right}, l:{Left}, t:{Top}, b:{Bottom}";
    }
}

public class MeshData
{
    public List<Vector3> Verts = new List<Vector3>();
    public List<int> Tris = new List<int>();
    public List<Vector3> Normals = new List<Vector3>();
    public List<Vector2> UV = new List<Vector2>();
}