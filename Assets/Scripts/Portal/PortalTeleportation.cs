using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleportation
{
    private PortalUser portalUser;
    private Portal from;
    private Portal to;
    private MaterialInstantiator mat;

    bool crossedPointOfNoReturn => teleportationProgress > 0.25f;
    public System.Action OnExit;

    float teleportationProgress = 0;

    public PortalTeleportation(PortalUser portalUser, Portal from, Portal to, MaterialInstantiator mat)
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
            teleportationProgress += Time.fixedDeltaTime * 2f;
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
        mat.Material.SetVector("portalPositionOffset", toUser);
        mat.Material.SetFloat("dissolve", teleportationProgress);
    }

    private void Teleport()
    {
        portalUser.Teleport(from, to);
    }
}
