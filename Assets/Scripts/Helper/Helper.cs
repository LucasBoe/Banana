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
        combatModule.HasCombatTarget += EnterCombat;
        combatModule.HasNoCombatTarget += MoveToNewTarget;
        portalUser.TeleportFinished += MoveToNewTarget;
        health.Die += OnDie;
    }

    private void OnDisable()
    {
        combatModule.Disable();
        combatModule.HasCombatTarget -= EnterCombat;
        combatModule.HasNoCombatTarget -= MoveToNewTarget;
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
            });
            SetState(HelperState.Walk);
        }
    }
    private void EnterCombat()
    {
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

    [System.Serializable]
    public class CombatModule
    {
        List<Enemy> targets = new List<Enemy>();
        [SerializeField] HelperCombatTrigger trigger;
        [SerializeField] Transform transform;

        public System.Action HasCombatTarget, HasNoCombatTarget;

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
                    HasCombatTarget?.Invoke();
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
                    HasNoCombatTarget?.Invoke();
            }
        }

        public void Update()
        {
            if (targets.Count == 0) return;

            transform.up = (targets[0].transform.position - transform.position).normalized;
        }
    }
}
