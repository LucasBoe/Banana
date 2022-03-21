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
        [SerializeField] Transform transform;
        [SerializeField] Rigidbody2D rigidbody;
        [SerializeField] Transform target;
        [SerializeField] float speed;

        public System.Action StartMove;
        public System.Action StopMove;
        private System.Action targetReachedCallback;

        public void StartMoving(Transform target, System.Action targetReachedCallback)
        {
            this.target = target;
            StartMove?.Invoke();
            this.targetReachedCallback = targetReachedCallback;
        }

        public void StopMoving()
        {
            this.target = null;
            StopMove?.Invoke();
            targetReachedCallback?.Invoke();
        }

        public void Update()
        {
            if (target != null)
            {
                Vector2 dir = target.position - transform.position;
                Vector2 vel = (dir).normalized * speed;
                rigidbody.velocity = vel;
                transform.up = rigidbody.velocity.normalized;

                if (dir.magnitude < 0.5f)
                    StopMoving();
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

            Debug.Log("Room is: " + roomInfo.Room);

            bool targetIsInsideSameRoom = roomInfo.Room.IsInside(target.transform.position);

            IsFinalTarget = targetIsInsideSameRoom;

            if (targetIsInsideSameRoom)
                return target.transform;

            bool roomHasPortal = roomInfo.Room.Portals.Count > 0;

            if (roomHasPortal)
                return roomInfo.Room.Portals[0].transform;

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
