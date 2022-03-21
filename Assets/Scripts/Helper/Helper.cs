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
}

public class Helper : MonoBehaviour
{
    [SerializeField] HelperMoveModule moveModule;
    [SerializeField] TargetModule targetModule;
    [SerializeField] PortalUser portalUser;

    private HelperState state;
    public System.Action<HelperState> ChangedState;
    private void OnEnable()
    {
        portalUser.TeleportFinished += MoveToNewTarget;
    }

    private void OnDisable()
    {
        portalUser.TeleportFinished += MoveToNewTarget;
    }
    private void Awake()
    {
        Cage cage = GetComponentInParent<Cage>();

        if (cage != null)
        {
            targetModule.Cage = cage;
            targetModule.IsInCage = true;
            SetState(HelperState.Idle);
        }
        else
        {
            MoveToNewTarget();
        }
    }
    public void MoveToNewTarget()
    {
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
    }

    [System.Serializable]
    public class TargetModule
    {
        [SerializeField] Enemy target;
        [SerializeField] RoomInfo roomInfo;

        public bool IsInCage = false;
        public bool IsFinalTarget = false;
        public Cage Cage;

        public Transform GetTarget()
        {
            if (target == null) return null;

            if (IsInCage)
            {
                IsInCage = false;
                IsFinalTarget = false;
                return Cage.TargetTransform;
            }

            bool targetIsInsideSameRoom = roomInfo.Room.IsInside(target.TargetTransform.position);

            IsFinalTarget = targetIsInsideSameRoom;

            if (targetIsInsideSameRoom)
                return target.TargetTransform;

            bool roomHasPortal = roomInfo.Room.Portals.Count > 0;

            if (roomHasPortal)
            {
                Transform portalTransform = roomInfo.Room.Portals[0].TargetTransform;

                Debug.Log("move to portal: " + portalTransform);
                Util.DebugDrawCross(portalTransform.position, Color.red, 0.5f, lifetime: 5f);

                return portalTransform;
            }

            return null;
        }
    }
}
