using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTeleportListener : MonoBehaviour
{
    [SerializeField] PortalUser player;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    private void OnEnable()
    {
        player.TeleportingPlayer += OnTeleportingPlayer;
    }

    private void OnDisable()
    {
        player.TeleportingPlayer += OnTeleportingPlayer;
    }

    private void OnTeleportingPlayer(Transform player, Vector3 positionDelta)
    {
        virtualCamera.OnTargetObjectWarped(player, positionDelta);
    }
}
