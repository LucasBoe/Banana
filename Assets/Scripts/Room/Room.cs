using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    [SerializeField] public TileData TileData;

    public List<Portal> Portals = new List<Portal>();
    public List<Enemy> Enemys = new List<Enemy>();
    public List<Player> Players = new List<Player>();
    public List<IEnemyCombatTarget> EnemyCombatTargets = new List<IEnemyCombatTarget>();

    private Transform[] randomPositions;

    private void Awake()
    {
        List<RoomInfo> children = new List<RoomInfo>(GetComponentsInChildren<RoomInfo>());
        foreach (RoomInfo info in children)
        {
            info.Room = this;
            RegisterInfo(info);
        }

        RoomManager.Instance.RegisterRoom(this);
    }

    private void Start()
    {
        int randomPointCount = (TileData.Array.Size.x + TileData.Array.Size.y) / 2;
        List<Transform> randomPoints = new List<Transform>();
        for (int i = 0; i < randomPointCount; i++)
            randomPoints.Add(CreateNewRandomPoint());

        randomPositions = randomPoints.ToArray();
    }

    private Transform CreateNewRandomPoint()
    {
        Vector2 point = new Vector2(float.MinValue, float.MaxValue);
        while (!IsInside(point))
        {
            float minX = transform.position.x + TileData.Array.Offset.x;
            float minY = transform.position.y + TileData.Array.Offset.y;
            point = new Vector2(minX + Random.Range(0,TileData.Array.Size.x) + 0.5f, minY + Random.Range(0, TileData.Array.Size.y) + 0.5f);
        }

        GameObject random = new GameObject("randomPoint");
        random.transform.position = point;
        return random.transform;
    }

    public Transform GetRandomPoint()
    {
        return randomPositions[Random.Range(0, randomPositions.Length)];
    }

    public void RegisterInfo(RoomInfo info)
    {
        Portals = AddIfMatchesTag(Portals, info, "Portal");
        Enemys = AddIfMatchesTag(Enemys, info, "Enemy");
        Players = AddIfMatchesTag(Players, info, "Player");
        EnemyCombatTargets = AddIfMatchesTag(EnemyCombatTargets, info, "Player");
        EnemyCombatTargets = AddIfMatchesTag(EnemyCombatTargets, info, "Helper");
    }

    public void UnregisterInfo(RoomInfo info)
    {
        Portals = RemoveIfMatchesTag(Portals, info, "Portal");
        Enemys = RemoveIfMatchesTag(Enemys, info, "Enemy");
        Players = RemoveIfMatchesTag(Players, info, "Player");
        EnemyCombatTargets = RemoveIfMatchesTag(EnemyCombatTargets, info, "Player");
        EnemyCombatTargets = RemoveIfMatchesTag(EnemyCombatTargets, info, "Helper");
    }

    public Vector2Int RemoveOffset(Vector3 point)
    {
        //Vector2Int newPoint = new Vector2Int(Mathf.FloorToInt(point.x - transform.position.x), Mathf.FloorToInt((point.y) - transform.position.y));
        Vector2 toLocal = transform.InverseTransformPoint(point);
        Vector2Int newPoint = new Vector2Int(Mathf.FloorToInt(toLocal.x), Mathf.FloorToInt(toLocal.y));
        return TileData.RemoveOffset(newPoint);
    }
    internal Vector2 AdaptVector2ToRoomPerimeter(Vector2 origin, Vector2 target)
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
        return TileData.Get(RemoveOffset(pos)) != TileType.SOLID;
    }

    private List<T> AddIfMatchesTag<T>(List<T> list, RoomInfo info, string tag)
    {
        if (info.CompareTag(tag))
            list.Add(info.GetComponent<T>());

        return list;
    }

    private List<T> RemoveIfMatchesTag<T>(List<T> list, RoomInfo info, string tag)
    {
        if (info.CompareTag(tag))
            list.Remove(info.GetComponent<T>());

        return list;
    }
}