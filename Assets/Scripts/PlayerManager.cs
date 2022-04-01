using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : SingletonBehaviour<PlayerManager>
{
    [SerializeField] Player player;
    [SerializeField] ManaManager playerMana;
    [SerializeField] RoomInfo playerRoomInfo;
    public static Player Player => Instance.player;
    public static Vector2 PlayerPos => Instance.player.transform.position;

    public static Room Room => Instance.playerRoomInfo.Room;
}
