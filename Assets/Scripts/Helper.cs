using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelperModules;
using System;

public class Helper : MonoBehaviour
{
    [SerializeField] HelperMoveModule moveModule;
    [SerializeField] TargetModule targetModule;
    [SerializeField] AnimatorModule animatorModule;
    [SerializeField] PortalUser portalUser;

    private void Awake()
    {
        Cage inCage = GetComponentInParent<Cage>();
        if (inCage == null) return;
    }

    private void OnEnable()
    {
        moveModule.StartMove += () => animatorModule.PlayState("walk");
        moveModule.StopMove += () => animatorModule.PlayState("idle");
        portalUser.TeleportFinished += OnTeleportFinished;
    }

    private void OnDisable()
    {
        moveModule.StartMove -= () => animatorModule.PlayState("walk");
        moveModule.StopMove -= () => animatorModule.PlayState("idle");
        portalUser.TeleportFinished += OnTeleportFinished;
    }

    private void Start()
    {
        animatorModule.PlayState("cage");
    }
    public void Free()
    {
        Transform target = targetModule.GetTarget();
        if (target != null)
            moveModule.StartMoving(target, ReachedTarget);
    }
    public void ReachedTarget()
    {
        if (targetModule.IsFinalTarget)
            animatorModule.PlayState("attack");
    }
    private void OnTeleportFinished(Room room)
    {
        Transform target = targetModule.GetTarget();
        if (target != null)
            moveModule.StartMoving(target, ReachedTarget);
    }

    private void FixedUpdate()
    {
        moveModule.Update();
    }
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
            this.path = null;
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

        public bool IsFinalTarget = false;

        public Transform GetTarget()
        {
            if (target == null) return null;

            bool targetIsInsideSameRoom = roomInfo.Room.IsInside(target.TargetTransform.position);

            IsFinalTarget = targetIsInsideSameRoom;

            if (targetIsInsideSameRoom)
                return target.TargetTransform;

            bool roomHasPortal = roomInfo.Room.Portals.Count > 0;

            if (roomHasPortal)
                return roomInfo.Room.Portals[0].TargetTransform;

            return null;
        }
    }

    [System.Serializable]
    public class AnimatorModule
    {
        [SerializeField] Animator animator;

        public void PlayState(string statename)
        {
            Debug.Log("Play " + statename);

            animator.Play(statename);
        }
    }
}
