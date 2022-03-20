using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [SerializeField] public MapData MapData;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material material;

    public const float yScale = 1f;

    public void UpdateMesh()
    {
        meshRenderer.sharedMaterial = material;

        MeshData meshData = new MeshData();

        for (int x = 0; x < MapData.map.Size.x; x++)
        {
            for (int y = 0; y < MapData.map.Size.y; y++)
            {
                NeightbourResult result = new NeightbourResult()
                {
                    Left = !MapData.IsAir(x - 1, y),
                    Right = !MapData.IsAir(x + 1, y),
                    Top = !MapData.IsAir(x, y - 1),
                    Bottom = !MapData.IsAir(x, y + 1)
                };
                Vector2Int pos = new Vector2Int(x + MapData.start.x, y + MapData.start.y);
                DrawTile(pos, MapData.IsAir(x, y), result, meshData);
            }
        }

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        mesh.vertices = meshData.Verts.ToArray();
        mesh.triangles = meshData.Tris.ToArray();
        mesh.normals = meshData.Normals.ToArray();
        mesh.uv = meshData.UV.ToArray();

        meshFilter.mesh = mesh;
    }

    public Vector2Int RemoveOffset(Vector3 point)
    {
        Vector2Int newPoint =  new Vector2Int(Mathf.FloorToInt(point.x - transform.position.x), Mathf.FloorToInt((point.y / Room.yScale) - transform.position.y));
        return MapData.RemoveOffset(newPoint);
    }
    internal Vector2 AddaptToRoomPerimeter(Vector2 origin, Vector2 target)
    {
        if (IsInside(target))        
            return target;        
        else if (IsInside(new Vector2(target.x, origin.y)))        
            return new Vector2(target.x, origin.y);        
        else if (IsInside(new Vector2(origin.x, target.y)))        
            return new Vector2(origin.x, target.y);      

        return origin;
    }

    public bool IsInside(Vector2 pos)
    {
        return MapData.GetAirAt(RemoveOffset(pos));
    }

    private void DrawTile(Vector2Int pos, bool air, NeightbourResult result, MeshData meshData)
    {
        if (air)
        {
            int zero = meshData.Verts.Count;
            meshData.Verts = meshData.Verts.Concat(new Vector3[4] {
                new Vector3(pos.x, pos.y * yScale),
                new Vector3(pos.x + 1, pos.y * yScale),
                new Vector3(pos.x, (pos.y + 1) * yScale),
                new Vector3(pos.x +1, (pos.y + 1) * yScale)
            }).ToList();

            meshData.Tris = ConnectQuad(meshData, zero);
            meshData.Normals = CreateNormals(meshData, Vector3.up);
            meshData.UV = GetUvs(meshData, new int[] { 12, 13, 8, 9 }, 4);


            if (result.Top)
            {
                int z = meshData.Verts.Count;
                meshData.Verts = meshData.Verts.Concat(new Vector3[4] {
                    new Vector3(pos.x, pos.y * yScale ),
                    new Vector3(pos.x + 1, pos.y * yScale),
                    new Vector3(pos.x, pos.y * yScale, -yScale ),
                    new Vector3(pos.x + 1, pos.y * yScale, -yScale),
                }).ToList();

                meshData.Tris = ConnectQuad(meshData, z);
                meshData.Normals = CreateNormals(meshData, Vector3.back);
                meshData.UV = GetUvs(meshData, new int[] { 2, 3, 6, 7 }, 4);
            }

            if (result.Bottom)
            {
                int z = meshData.Verts.Count;
                meshData.Verts = meshData.Verts.Concat(new Vector3[4] {
                    new Vector3(pos.x, (pos.y + 1) * yScale ),
                    new Vector3(pos.x + 1, (pos.y + 1) * yScale),
                    new Vector3(pos.x, (pos.y + 1) * yScale, -yScale ),
                    new Vector3(pos.x + 1, (pos.y + 1) * yScale, -yScale),
                }).ToList();

                meshData.Tris = ConnectQuad(meshData, z);
                meshData.Normals = CreateNormals(meshData, Vector3.back);
                meshData.UV = GetUvs(meshData, new int[] { 14, 15, 10, 11 }, 4);
            }

            if (result.Right)
            {
                int z = meshData.Verts.Count;
                meshData.Verts = meshData.Verts.Concat(new Vector3[4] {
                    new Vector3(pos.x + 1, (pos.y + 1) * yScale),
                    new Vector3(pos.x + 1,  pos.y * yScale ),
                    new Vector3(pos.x + 1, (pos.y + 1) * yScale, -yScale),
                    new Vector3(pos.x + 1, pos.y * yScale, -yScale ),
                }).ToList();

                meshData.Tris = ConnectQuad(meshData, z);
                meshData.Normals = CreateNormals(meshData, Vector3.left);
                meshData.UV = GetUvs(meshData, new int[] { 14, 15, 10, 11 }, 4);
            }

            if (result.Left)
            {
                int z = meshData.Verts.Count;
                meshData.Verts = meshData.Verts.Concat(new Vector3[4] {
                    new Vector3(pos.x, (pos.y + 1) * yScale, -yScale),
                    new Vector3(pos.x, pos.y * yScale , -yScale),
                    new Vector3(pos.x, (pos.y + 1) * yScale),
                    new Vector3(pos.x, pos.y * yScale ),
                }).ToList();

                meshData.Tris = ConnectQuad(meshData, z);
                meshData.Normals = CreateNormals(meshData, Vector3.right);
                meshData.UV = GetUvs(meshData, new int[] { 14, 15, 10, 11 }, 4);
            }
        }
        else
        {
            if (!result.Top)
            {
                int z = meshData.Verts.Count;
                meshData.Verts = meshData.Verts.Concat(new Vector3[4] {
                    new Vector3(pos.x,  pos.y * yScale, -yScale ),
                    new Vector3(pos.x + 1,  pos.y * yScale, -yScale),
                    new Vector3(pos.x,  (pos.y + 1) * yScale, -yScale),
                    new Vector3(pos.x +1,  (pos.y + 1) * yScale, -yScale)
                }).ToList();

                meshData.Tris = ConnectQuad(meshData, z);
                meshData.Normals = CreateNormals(meshData, Vector3.up);
                meshData.UV = GetUvs(meshData, new int[] { 4 }, 4);
            }

            if (!result.Bottom)
            {
                int z = meshData.Verts.Count;
                meshData.Verts = meshData.Verts.Concat(new Vector3[4] {
                    new Vector3(pos.x,  pos.y * yScale, -yScale ),
                    new Vector3(pos.x + 1,  pos.y * yScale, -yScale),
                    new Vector3(pos.x,  (pos.y + 1) * yScale, -yScale),
                    new Vector3(pos.x +1,  (pos.y + 1) * yScale, -yScale)
                }).ToList();

                meshData.Tris = ConnectQuad(meshData, z);
                meshData.Normals = CreateNormals(meshData, Vector3.up);
                meshData.UV = GetUvs(meshData, new int[] { 0 }, 4);
            }

            if (!result.Right)
            {
                int z = meshData.Verts.Count;
                meshData.Verts = meshData.Verts.Concat(new Vector3[4] {
                    new Vector3(pos.x,  pos.y * yScale, -yScale ),
                    new Vector3(pos.x + 1,  pos.y * yScale, -yScale),
                    new Vector3(pos.x,  (pos.y + 1) * yScale, -yScale),
                    new Vector3(pos.x +1,  (pos.y + 1) * yScale,-yScale)
                }).ToList();

                meshData.Tris = ConnectQuad(meshData, z);
                meshData.Normals = CreateNormals(meshData, Vector3.up);
                meshData.UV = GetUvs(meshData, new int[] { 1 }, 4);
            }

            if (!result.Left)
            {
                int z = meshData.Verts.Count;
                meshData.Verts = meshData.Verts.Concat(new Vector3[4] {
                    new Vector3(pos.x,  pos.y * yScale, -yScale ),
                    new Vector3(pos.x + 1,  pos.y * yScale, -yScale),
                    new Vector3(pos.x,  (pos.y + 1) * yScale, -yScale),
                    new Vector3(pos.x +1,  (pos.y + 1) * yScale, -yScale)
                }).ToList();

                meshData.Tris = ConnectQuad(meshData, z);
                meshData.Normals = CreateNormals(meshData, Vector3.up);
                meshData.UV = GetUvs(meshData, new int[] { 5 }, 4);
            }
        }
    }

    private List<Vector2> GetUvs(MeshData meshData, int[] vs, int tileCount)
    {
        int random = Util.GetRandom(vs);
        float size = 1f / tileCount;
        Vector2 start = new Vector2(
            (float)(random % tileCount) / tileCount,
            Mathf.Round(random / tileCount) / tileCount
            );

        return meshData.UV.Concat(new Vector2[]{
                new Vector2(start.x, start.y),
                new Vector2(start.x + size, start.y),
                new Vector2(start.x, start.y + size),
                new Vector2(start.x +size, start.y + size)
        }).ToList();
    }

    private List<Vector3> CreateNormals(MeshData meshData, Vector3 up)
    {
        return meshData.Normals.Concat(new Vector3[4] {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up
            }).ToList();
    }

    private static List<int> ConnectQuad(MeshData meshData, int zero)
    {
        return meshData.Tris.Concat(new int[6] {
                // lower left triangle
                zero, zero +  2, zero + 1,
                // upper right triangle
                zero + 2, zero + 3, zero + 1
            }).ToList();
    }
}

public class NeightbourResult
{
    public bool Right, Left, Top, Bottom;
    public override string ToString()
    {
        return $"r:{Right}, l:{Left}, t:{Top}, b:{Bottom}";
    }
}

public class MeshData
{
    public List<Vector3> Verts = new List<Vector3>();
    public List<int> Tris = new List<int>();
    public List<Vector3> Normals = new List<Vector3>();
    public List<Vector2> UV = new List<Vector2>();
}