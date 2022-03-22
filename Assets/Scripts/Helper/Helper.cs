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
    [SerializeField] CombatModule combatModule;
    [SerializeField] Health health;

    private HelperState state;
    public System.Action<HelperState> ChangedState;
    public Vector2 Position => transform.position;

    public bool IsNull => Equals(null);

    private void OnEnable()
    {
        combatModule.Enable();
        combatModule.FoundCombatTarget += EnterCombat;
        combatModule.LostAllCombatTargets += MoveToNewTarget;
        portalUser.TeleportFinished += MoveToNewTarget;
        moveModule.StopMove += OnTargetReached;
        health.Die += OnDie;
    }

    private void OnDisable()
    {
        combatModule.Disable();
        combatModule.FoundCombatTarget -= EnterCombat;
        combatModule.LostAllCombatTargets -= MoveToNewTarget;
        portalUser.TeleportFinished -= MoveToNewTarget;
        moveModule.StopMove += OnTargetReached;
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
    private void OnTargetReached()
    {
        if (state != HelperState.Attack && !moveModule.IsMoving)
            MoveToNewTarget();
    }

    public void MoveToNewTarget()
    {
        Transform target = targetModule.GetTarget();

        if (target != null)
        {
            moveModule.StartMoving(target, () =>
            {
                if (!targetModule.IsFinalTarget && state != HelperState.Attack)
                    MoveToNewTarget();
            });
            SetState(HelperState.Walk);
        }
    }
    private void EnterCombat()
    {
        moveModule.StopMoving();
        SetState(HelperState.Attack);
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
    private void FixedUpdate()
    {
        if (!combatModule.HasCombatTarget)
            moveModule.Update();
        combatModule.Update();
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

        [SerializeField] bool isMoving;

        public System.Action StartMove;
        public System.Action StopMove;

        private System.Action targetReachedCallback;
        private List<Vector2> path;
        public bool IsMoving => path != null && path.Count > 0;

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
            targetReachedCallback?.Invoke();
            StopMove?.Invoke();
        }

        public void Update()
        {
            isMoving = false;
            if (path != null)
            {
                if (path.Count == 0)
                    StopMoving();
                else
                {
                    isMoving = true;
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
        [SerializeField] RoomInfo roomInfo;

        public bool IsFinalTarget => isFinalTarget;

        private bool isInCage = false;
        private bool isFinalTarget = false;
        public Cage Cage;
        private Enemy target;

        public void SetCage(Cage cage)
        {
            isInCage = true;
            Cage = cage;
        }

        public Transform GetTarget()
        {
            if (isInCage)
            {
                isInCage = false;
                return Cage.TargetTransform;
            }

            target = EnemyManager.Instance.GetEnemy();

            if (target != null)
            {
                if (roomInfo.Room.IsInside(target.TargetTransform.position))
                {
                    isFinalTarget = true;
                    return target.TargetTransform;
                }
                else
                {
                    Portal portalToTarget = RoomManager.Instance.GetPortalThatLeadsTo(roomInfo.Room, target.Room);
                    if (portalToTarget != null)
                        return portalToTarget.TargetTransform;
                }
            }

            return roomInfo.Room.GetRandomPoint();
        }
    }

    [System.Serializable]
    public class CombatModule
    {
        List<Enemy> targets = new List<Enemy>();
        [SerializeField] HelperCombatTrigger trigger;
        [SerializeField] Transform transform;

        public System.Action FoundCombatTarget, LostAllCombatTargets;
        public bool HasCombatTarget => targets.Count > 0;

        internal void Enable()
        {
            trigger.TriggerEnter2D += OnTriggerEnter2D;
            trigger.TriggerExit2D += OnTriggerExit2D;
        }
        internal void Disable()
        {
            trigger.TriggerEnter2D -= OnTriggerEnter2D;
            trigger.TriggerExit2D -= OnTriggerExit2D;
        }
        private void OnTriggerEnter2D(Collider2D collider)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (targets.Count == 0)
                    FoundCombatTarget?.Invoke();
                targets.AddUnique(enemy);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null && targets.Contains(enemy))
            {
                targets.Remove(enemy);

                if (targets.Count == 0)
                    LostAllCombatTargets?.Invoke();
            }
        }

        public void Update()
        {
            if (targets.Count == 0) return;

            transform.up = (targets[0].transform.position - transform.position).normalized;
        }
    }
}
