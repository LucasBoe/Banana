using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerModules;

public class Player : MonoBehaviour, IEnemyCombatTarget, IHelperPathTarget
{
    [SerializeField] PlayerMoveModule moveModule;
    [SerializeField] PlayerRoomModule roomModule;
    [SerializeField] PlayerInputModule inputModule;
    [SerializeField] PlayerAnimationModule animationModule;

    [SerializeField] PortalUser portalUser;
    [SerializeField] public Transform followerTransform;

    public Vector2 Position => transform.position;
    public bool IsNull => Equals(null);
    public bool IsAlive => !IsNull;
    public Transform TargetTransform => followerTransform;
    public Room Room => roomModule.Room;


    private void OnEnable()
    {
        portalUser.TeleportingPlayer += inputModule.OnTeleportingPlayer;
    }

    private void OnDisable()
    {
        portalUser.TeleportingPlayer -= inputModule.OnTeleportingPlayer;
    }

    private void FixedUpdate()
    {
        Vector2 position = transform.position;
        Vector2 input = inputModule.GetInput();
        Vector2 moveVector = moveModule.GetMoveVectorFromInput(input);

        Vector2 targetPosition = roomModule.GetClosestPointInRoom(position, moveVector);

        if (targetPosition != position)
            moveModule.MoveTo(targetPosition);

        animationModule.Animate(input);

    }
}

namespace PlayerModules
{

    public abstract class PlayerModule { }

    [System.Serializable]
    public class PlayerInputModule : PlayerModule
    {
        float angleOffset = 0;

        [SerializeField] Vector2 input;
        public Vector2 GetInput()
        {
            input = (Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up).Rotate(angleOffset);
            return input;
        }

        internal void OnTeleportingPlayer(Transform transform, Vector3 positionDelta, float angleOffsetDelta)
        {
            angleOffset += angleOffsetDelta;
        }
    }

    [System.Serializable]
    public class PlayerMoveModule : PlayerModule
    {
        [SerializeField] Rigidbody2D rigidbody2D;
        [SerializeField] float speed = 3.5f;
        public float Speed => speed;

        internal Vector2 GetMoveVectorFromInput(Vector2 input)
        {
            return input * Time.fixedDeltaTime * speed;
        }

        internal void MoveTo(Vector2 moveTarget)
        {
            float targetAngle = Vector2.SignedAngle(Vector2.up, (moveTarget - rigidbody2D.position).normalized);
            float currentAngle = rigidbody2D.rotation;
            rigidbody2D.MoveRotation(Mathf.MoveTowardsAngle(currentAngle, targetAngle, Time.fixedDeltaTime * 360f));
            rigidbody2D.MovePosition(moveTarget);
        }
    }

    [System.Serializable]
    public class PlayerRoomModule : PlayerModule
    {
        [SerializeField] RoomInfo roomInfo;

        public Room Room => roomInfo.Room;

        public bool IsInsideRoom(Vector2 pos)
        {
            return roomInfo.Room.IsInside(pos);
        }

        public Vector2 GetClosestPointInRoom(Vector2 origin, Vector2 change)
        {
            return roomInfo.Room.AdaptVector2ToRoomPerimeter(origin, origin + change);
        }
    }

    [System.Serializable]
    public class PlayerAnimationModule : PlayerModule
    {
        [SerializeField] Animator animator;
        internal void Animate(Vector2 input)
        {
            animator.SetFloat("speed", input.magnitude);
        }
    }
}
