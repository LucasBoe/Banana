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

    private void Awake()
    {
        Cage inCage = GetComponentInParent<Cage>();
        if (inCage == null) return;
    }

    private void OnEnable()
    {
        moveModule.StartMove += () => animatorModule.PlayState("walk");
        moveModule.StopMove += () => animatorModule.PlayState("idle");
    }

    private void OnDisable()
    {
        moveModule.StartMove -= () => animatorModule.PlayState("walk");
        moveModule.StopMove -= () => animatorModule.PlayState("idle");
    }

    private void Start()
    {
        Transform target = targetModule.GetTarget();
        if (target != null)
            moveModule.StartMoving(target, () => animatorModule.PlayState("attack"));
        else
            animatorModule.PlayState("cage");
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
        [SerializeField] Transform target;

        public Transform GetTarget()
        {
            return target;
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
