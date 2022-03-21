using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerModules;
public class Player : MonoBehaviour, IEnemyCombatTarget
{
    [SerializeField] PlayerMoveModule moveModule;
    [SerializeField] PlayerRoomModule roomModule;
    [SerializeField] PlayerInputModule inputModule;
    [SerializeField] PlayerAnimationModule animationModule;

    public Vector2 Position => transform.position;

    private void FixedUpdate()
    {
        Vector2 position = transform.position;
        Vector2 input = inputModule.GetInput();
        Vector2 moveVector = moveModule.GetMoveVectorFromInput(input);

        Vector2 targetPosition = roomModule.GetClosestPointInRoom(position, moveVector);
        moveModule.MoveTo(targetPosition);

        if (targetPosition != position)
            animationModule.Animate(input, Time.fixedDeltaTime);
    }
}

namespace PlayerModules
{

    public abstract class PlayerModule { }

    [System.Serializable]
    public class PlayerInputModule : PlayerModule
    {
        [SerializeField] Vector2 input;
        public Vector2 GetInput()
        {
            input = (Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up).normalized;
            return input;
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
            rigidbody2D.MovePosition(moveTarget);
        }
    }

    [System.Serializable]
    public class PlayerRoomModule : PlayerModule
    {
        [SerializeField] RoomInfo roomInfo;

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
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] PlayerAnimationStrip down, left, right, up;
        float time = 0;

        internal void Animate(Vector2 input, float fixedDeltaTime)
        {
            time += fixedDeltaTime;
            PlayerAnimationStrip strip = GetStripFromInput(input);
            spriteRenderer.sprite = strip.Sprites[Mathf.RoundToInt((time * 12f) % (strip.Sprites.Length - 1))];
        }

        private PlayerAnimationStrip GetStripFromInput(Vector2 input)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                return input.x < 0 ? left : right;
            else
                return input.y < 0 ? down : up;
        }

        [System.Serializable]
        public class PlayerAnimationStrip
        {
            public Sprite[] Sprites;
        }
    }
}
