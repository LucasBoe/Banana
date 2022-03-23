using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, IPathTarget
{
    [SerializeField] public Transform TeleportPosition;
    [SerializeField] public Portal Target;
    [SerializeField] RoomInfo roomInfo;
    [SerializeField] PortalBlocker blocker;
    [SerializeField] Transform referencePosition;

    public bool IsBlocked => blocker != null && blocker.isActiveAndEnabled;
    public System.Action Teleported;
    public Room Room => roomInfo.Room;

    public Transform TargetTransform => TeleportPosition;

    private Dictionary<PortalUser, PortalTeleportation> teleportations = new Dictionary<PortalUser, PortalTeleportation>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PortalUser user = collision.GetComponent<PortalUser>();

        if (user == null || user.IsTeleporting) return;

        if (Target == null)
            Target = RoomManager.Instance.GetNewRoomsPortal(this);

        if (Target == null) return;



        PortalTeleportation teleportation = user.StartTeleportation(this, Target);
        teleportations.Add(user, teleportation);
        teleportation.OnExit += () => teleportations.Remove(user);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PortalUser user = collision.GetComponent<PortalUser>();

        if (user == null) return;

        if (teleportations.ContainsKey(user))
        {
            teleportations[user].TryAbort();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (Target == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.back * 0.5f, Target.transform.position + Vector3.back * 0.5f);
    }

    private void OnDrawGizmos()
    {
        if (referencePosition == null) return;

        Gizmos.DrawWireSphere(
        TransformPointToTarget(referencePosition.position), 0.1f);
    }

    public Vector2 TransformPointToTarget (Vector2 worldPoint)
    {
        Vector2 ownLocal = transform.InverseTransformPoint(worldPoint);
        Vector2 otherWorld = Target.transform.TransformPoint(-ownLocal);
        return otherWorld;
    } 

}
