using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTeleportListener : MonoBehaviour
{
    [SerializeField] PortalUser player;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] float angleOffset = 0;
    [SerializeField] OffsetPairs[] pairs;

    private void OnEnable()
    {
        player.TeleportingPlayer += OnTeleportingPlayer;
    }

    private void OnDisable()
    {
        player.TeleportingPlayer += OnTeleportingPlayer;
    }

    private void OnTeleportingPlayer(Transform player, Vector3 positionDelta, float angleDifference)
    {
        virtualCamera.OnTargetObjectWarped(player, positionDelta);

        float angleCleaned = Mathf.Round(angleDifference / 90f) * 90f;
        angleOffset += angleCleaned;

        OffsetPairs pair = GetPairForAngle(angleOffset);


        virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = pair.Offset;
        virtualCamera.m_Lens.Dutch = pair.Dutch;
    }

    private OffsetPairs GetPairForAngle(float angle)
    {
        foreach (OffsetPairs pair in pairs)
        {
            if (Mathf.Abs(Mathf.DeltaAngle(pair.Angle, angle)) < 10f)
            {
                return pair;
            }
        }

        return null;
    }
}

[System.Serializable]
public class OffsetPairs
{
    public float Angle;
    public Vector3 Offset;
    public float Dutch;
}
