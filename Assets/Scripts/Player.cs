using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerMoveModule moveModule;
    [SerializeField] PlayerRoomModule roomModule;
    [SerializeField] PlayerInputModule inputModule;
    [SerializeField] PlayerAnimationModule animationModule;

    void Awake()
    {
        moveModule.Init(gameObject);
    }

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        Vector2 position = transform.position;
        Vector2 input = inputModule.GetInput();
        Vector2 moveTarget = roomModule.GetClosestPointInRoom(position, position + input * Time.fixedDeltaTime * 3f);
        moveModule.MoveTo(moveTarget);
        if (moveTarget != position)
            animationModule.Animate(input, Time.fixedDeltaTime);
    }
}

public abstract class PlayerModule
{
    protected GameObject gameObject;
    public virtual void Init(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
}

[System.Serializable]
public class PlayerInputModule : PlayerModule
{
    public Vector2 GetInput()
    {
        return (Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up).normalized;
    }
}

[System.Serializable]
public class PlayerMoveModule : PlayerModule
{
    [SerializeField] Rigidbody2D rigidbody2D;

    internal void MoveTo(Vector2 moveTarget)
    {
        rigidbody2D.MovePosition(moveTarget);
    }
}

[System.Serializable]
public class PlayerRoomModule : PlayerModule
{
    [SerializeField] Room Current;

    public bool IsInsideRoom(Vector2 pos)
    {
        return Current.IsInside(pos);
    }

    internal Vector2 GetClosestPointInRoom(Vector2 origin, Vector2 change)
    {
        return Current.CorrectToRoomPerimeter(origin, change);
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
