using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    int activeToolIndex = 0;
    bool isInEditMode = false;

    void OnSceneGUI()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        bool clickedOnUI = false;

        Handles.BeginGUI();

        if (isInEditMode)
        {
            var toolbarImages = new Texture[] { EditorGUIUtility.FindTexture("d_Grid.PaintTool@2x"), EditorGUIUtility.FindTexture("d_Grid.EraserTool@2x"), (Texture)AssetDatabase.LoadAssetAtPath("Assets/Textures/Editor-Icons/editor_icon_portal.png", typeof(Texture)) };

            int indexBefore = activeToolIndex;
            activeToolIndex = GUI.Toolbar(new Rect(70, 20, 120, 40), indexBefore, toolbarImages);

            if (indexBefore != activeToolIndex)
                clickedOnUI = true;
        }

        if (GUI.Button(new Rect(20, 20, 40, 40), EditorGUIUtility.FindTexture("UnityEditor.HierarchyWindow@2x")))
        {
            isInEditMode = !isInEditMode;
            clickedOnUI = true;
        }


        Handles.EndGUI();

        if (clickedOnUI) return;


        if (isInEditMode && Event.current.type == EventType.MouseDown)
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

                if (activeToolIndex == 1)
                {
                    room.TileData.RemoveTileAt(tile);
                    if (room.TileData.IsBorderTile(tile))
                        room.TileData.RemoveEmptyTiles();
                }
                else
                {
                    TileType type = activeToolIndex == 0 ? TileType.AIR :  TileType.PORTAL;
                    room.TileData.SetTilAt(tile, type);
                }

                room.GetComponent<RoomMeshCreator>().UpdateMesh(room.TileData);

                EditorUtility.SetDirty(room);
            }
        }
    }
}

