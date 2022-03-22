using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PortalBlocker : MonoBehaviour
{
    [SerializeField] Collider2D collider;
    [SerializeField] List<GameObject> planks;
    // Start is called before the first frame update

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DamageHelper"))
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
