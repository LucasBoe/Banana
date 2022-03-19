using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuadCreator))]
public class TileEditor : Editor
{
    void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            Plane groundPlane = new Plane(new Vector3(0, 1, 0), Vector3.zero);

            Debug.Log("Event.current.type");
            //Ray ray = Camera.current.ScreenPointToRay(Event.current.mousePosition);
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            float rayDistance;
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                Util.DebugDrawCross(point, Color.green, 0.5f, lifetime: 3f);
                QuadCreator t = (target as QuadCreator);
                Vector2Int tile = new Vector2Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.z / QuadCreator.yScale));
                t.MapData.AddAirAt(tile);
                t.UpdateMesh();
            }
        }
    }
}

