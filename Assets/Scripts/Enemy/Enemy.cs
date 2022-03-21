using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour, IPathTarget
{
    [SerializeField] RoomInfo roomInfo;
    [SerializeField] Health health;
    [SerializeField] Collider2D collider2D;
    public Transform TargetTransform => transform;

    public System.Action<EnemyState, IEnemyCombatTarget> SetEnemyState;

    private bool dead = false;

    private void OnEnable()
    {
        health.Die += OnDie;
    }

    private void OnDisable()
    {
        health.Die += OnDie;
    }
    private void OnDie()
    {
        SetEnemyState?.Invoke(EnemyState.Dead, null);
        collider2D.enabled = false;
        Destroy(this);
    }

    private void FixedUpdate()
    {
        if (dead)
            return;

        List<IEnemyCombatTarget> targets = roomInfo.Room.EnemyCombatTargets;

        if (targets.Count > 0)
        {
            IEnumerable<IEnemyCombatTarget> alive = targets.Where(t => !t.IsNull);

            if (alive.Count() > 0)
            {
                IEnemyCombatTarget current = alive.OrderBy(t => Vector2.Distance(transform.position, t.Position)).First();
                SetEnemyState?.Invoke(EnemyState.Attack, current);
                return;
            }
        }

        SetEnemyState?.Invoke(EnemyState.Idle, null);
    }
}
public enum EnemyState
{
    Idle,
    Attack,
    Walk,
    Dead,
}