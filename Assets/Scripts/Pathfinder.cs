using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Room room;
    private void OnDrawGizmosSelected()
    {
        if (target == null) return;

        bool isTargetInsideRoom = IsTargetInsideRoom(target, room);

        if (!isTargetInsideRoom) return;


        Vector2 pos = transform.position;
        Gizmos.DrawSphere(pos, 0.25f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(target.position, 0.25f);

        float distance = Vector2.Distance(pos, target.position);
        int steps = 0;
        float angleOffset = 0f;
        float angleDirection = 1;

        List<Vector2> points = new List<Vector2>();

        while (steps < 100 && distance > 0.1f)
        {

            distance = Vector2.Distance(pos, target.position);

            Vector2 directVector = ((Vector2)target.position - pos).normalized;
            Vector2 roatedVector = directVector.Rotate(angleOffset);
            Vector2 toCheck = pos + roatedVector * (distance < 0.5f ? 0.1f : 0.5f);

            bool isAirAtDirect = CheckPosition(toCheck);
            Gizmos.color = isAirAtDirect ? Color.green : Color.red;
            Gizmos.DrawWireSphere(toCheck, 0.1f);

            if (isAirAtDirect)
            {
                points.Add(toCheck);
                pos = toCheck;
                angleOffset = 0;

            }
            else
            {
                if (angleOffset > 0)
                    angleOffset = - angleOffset;
                else
                {
                    angleOffset = (-angleOffset + 45f);
                }
            }

            steps++;
        }

        for (int i = 1; i < 6; i++)
        {
            CleanUp(points, i);
        }

        for (int i = 1; i < points.Count; i++)
            Gizmos.DrawLine(points[i - 1], points[i]);

#if UNITY_EDITOR
        Handles.Label(transform.position, steps.ToString());
#endif
    }

    private bool CheckPosition(Vector2 toCheck)
    {
        return room.IsInside(toCheck + Vector2.up * 0.25f) && room.IsInside(toCheck + Vector2.down * 0.25f) && room.IsInside(toCheck + Vector2.right * 0.25f) && room.IsInside(toCheck + Vector2.left * 0.25f);
    }

    private bool CheckPosition(Vector2 start, Vector2 end)
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

            if (Vector2.Distance(before, after) < maxDistance && CheckPosition(before, after))
                toRemove.Add(points[i]);
        }

        foreach (Vector2 point in toRemove)
            points.Remove(point);
    }

    private Vector2 ToGrid(Vector2 position)
    {
        return new Vector2(Mathf.Round(position.x + 0.5f) - 0.5f, Mathf.Round(position.y + 0.5f) - 0.5f);
    }

    private bool IsTargetInsideRoom(Transform target, Room room)
    {
        return room.IsInside(target.position);
    }
}
