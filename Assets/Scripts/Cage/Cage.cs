using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour, IHelperPathTarget
{
    [SerializeField] RoomInfo roomInfo;
    [SerializeField] Helper contains;
    [SerializeField] Collider2D toDisable;
    [SerializeField] GameObject toEnable;
    [SerializeField] Transform leaveTransform;

    public System.Action Open;
    bool active = false;

    public Transform TargetTransform => leaveTransform;
    public Room Room => roomInfo.Room;

    public bool IsAlive => true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (active)
            return;

        if (collision.collider.IsPlayer())
            OpenCage();
        
    }

    private void OpenCage()
    {
        toDisable.enabled = false;
        toEnable.SetActive(true);
        active = true;
        contains.transform.parent = roomInfo.Room.transform;
        contains.ReleaseFromCage();
        Open?.Invoke();
    }
}
