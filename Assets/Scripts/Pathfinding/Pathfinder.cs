using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : SingletonBehaviour<Pathfinder>
{
    public List<Vector2> GetPathTo(Vector2 start, Vector2 end, Room room, Collider2D[] toIgnore = null)
    {
        List<Vector2> points = new List<Vector2>();
        List<Collider2D> ignore = new List<Collider2D>(toIgnore);

        float distance = Vector2.Distance(start, end);
        int stepCounter = 0;
        float angleOffset = 0f;


        while (stepCounter < 100 && distance > 0.1f)
        {
            distance = Vector2.Distance(start, end);

            Vector2 directVector = (end - start).normalized;
            Vector2 roatedVector = directVector.Rotate(angleOffset);
            Vector2 toCheck = start + roatedVector * (distance < 0.5f ? 0.1f : 0.25f);

            bool isAirAtDirect = CheckPosition(toCheck, ignore, room);
            Util.DebugDrawCircle(toCheck, isAirAtDirect ? Color.green : Color.red, 0.02f, lifetime: 0.5f);

            if (isAirAtDirect)
            {
                points.Add(toCheck);
                start = toCheck;
                angleOffset = 0;
            }
            else
            {
                if (angleOffset > 0)
                    angleOffset = -angleOffset;
                else
                    angleOffset = (-angleOffset + 45f);
            }
            stepCounter++;
        }

        for (int i = 1; i < 6; i++)
            CleanUp(points, i, ignore, room);


        for (int i = 1; i < points.Count; i++)
            Debug.DrawLine(points[i - 1], points[i], Color.green, 0.5f);

        return points;
    }

    private bool CheckPosition(Vector2 toCheck, List<Collider2D> toIgnore, Room room)
    {
        Collider2D collider = Physics2D.OverlapBox(toCheck, new Vector2(0.125f, 0.125f), 0);
        return room.IsInside(toCheck + Vector2.up * 0.25f) && room.IsInside(toCheck + Vector2.down * 0.25f) && room.IsInside(toCheck + Vector2.right * 0.25f) && room.IsInside(toCheck + Vector2.left * 0.25f) && (collider == null || toIgnore.Contains(collider));
    }

    private bool CheckLine(Vector2 start, Vector2 end, List<Collider2D> toIgnore, Room room)
    {
        int intervalls = Mathf.RoundToInt(Vector2.Distance(start, end) * 2);
        for (int i = 0; i < intervalls; i++)
        {
            Vector2 point = Vector2.Lerp(start, end, i / (float)(intervalls));
            if (!CheckPosition(point, toIgnore, room))
                return false;
        }

        return true;
    }

    private void CleanUp(List<Vector2> points, float maxDistance, List<Collider2D> toIgnore, Room room)
    {
        List<Vector2> toRemove = new List<Vector2>();

        for (int i = 1; i < points.Count - 1; i += 2)
        {
            Vector2 before = points[i - 1];
            Vector2 after = points[i + 1];

            if (Vector2.Distance(before, after) < maxDistance && CheckLine(before, after, toIgnore, room))
                toRemove.Add(points[i]);
        }

        foreach (Vector2 point in toRemove)
            points.Remove(point);
    }
}
