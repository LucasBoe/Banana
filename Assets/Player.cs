using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerMoveModule moveModule;
    void Awake()
    {
        moveModule = new PlayerMoveModule(gameObject);
    }

    private void Start()
    {
        
    }

    void Update()
    {
        moveModule.Update();
    }
}

[System.Serializable]
public class PlayerMoveModule
{
    GameObject gameObject;
    Rigidbody2D rigidbody2D;
    public PlayerMoveModule(GameObject playerObject)
    {
        gameObject = playerObject;
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    internal void Update()
    {
        rigidbody2D.velocity = (Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up).normalized * 5f;
    }
} 
