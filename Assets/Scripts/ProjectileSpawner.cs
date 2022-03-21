using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] RoomInfo roomInfo;
    [SerializeField] Projectile prefab;
    [SerializeField] Transform target;
    [SerializeField] float intervall = 1f;
    private void Start()
    {
        StartCoroutine(SpawningRoutine());
    }

    private IEnumerator SpawningRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(intervall);
        while (target != null)
        {
            yield return wait;
            Vector2 dir = (target.position - transform.position).normalized;
            Instantiate(prefab, transform.position, Quaternion.identity).Shoot(roomInfo.Room, dir);
        }
    }
}
