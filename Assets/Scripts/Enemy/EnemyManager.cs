using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonBehaviour<EnemyManager>
{
    [SerializeField] List<Enemy> active;

    public Enemy GetEnemy()
    {
        if (active.Count > 0)
            return active[0];

        return null;
    }

    internal void RegisterActive(Enemy enemy)
    {
        active.Add(enemy);
    }

    internal void UnregisterActive(Enemy enemy)
    {
        active.Remove(enemy);
    }
}
