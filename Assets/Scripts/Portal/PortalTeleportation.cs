using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleportation
{
    private PortalUser portalUser;
    private Portal from;
    private Portal to;
    private Material mat;

    bool crossedPointOfNoReturn => teleportationProgress > 0.25f;
    public System.Action OnExit;

    float teleportationProgress = 0;

    public PortalTeleportation(PortalUser portalUser, Portal from, Portal to, Material mat)
    {
        this.portalUser = portalUser;
        this.from = from;
        this.to = to;
        this.mat = mat;
    }

    internal void TryAbort()
    {
        if (!crossedPointOfNoReturn)
            OnExit?.Invoke();
    }

    internal void Update()
    {
        if (crossedPointOfNoReturn)
        {
            teleportationProgress += Time.fixedDeltaTime;
            if (teleportationProgress >= 1f)
            {
                OnExit?.Invoke();
            }
        }
        else
        {
            teleportationProgress = from.transform.InverseTransformPoint(portalUser.transform.position).y;
            if (teleportationProgress > 0.25f)
                Teleport();
        }

        Vector2 world = crossedPointOfNoReturn ? to.TransformPointToTarget(portalUser.transform.position) : from.TransformPointToTarget(portalUser.transform.position);

        Util.DebugDrawCross(world, Color.red, 0.25f, Time.fixedDeltaTime);
        Vector2 toUser = world - (Vector2)portalUser.transform.position;
        mat.SetVector("portalPositionOffset", toUser);
        mat.SetFloat("dissolve", teleportationProgress);
    }

    private void Teleport()
    {
        Debug.Log("Teleport to " + to);

        Vector2 pos = portalUser.transform.position;
        portalUser.transform.position = from.TransformPointToTarget(pos);
    }
}
