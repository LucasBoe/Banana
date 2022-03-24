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
        portalUser.ChangeRoom += OnChangeRoom;
        health.Die += OnDie;
    }

    private void OnDisable()
    {
        combatModule.Disable();
        combatModule.FoundCombatTarget -= EnterCombat;
        portalUser.ChangeRoom -= OnChangeRoom;
        health.Die -= OnDie;
    }

    internal void ReleaseFromCage()
    {
        targetModule.ReleaseFromCage();
    }

    private void Awake()
    {
        Cage cage = GetComponentInParent<Cage>();

        if (cage != null)
        {
            targetModule.SetCage(cage);
            SetState(HelperState.Idle);
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
        if (state == newState) return;
        state = newState;
        ChangedState?.Invoke(newState);
    }
    private void FixedUpdate()
    {
        if (!combatModule.HasCombatTarget)
        {
            UpdateTarget(UpdateTargetMode.Try);
            moveModule.Update();
        }

        combatModule.Update();
    }

    private void UpdateTarget(UpdateTargetMode mode)
    {
        if (mode == UpdateTargetMode.Try && !targetModule.ShouldUpdate) return;

        if (mode == UpdateTargetMode.Force)
            moveModule.StopMoving();

        Transform target = targetModule.GetCurrentTarget();

        if (target != null)
        {
            SetState(HelperState.Walk);
            moveModule.SetMoveTarget(target);
        }
    }

    private void OnChangeRoom(Room from, Room to) => UpdateTarget(UpdateTargetMode.Force);

    private enum UpdateTargetMode
    {
        Try,
        Force
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

        private List<Vector2> path;
        private Pathrequest runningRequest;
        public bool IsMoving => path != null && path.Count > 0;

        public int pathUpdateCounter = 0;

        internal void SetMoveTarget(Transform target)
        {
            if (!IsMoving)
                StartMove?.Invoke();

            if (runningRequest == null)
                RequestNewPath(target);
        }

        private void RequestNewPath(Transform target)
        {
            if (runningRequest != null) return;

            pathUpdateCounter++;
            runningRequest = Pathfinder.Instance.GetPathTo(transform.position, target.position, roomInfo.Room, new Collider2D[] { ownColliderToIgnoreForPathfinding });
            runningRequest.Resolved += OnResolvedPathRequest;
            
        }

        private void OnResolvedPathRequest(Pathrequest request, List<Vector2> newPath)
        {
            request.Resolved -= OnResolvedPathRequest;
            runningRequest = null;

            if (newPath.Count > 0)            
                path = newPath;
        }

        public void StopMoving()
        {
            path = null;
            rigidbody.velocity = Vector2.zero;
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
        [SerializeField] Transform transform;
        [SerializeField] RoomInfo roomInfo;
        public bool ShouldUpdate => cageIsOpen && lastTargetUpdateTime + 0.5f < Time.time && (target == null || !target.IsAlive || Vector2.Distance(transform.position, target.TargetTransform.position) > 0.5f);

        private bool isInCage = false;
        private bool cageIsOpen = true;

        public Cage Cage;
        private IHelperPathTarget target;

        float lastTargetUpdateTime = 0;

        public void SetCage(Cage cage)
        {
            cageIsOpen = false;
            isInCage = true;
            Cage = cage;
        }

        public void ReleaseFromCage()
        {
            cageIsOpen = true;
        }

        public Transform GetCurrentTarget()
        {
            if (isInCage)
            {
                isInCage = false;
                return Cage.TargetTransform;
            }

            lastTargetUpdateTime = Time.time;

            if (roomInfo.Room.Enemys.Count > 0 && roomInfo.Room.Enemys[0].IsAlive)
            {
                target = roomInfo.Room.Enemys[0];
            }
            else
            {
                target = EnemyManager.Instance.GetEnemy();
            }

            if (target == null)
            {
                target = PlayerManager.Player;
            }

            if (target != null && target.IsAlive)
            {
                if (roomInfo.Room.IsInside(target.TargetTransform.position))
                {
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
        List<IHelperCombatTarget> targets = new List<IHelperCombatTarget>();
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
            IHelperCombatTarget enemy = collider.GetComponent<IHelperCombatTarget>();
            if (enemy != null)
            {
                if (targets.Count == 0)
                    FoundCombatTarget?.Invoke();
                targets.AddUnique(enemy);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            IHelperCombatTarget enemy = collider.GetComponent<IHelperCombatTarget>();
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

            transform.up = (targets[0].Position - transform.position).normalized;
        }
    }
}
