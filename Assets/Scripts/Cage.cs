using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour
{
    [SerializeField] RoomInfo roomInfo;
    [SerializeField] Helper contains;

    public System.Action Open;
    bool active = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (active)
            return;

        if (collision.collider.IsPlayer())
            OpenCage();
        
    }

    private void OpenCage()
    {
        active = true;
        contains.transform.parent = roomInfo.Room.transform;
        contains.Free();
        Open?.Invoke();
    }
}
