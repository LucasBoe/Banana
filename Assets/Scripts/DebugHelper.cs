using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    [SerializeField] RoomInfo prefabToSpawn;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F4))
        {
            RoomInfo instance = Instantiate(prefabToSpawn, PlayerManager.Player.transform.position, Quaternion.identity);
            instance.Room = PlayerManager.Room;
        }    
    }
}
