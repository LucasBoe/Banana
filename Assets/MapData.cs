using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapData : ScriptableObject
{
    public Vector2Int start;
    public Vector2Int size;
    public bool[,] Air = new bool[0,0];

    [Button]
    public void Clear()
    {
        Air = new bool[,] { { true } };
        size = Vector2Int.one;
        start = Vector2Int.zero;
    }

    internal bool IsAir(int x, int y)
    {
        if (!IsInsidePerimeterLocal(x, y))
            return false;

        Debug.LogWarning($"IsAir {x}, {y}");

        return Air[x, y];
    }

    private bool IsInsidePerimeterLocal(int x, int y)
    {
        if (Air.Length == 0)
            return false;

        return x >= 0 && x < size.x - 1 && y >= 0 && y < size.y - 1;
    }
    public void AddAirAt(Vector2Int global)
    {
        Vector2Int local = ToLocal(global);

        if (IsInsidePerimeterLocal(local.x, local.y))
        {
            Air[local.x, local.y] = true;
        }
        else
        {
            ResizeToFit(global.x, global.y);
        }
    }

    private void ResizeToFit(int newX, int newY)
    {
        Vector2Int oldMin = start;
        Vector2Int oldSize = size;
        Vector2Int oldMax = start + size;

        int newMinX = newX - 1 < oldMin.x ? newX - 1 : oldMin.x;
        int newMinY = newY - 1 < oldMin.y ? newY - 1 : oldMin.y;
        int newMaxX = newX + 1 > oldMax.x ? newX + 1 : oldMax.x;
        int newMaxY = newY + 1 > oldMax.y ? newY + 1 : oldMax.y;

        Vector2Int newMin = new Vector2Int(newMinX, newMinY);
        Vector2Int newMax = new Vector2Int(newMaxX, newMaxY);
        Vector2Int newSize = newMax - newMin;

        bool[,] newAir = new bool[newSize.x, newSize.y];
        for (int x = 0; x < newSize.x; x++)
        {
            for (int y = 0; y < newSize.y; y++)
            {
                Vector2Int toGlobal = new Vector2Int(x + newMin.x, y + newMin.y);
                Vector2Int toOld = toGlobal - oldMin;
                bool isNewTile = toGlobal.x == newX && toGlobal.y == newY;
                newAir[x, y] = (IsInsidePerimeterLocal(toOld.x, toOld.y) && Air[toOld.x, toOld.y]) || isNewTile;
            }
        }

        Air = newAir;
        start = newMin;
        size = newSize;
}

    private Vector2Int ToLocal(Vector2Int global)
    {
        return new Vector2Int(global.x - start.x, global.y - start.y);
    }
}
