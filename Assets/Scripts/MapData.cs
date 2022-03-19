using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapData : ScriptableObject
{
    public Vector2Int start;
    public MapArray map;

    [Button]
    public void Clear()
    {
        start = Vector2Int.zero;
        map = new MapArray(Vector2Int.zero);
    }

    internal bool IsAir(int x, int y)
    {
        if (!IsInsidePerimeter(x, y))
            return false;

        return map.Get(x, y);
    }

    private bool IsInsidePerimeter(int x, int y)
    {
        return x >= 0 && x < map.Size.x && y >= 0 && y < map.Size.y;
    }

    private bool IsInsidePerimeterBorder(int x, int y)
    {
        float maxX = map.Size.x - 1;
        float maxY = map.Size.y - 1;
        bool inside = x > 0 && x < maxX && y > 0 && y + 1 < maxY;

        return x > 0 && x + 1 < map.Size.x - 2 && y > 0 && y + 1 < map.Size.y;
    }

    public bool GetAirAt(Vector2Int global)
    {
        Vector2Int local = ToLocal(global);
        return IsAir(local.x, local.y);
    }

    public void AddAirAt(Vector2Int global)
    {
        Vector2Int local = ToLocal(global);

        if (IsInsidePerimeterBorder(local.x, local.y))
        {
            map.Set(local.x, local.y, true);
        }
        else
        {
            ResizeToFit(global.x, global.y);
        }
    }

    public void RemoveTileAt(Vector2Int global)
    {
        Vector2Int local = ToLocal(global);

        if (IsInsidePerimeter(local.x, local.y))
        {
            map.Set(local.x, local.y, false);
        }
    }

    private void ResizeToFit(int newX, int newY)
    {

        Vector2Int oldMin = start;
        Vector2Int oldSize = map.Size;
        Vector2Int oldMax = start + map.Size;

        int newMinX = newX - 1 < oldMin.x ? newX - 2 : oldMin.x;
        int newMinY = newY - 1 < oldMin.y ? newY - 2 : oldMin.y;
        int newMaxX = newX + 1 >= oldMax.x ? newX + 2 : oldMax.x;
        int newMaxY = newY + 1 >= oldMax.y ? newY + 2 : oldMax.y;

        Vector2Int newMin = new Vector2Int(newMinX, newMinY);
        Vector2Int newMax = new Vector2Int(newMaxX, newMaxY);
        Vector2Int newSize = newMax - newMin;

        MapArray newMap = new MapArray(newSize);
        for (int x = 0; x < newSize.x; x++)
        {
            for (int y = 0; y < newSize.y; y++)
            {
                Vector2Int toGlobal = new Vector2Int(x + newMin.x, y + newMin.y);
                Vector2Int toOld = toGlobal - oldMin;
                bool isOldTile = map.Get(toOld.x, toOld.y);
                bool isNewTile = toGlobal.x == newX && toGlobal.y == newY;
                newMap.Set(x, y, isOldTile || isNewTile);
            }
        }

        map = newMap;
        start = newMin;
        map.Size = newSize;
    }

    public Vector2Int ToLocal(Vector2Int global)
    {
        return new Vector2Int(global.x - start.x, global.y - start.y);
    }
}

[System.Serializable]
public class MapArray
{
    public Vector2Int Size;
    public bool[] Elements;

    public MapArray(Vector2Int newSize)
    {
        Size = newSize;
        Elements = new bool[newSize.x * newSize.y];
    }

    public bool Get(int x, int y)
    {
        bool outside = x < 0 || y < 0 || x >= Size.x || y >= Size.y;

        if (outside)
            return false;

        return Elements[y * Size.x + x];
    }
    public void Set(int x, int y, bool value)
    {
        Elements[y * Size.x + x] = value;
    }
}
