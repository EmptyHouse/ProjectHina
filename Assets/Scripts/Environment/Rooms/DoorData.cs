using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoorData", menuName = "ScriptableObjects/Rooms/DoorData", order = 1)]
public class DoorData : ScriptableObject
{
    [SerializeField]
    private RoomData ConnectedRoom;
    [SerializeField]
    private DoorData ConnectedDoor;

    /// <summary>
    /// Returns the room that is associated with this door
    /// </summary>
    /// <returns></returns>
    public RoomData GetDoorRoom()
    {
        return ConnectedRoom;
    }

    /// <summary>
    /// Returns the door data that is connected to this door
    /// </summary>
    /// <returns></returns>
    public DoorData GetConnectedDoorData()
    {
        return ConnectedDoor;
    }
}
