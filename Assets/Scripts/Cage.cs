using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour
{
    public System.Action Open;
    bool active = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (active)
            return;

        if (collision.collider.IsPlayer())
        {
            active = true;
            Open?.Invoke();
        }
    }
}
