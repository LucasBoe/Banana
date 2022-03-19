using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeFinder : MonoBehaviour
{
    [SerializeField] MapData mapData;

    [Button]
    private void Find()
    {
        StopAllCoroutines();
        StartCoroutine(FindRoutine());
    }

    private IEnumerator FindRoutine()
    {
        for (int x = 0; x < mapData.map.Size.x; x++)
        {
            int y = 0;


            while (!mapData.map.Get(x, y) && y < mapData.map.Size.y)
                y++;

            if (y < mapData.map.Size.y)
            {
                CreatePoint(x, y);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private Vector2 CreatePoint(int x, int y)
    {
        Vector2 point = new Vector2(mapData.start.x + x, mapData.start.y + y);
        Util.DebugDrawCross((Vector3)point + 1.5f * Vector3.back, Color.yellow, 0.25f, 10f);
        return point;
    }
}
