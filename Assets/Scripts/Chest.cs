using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] Animator animator;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.enabled = true;
    }
}
