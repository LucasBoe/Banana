using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    bool isInRazorMode = true;

    void OnSceneGUI()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        bool clickedOnUI = false;

        Handles.BeginGUI();

        if (GUI.Button(new Rect(20, 20, 40, 40), EditorGUIUtility.FindTexture(isInRazorMode ? "d_Grid.EraserTool@2x" : "d_Grid.PaintTool@2x")))
        {
            isInRazorMode = !isInRazorMode;
            clickedOnUI = true;
        }
        Handles.EndGUI();

        if (clickedOnUI) return;


        if (Event.current.type == EventType.MouseDown)
        {
            Plane groundPlane = new Plane(new Vector3(0, 0, 1), Vector3.zero);

            //Ray ray = Camera.current.ScreenPointToRay(Event.current.mousePosition);
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            float rayDistance;
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                Util.DebugDrawCross(point, Color.green, 0.5f, lifetime: 3f);
                Room room = (target as Room);

                Vector2Int tile = room.RemoveOffset(point);



                if (isInRazorMode)
                {
                    room.TileData.RemoveTileAt(tile);
                    if (room.TileData.IsBorderTile(tile))
                        room.TileData.RemoveEmptyTiles();
                }
                else
                    room.TileData.AddAirAt(tile);

                room.GetComponent<RoomMeshCreator>().UpdateMesh(room.TileData);

                EditorUtility.SetDirty(room);
            }
        }
    }
}
