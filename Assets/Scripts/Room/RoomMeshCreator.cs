using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[ExecuteInEditMode]
public class RoomMeshCreator : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material material;

    public void UpdateMesh(TileData data)
    {
        meshRenderer.sharedMaterial = material;

        MeshData meshData = new MeshData();

        for (int x = 0; x < data.Array.Size.x; x++)
        {
            for (int y = 0; y < data.Array.Size.y; y++)
            {
                NeightbourResult result = new NeightbourResult()
                {
                    Left = !data.IsAir(x - 1, y),
                    Right = !data.IsAir(x + 1, y),
                    Top = !data.IsAir(x, y - 1),
                    Bottom = !data.IsAir(x, y + 1),
                    corner = data.IsAir(x + 1, y + 1) || data.IsAir(x - 1, y + 1) || data.IsAir(x - 1, y - 1) || data.IsAir(x + 1, y - 1)
                };
                Vector2Int pos = new Vector2Int(x + data.Array.Offset.x, y + data.Array.Offset.y);
                DrawTile(pos, data.Get(x, y), result, meshData);
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

    private void DrawTile(Vector2Int pos, TileType type, NeightbourResult result, MeshData meshData)
    {
        if (type == TileType.AIR)
        {
            int zero = meshData.Verts.Count;
            meshData.Verts = meshData.Verts.Concat(new Vector3[4] {
                new Vector3(pos.x, pos.y),
                new Vector3(pos.x + 1, pos.y),
                new Vector3(pos.x, pos.y + 1),
                new Vector3(pos.x +1, pos.y + 1)
            }).ToList();

            meshData.Tris = ConnectQuad(meshData, zero);
            meshData.Normals = CreateNormals(meshData, Vector3.back);
            meshData.UV = GetUvs(meshData, new int[] { 12, 13, 8, 9 }, 4);
        }
        else
        {
            if (!result.Top)
            {
                Vector3 a = new Vector3(pos.x, pos.y);
                Vector3 b = new Vector3(pos.x + 1, pos.y);
                Vector3 c = new Vector3(pos.x, pos.y, -1);
                Vector3 d = new Vector3(pos.x + 1, pos.y, -1);

                CreateWallMesh(meshData, a, b, c, d, Vector3.up, GetSideFrontTextureForType(type), GetSideBackTextureForType(type));

                pos = CreateTopPlaneWithTexture(pos, meshData, 4);
            }

            if (!result.Bottom)
            {
                Vector3 a = new Vector3(pos.x + 1, pos.y + 1);
                Vector3 b = new Vector3(pos.x, pos.y + 1);
                Vector3 c = new Vector3(pos.x + 1, pos.y + 1, -1);
                Vector3 d = new Vector3(pos.x, pos.y + 1, -1);

                CreateWallMesh(meshData, a, b, c, d, Vector3.down, GetSideFrontTextureForType(type), GetSideBackTextureForType(type));

                pos = CreateTopPlaneWithTexture(pos, meshData, 0);
            }

            if (!result.Right)
            {
                Vector3 a = new Vector3(pos.x + 1, pos.y);
                Vector3 b = new Vector3(pos.x + 1, pos.y + 1);
                Vector3 c = new Vector3(pos.x + 1, pos.y, -1);
                Vector3 d = new Vector3(pos.x + 1, pos.y + 1, -1);

                CreateWallMesh(meshData, a, b, c, d, Vector3.left, GetSideFrontTextureForType(type), GetSideBackTextureForType(type));

                pos = CreateTopPlaneWithTexture(pos, meshData, 1);
            }

            if (!result.Left)
            {
                Vector3 a = new Vector3(pos.x, pos.y + 1);
                Vector3 b = new Vector3(pos.x, pos.y);
                Vector3 c = new Vector3(pos.x, pos.y + 1, -1);
                Vector3 d = new Vector3(pos.x, pos.y, -1);

                CreateWallMesh(meshData, a, b, c, d, Vector3.right, GetSideFrontTextureForType(type), GetSideBackTextureForType(type));

                pos = CreateTopPlaneWithTexture(pos, meshData, 5);
            }

            if (result.Corner)
            {
                int z = meshData.Verts.Count;
                meshData.Verts = meshData.Verts.Concat(new Vector3[4] {
                    new Vector3(pos.x,  pos.y, -1 ),
                    new Vector3(pos.x + 1,  pos.y, -1),
                    new Vector3(pos.x,  pos.y + 1, -1),
                    new Vector3(pos.x +1,  pos.y + 1, -1)
                }).ToList();

                meshData.Tris = ConnectQuad(meshData, z);
                meshData.Normals = CreateNormals(meshData, Vector3.up);
                meshData.UV = GetUvs(meshData, new int[] { 3 }, 4);
            }
        }
    }

    private int[] GetSideBackTextureForType(TileType type)
    {
        switch (type)
        {
            case TileType.PORTAL:
                return new int[] { 6 };
        }

        return new int[] { 3 };
    }

    private int[] GetSideFrontTextureForType(TileType type)
    {
        switch (type)
        {
            case TileType.PORTAL:
                return new int[] { 7 };
        }

        return new int[] { 14, 15, 10, 11 };
    }

    private void CreateWallMesh(MeshData meshData, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 backOffset, int[] wallIndexies, int[] backIndexies)
    {
        int z = meshData.Verts.Count;
        meshData.Verts = meshData.Verts.Concat(new Vector3[4] { a, b, c, d, }).ToList();
        meshData.Tris = ConnectQuad(meshData, z);
        meshData.Normals = CreateNormals(meshData, -backOffset);
        meshData.UV = GetUvs(meshData, wallIndexies, 4);

        var o = backOffset * 0.1f;

        int zInner = meshData.Verts.Count;
        meshData.Verts = meshData.Verts.Concat(new Vector3[4] { b + o, a + o, d + o, c + o }).ToList();
        meshData.Tris = ConnectQuad(meshData, zInner);
        meshData.Normals = CreateNormals(meshData, backOffset);
        meshData.UV = GetUvs(meshData, backIndexies, 4);
    }

    private Vector2Int CreateTopPlaneWithTexture(Vector2Int pos, MeshData meshData, int textureIndex)
    {
        int zTop = meshData.Verts.Count;
        meshData.Verts = meshData.Verts.Concat(new Vector3[4] {
                    new Vector3(pos.x,  pos.y, -1 ),
                    new Vector3(pos.x + 1,  pos.y, -1),
                    new Vector3(pos.x,  pos.y + 1, -1),
                    new Vector3(pos.x +1,  pos.y + 1, -1)
                }).ToList();

        meshData.Tris = ConnectQuad(meshData, zTop);
        meshData.Normals = CreateNormals(meshData, Vector3.back);
        meshData.UV = GetUvs(meshData, new int[] { textureIndex }, 4);
        return pos;
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
                up,
                up,
                up,
                up
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
public class MeshData
{
    public List<Vector3> Verts = new List<Vector3>();
    public List<int> Tris = new List<int>();
    public List<Vector3> Normals = new List<Vector3>();
    public List<Vector2> UV = new List<Vector2>();
}
public class NeightbourResult
{
    public bool corner;
    public bool Right, Left, Top, Bottom;
    public bool Corner => corner && Top && Bottom && Left && Right;
    public override string ToString()
    {
        return $"r:{Right}, l:{Left}, t:{Top}, b:{Bottom}";
    }
}