using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class SpawnObjectOnDeath : MonoBehaviour
{
    [SerializeField] GameObject toSpawn;
    Health health;
    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.Die += SpawnObject;
    }
    private void OnDisable()
    {
        health.Die -= SpawnObject;
    }

    private void SpawnObject()
    {
        health.Die -= SpawnObject;
        Instantiate(toSpawn, transform.position, Quaternion.identity);
    }
}
