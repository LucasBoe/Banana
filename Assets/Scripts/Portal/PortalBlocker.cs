using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PortalBlocker : MonoBehaviour, IHelperCombatTarget
{
    [SerializeField] Collider2D collider;
    [SerializeField] List<GameObject> planks;

    public Vector3 Position => transform.position;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckForDamage(collision.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckForDamage(collision.gameObject);
    }
    private void CheckForDamage(GameObject gameObject)
    {
        if (gameObject.CompareTag("DamageHelper") || gameObject.CompareTag("DamageEnemy"))
            Hit();
    }

    private void Hit()
    {
        GameObject plank = planks[0];
        planks.Remove(plank);

        Rigidbody rb = plank.AddComponent<Rigidbody>();
        rb.AddForceAtPosition(-transform.up * Random.Range(50f, 200f), transform.position + transform.right * Random.Range(0f, 1f));

        if (planks.Count == 0)
        {
            Destroy(collider);
            Destroy(this);
        }
    }
}
