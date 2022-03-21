using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelperModules;
using System;

public enum HelperState
{
    Idle,
    Walk,
    Attack,
    Dead,
}

public class Helper : MonoBehaviour, IEnemyCombatTarget
{
    [SerializeField] HelperMoveModule moveModule;
    [SerializeField] TargetModule targetModule;
    [SerializeField] PortalUser portalUser;
    [SerializeField] Health health;

    private HelperState state;
    public System.Action<HelperState> ChangedState;
    public Vector2 Position => transform.position;

    public bool IsNull => Equals(null);

    private void OnEnable()
    {
        portalUser.TeleportFinished += MoveToNewTarget;
        health.Die += OnDie;
    }

    private void OnDisable()
    {
        portalUser.TeleportFinished -= MoveToNewTarget;
        health.Die -= OnDie;
    }
    private void Awake()
    {
        Cage cage = GetComponentInParent<Cage>();

        if (cage != null)
        {
            targetModule.SetCage(cage);
            SetState(HelperState.Idle);
        }
        else
        {
            MoveToNewTarget();
        }
    }
    public void MoveToNewTarget()
    {
        Debug.LogWarning("MoveToNewTarget...");

        Transform target = targetModule.GetTarget();
        if (target != null)
        {
            moveModule.StartMoving(target, () =>
            {
                if (!targetModule.IsFinalTarget)
                    MoveToNewTarget();
                else
                    SetState(HelperState.Attack);
            });
            SetState(HelperState.Walk);
        }
    }
    private void OnDie()
    {
        moveModule.DisableMovementAndCollsion();
        SetState(HelperState.Dead);
        Destroy(this);
    }

    private void SetState(HelperState newState)
    {
        state = newState;
        ChangedState?.Invoke(newState);
    }
    private void FixedUpdate() => moveModule.Update();

}

namespace HelperModules
{
    [System.Serializable]
    public class HelperMoveModule
    {
        [SerializeField] RoomInfo roomInfo;
        [SerializeField] Transform transform;
        [SerializeField] Rigidbody2D rigidbody;
        [SerializeField] float speed;
        [SerializeField] Collider2D ownColliderToIgnoreForPathfinding;

        public System.Action StartMove;
        public System.Action StopMove;

        private System.Action targetReachedCallback;
        private List<Vector2> path;

        public void StartMoving(Transform target, System.Action targetReachedCallback)
        {
            path = Pathfinder.Instance.GetPathTo(transform.position, target.position, roomInfo.Room, new Collider2D[] { ownColliderToIgnoreForPathfinding });
            StartMove?.Invoke();
            this.targetReachedCallback = targetReachedCallback;
        }

        public void StopMoving()
        {
            path = null;
            rigidbody.velocity = Vector2.zero;
            StopMove?.Invoke();
            targetReachedCallback?.Invoke();
        }

        public void Update()
        {
            if (path != null)
            {
                if (path.Count == 0)
                    StopMoving();
                else
                {
                    Vector2 dir = path[0] - (Vector2)transform.position;
                    Vector2 vel = (dir).normalized * speed;
                    rigidbody.velocity = vel;
                    transform.up = rigidbody.velocity.normalized;

                    if (dir.magnitude < 0.25f)
                        path.RemoveAt(0);
                }
            }
            else
            {
                rigidbody.velocity = Vector2.zero;
            }
        }

        internal void DisableMovementAndCollsion()
        {
            StopMoving();
            rigidbody.simulated = false;
            ownColliderToIgnoreForPathfinding.enabled = false;
        }
    }

    [System.Serializable]
    public class TargetModule
    {
        [SerializeField] Enemy target;
        [SerializeField] RoomInfo roomInfo;

        public bool IsFinalTarget => isFinalTarget;

        private bool isInCage = false;
        private bool isFinalTarget = false;
        public Cage Cage;

        public void SetCage(Cage cage)
        {
            isInCage = true;
            Cage = cage;
        }

        public Transform GetTarget()
        {
            if (target == null)
                return null;

            if (isInCage)
            {
                isInCage = false;
                return Cage.TargetTransform;
            }

            if (roomInfo.Room.IsInside(target.TargetTransform.position))
            {
                isFinalTarget = true;
                return target.TargetTransform;
            }
            else
            {
                bool roomHasPortal = roomInfo.Room.Portals.Count > 0;
                if (roomHasPortal)
                    return roomInfo.Room.Portals[0].TargetTransform;
            }

            return null;
        }
    }
}
