using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public TileArray Array;

    [Button]
    public void Clear()
    {
        Array = new TileArray(Vector2Int.zero, Vector2Int.zero);
    }

    internal bool IsAir(int x, int y)
    {
        if (!IsInsidePerimeter(x, y))
            return false;

        return Array.Get(x, y);
    }

    private bool IsInsidePerimeter(int x, int y)
    {
        return x >= 0 && x < Array.Size.x && y >= 0 && y < Array.Size.y;
    }

    private bool IsInsidePerimeterBorder(int x, int y)
    {
        return x > 0 && x + 1 < Array.Size.x - 2 && y > 0 && y + 1 < Array.Size.y;
    }

    public bool GetAirAt(Vector2Int local)
    {
        return IsAir(local.x, local.y);
    }

    public void AddAirAt(Vector2Int local)
    {
        if (IsInsidePerimeterBorder(local.x, local.y))
        {
            Array.Set(local.x, local.y, true);
        }
        else
        {
            ResizeToFit(local);
        }
    }

    public void RemoveEmptyTiles()
    {
        int minX = 1;
        while (CheckAxis(false, true, minX, Array.Size.y) && minX < Array.Size.x)
            minX++;

        int maxX = Array.Size.x - 1;
        while (CheckAxis(false, true, maxX, Array.Size.y) && maxX > 0)
            maxX--;

        int minY = 1;
        while (CheckAxis(true, false, minY, Array.Size.x) && minY < Array.Size.x)
            minY++;

        int maxY = Array.Size.y - 1;
        while (CheckAxis(true, false, maxY, Array.Size.x) && maxY > 0)
            maxY--;

        //add back borders
        minX -= 1; maxX += 2; minY -= 1; maxY += 2;

        Vector2Int newMin = new Vector2Int(minX, minY) + Array.Offset;
        Vector2Int newMax = new Vector2Int(maxX, maxY) + Array.Offset;

        if (newMin == Array.Offset && newMax == (Array.Offset + Array.Size)) return;

        Array = CreateNewMapWithSize(Array.Offset, newMin, newMax - newMin);
    }

    private bool CheckAxis(bool checkX, bool checkY, int staticAxisValue, int dynamicAxisMax)
    {
        if (checkX)
        {
            for (int dynamicAxisValue = 1; dynamicAxisValue < dynamicAxisMax; dynamicAxisValue++)
            {
                if (Array.Get(dynamicAxisValue, staticAxisValue)) return false;
            }
        }
        else if (checkY)
        {
            for (int dynamicAxisValue = 1; dynamicAxisValue < dynamicAxisMax; dynamicAxisValue++)
            {
                if (Array.Get(staticAxisValue, dynamicAxisValue)) return false;
            }
        }

        return true;
    }

    public void RemoveTileAt(Vector2Int local)
    {
        if (IsInsidePerimeter(local.x, local.y))
            Array.Set(local.x, local.y, false);
    }
    public bool IsBorderTile(Vector2Int local)
    {
        return !IsInsidePerimeterBorder(local.x, local.y);
    }

    private void ResizeToFit(Vector2Int local)
    {
        Vector2Int newTile = AddOffset(local);

        int newX = newTile.x;
        int newY = newTile.y;

        Vector2Int oldMin = Array.Offset;
        Vector2Int oldMax = Array.Offset + Array.Size;

        int newMinX = newX - 1 < oldMin.x ? newX - 2 : oldMin.x;
        int newMinY = newY - 1 < oldMin.y ? newY - 2 : oldMin.y;
        int newMaxX = newX + 1 >= oldMax.x ? newX + 2 : oldMax.x;
        int newMaxY = newY + 1 >= oldMax.y ? newY + 2 : oldMax.y;

        Vector2Int newMin = new Vector2Int(newMinX, newMinY);
        Vector2Int newMax = new Vector2Int(newMaxX, newMaxY);
        Vector2Int newSize = newMax - newMin;

        TileArray newMap = CreateNewMapWithSize(oldMin, newMin, newSize);
        newMap.Set(newX - newMinX, newY - newMinY, true);

        Array = newMap;
    }

    private TileArray CreateNewMapWithSize(Vector2Int oldMin, Vector2Int newMin, Vector2Int newSize)
    {
        TileArray newMap = new TileArray(newSize, newMin);
        for (int x = 0; x < newSize.x; x++)
        {
            for (int y = 0; y < newSize.y; y++)
            {
                Vector2Int toGlobal = new Vector2Int(x + newMin.x, y + newMin.y);
                Vector2Int toOld = toGlobal - oldMin;
                bool isOldTile = Array.Get(toOld.x, toOld.y);
                newMap.Set(x, y, isOldTile);
            }
        }

        return newMap;
    }

    public Vector2Int RemoveOffset(Vector2Int global)
    {
        return new Vector2Int(global.x - Array.Offset.x, global.y - Array.Offset.y);
    }
    public Vector2Int AddOffset(Vector2Int local)
    {
        return new Vector2Int(local.x + Array.Offset.x, local.y + Array.Offset.y);
    }
}

[System.Serializable]
public class TileArray
{
    public Vector2Int Size;
    public Vector2Int Offset;
    public bool[] Elements;

    public TileArray(Vector2Int newSize, Vector2Int newOffset)
    {
        Size = newSize;
        Offset = newOffset;
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
