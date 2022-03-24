using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : SingletonBehaviour<Pathfinder>
{
    private Queue<Pathrequest> paths = new Queue<Pathrequest>();
    [SerializeField] private int pathCounter = 0;

    public Pathrequest GetPathTo(Vector2 start, Vector2 end, Room room, Collider2D[] toIgnore = null)
    {
        Pathrequest pathrequest = new Pathrequest(start, end, room, toIgnore);
        paths.Enqueue(pathrequest);
        return pathrequest;
    }

    private void Update()
    {
        pathCounter = paths.Count;
        if (pathCounter == 0) return;
        paths.Dequeue().Resolve();
    }
}

public class Pathrequest
{
    private List<Collider2D> toIgnore;
    private Room room;
    private Vector2 start;
    private Vector2 end;

    public System.Action<Pathrequest, List<Vector2>> Resolved;

    public Pathrequest(Vector2 start, Vector2 end, Room room, Collider2D[] toIgnore)
    {
        this.start = start;
        this.end = end;
        this.room = room;
        this.toIgnore = new List<Collider2D>(toIgnore);
    }
    public void Resolve()
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

            bool isAirAtDirect = CheckPosition(toCheck);
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
            CleanUp(points, i);


        for (int i = 1; i < points.Count; i++)
            Debug.DrawLine(points[i - 1], points[i], Color.green, 0.5f);

        Resolved?.Invoke(this, points);
    }

    private bool CheckPosition(Vector2 toCheck)
    {
        Collider2D collider = Physics2D.OverlapBox(toCheck, new Vector2(0.125f, 0.125f), 0);
        return room.IsInside(toCheck + Vector2.up * 0.25f) && room.IsInside(toCheck + Vector2.down * 0.25f) && room.IsInside(toCheck + Vector2.right * 0.25f) && room.IsInside(toCheck + Vector2.left * 0.25f) && (collider == null || toIgnore.Contains(collider));
    }

    private bool CheckLine(Vector2 start, Vector2 end)
    {
        int intervalls = Mathf.RoundToInt(Vector2.Distance(start, end) * 2);
        for (int i = 0; i < intervalls; i++)
        {
            Vector2 point = Vector2.Lerp(start, end, i / (float)(intervalls));
            if (!CheckPosition(point))
                return false;
        }

        return true;
    }

    private void CleanUp(List<Vector2> points, float maxDistance)
    {
        List<Vector2> toRemove = new List<Vector2>();

        for (int i = 1; i < points.Count - 1; i += 2)
        {
            Vector2 before = points[i - 1];
            Vector2 after = points[i + 1];

            if (Vector2.Distance(before, after) < maxDistance && CheckLine(before, after))
                toRemove.Add(points[i]);
        }

        foreach (Vector2 point in toRemove)
            points.Remove(point);
    }
}
