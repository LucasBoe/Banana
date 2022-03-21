using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IDamager
{
    [SerializeField] Room room;
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] float initialVelocityMultiplier;
    [SerializeField] GameObject deathPrefab;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] float damage;

    ParticleSystem.ShapeModule shapeModule;
    ParticleSystem.EmissionModule emissionModule;

    Vector2 direction;
    bool active = false;
    public bool IsEnabled => active;

    public float Amount => damage;

    private void Awake()
    {
        shapeModule = particleSystem.shape;
        emissionModule = particleSystem.emission;
    }

    public void Shoot(Room room, Vector2 direction)
    {
        this.room = room;
        this.direction = direction;
        rigidbody2D.velocity = direction * initialVelocityMultiplier;
        active = true;
    }

    private void FixedUpdate()
    {
        if (room == null || !active) return;

        float velocityMagnitude = rigidbody2D.velocity.magnitude;

        float zOffset = GetZOffsetFromVelocityMagnitude(velocityMagnitude);
        shapeModule.position = new Vector3(0, 0, zOffset);

        if (!room.IsInside(transform.position) || velocityMagnitude < 0.5f)        
            Disable();
        
    }

    private float GetZOffsetFromVelocityMagnitude(float velocityMagnitude)
    {
        return (velocityMagnitude / initialVelocityMultiplier) * - 0.5f;
    }

    public void Disable()
    {
        active = false;
        Instantiate(deathPrefab, transform.position, Quaternion.Euler(0, 0, Vector2.Angle(Vector2.right, direction)));
        emissionModule.rateOverTimeMultiplier = 0;
        Destroy(rigidbody2D);
        Destroy(gameObject, 1);
    }
}
