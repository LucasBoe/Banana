using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileSpawner : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [SerializeField] RoomInfo roomInfo;
    [SerializeField] Projectile prefab;
    [SerializeField] IEnemyCombatTarget target;
    [SerializeField] Transform origin;
    [SerializeField] float intervall = 1f;

    private void OnEnable()
    {
        enemy.SetEnemyState += OnChangedState;
    }

    private void OnDisable()
    {
        enemy.SetEnemyState -= OnChangedState;
    }

    private void OnChangedState(EnemyState state, IEnemyCombatTarget newTarget)
    {
        target = newTarget;
    }

    private void FixedUpdate()
    {
        if (target != null && !target.IsNull)
        {
            Vector2 dir = (target.Position - (Vector2)transform.position).normalized;
            transform.up = dir;
        }
    }

    public void Spawn()
    {
        Vector2 dir = (target.Position - (Vector2)origin.position).normalized;
        Instantiate(prefab, (Vector2)origin.position, Quaternion.identity).Shoot(roomInfo.Room, dir);
    }
}
