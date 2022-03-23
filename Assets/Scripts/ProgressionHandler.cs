using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionHandler : MonoBehaviour
{
    private void OnGUI()
    {
        if (!Application.isPlaying) return;

        int avaliablePortals = 0;
        int avaliablePortalTargetValue = 2;


        int avaliableHelper = -1;
        int avaliableHelperTargetValue = 3;

        foreach (Room room in RoomManager.Instance.Rooms)
        {
            foreach (Portal portal in room.Portals)
            {
                if (portal.Target == null && !portal.IsBlocked)
                    avaliablePortals++;
            }

            foreach (IEnemyCombatTarget helper in room.EnemyCombatTargets)
            {
                if (!helper.IsNull)
                    avaliableHelper++;
            }
        }

        GUILayout.Box("portals: " + avaliablePortals.ToString() + " => " + avaliablePortalTargetValue + "\n"
            + "helpers: " + avaliableHelper.ToString() + " => " + avaliableHelperTargetValue);
    }
}
