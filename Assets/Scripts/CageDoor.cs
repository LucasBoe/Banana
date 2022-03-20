using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class CageDoor : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Cage cage;

    private void OnEnable()
    {
        cage.Open += OnOpenCage;
    }

    private void OnDisable()
    {
        cage.Open -= OnOpenCage;
    }
    private void OnOpenCage()
    {
        rigidbody.AddForceAtPosition(-transform.up * Random.Range(10, 100), transform.position - Vector3.forward - transform.right);
    }
}
