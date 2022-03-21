using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour, IPathTarget
{
    [SerializeField] RoomInfo roomInfo;
    public Transform TargetTransform => transform;


    public System.Action<EnemyState, IEnemyCombatTarget> SetEnemyState;

    private void FixedUpdate()
    {
        List<IEnemyCombatTarget> targets = roomInfo.Room.EnemyCombatTargets;

        if (targets.Count > 0)
        {
            IEnemyCombatTarget current = targets.OrderBy(t => Vector2.Distance(transform.position, t.Position)).First();
            SetEnemyState?.Invoke(EnemyState.Attack, current);
        } else
        {
            SetEnemyState?.Invoke(EnemyState.Idle, null);
        }
    }
}
public enum EnemyState
{
    Idle,
    Attack,
    Walk
}