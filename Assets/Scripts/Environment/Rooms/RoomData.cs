using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EHUtilities;

[CreateAssetMenu(fileName = "RoomData", menuName = "ScriptableObjects/Rooms/RoomData", order = 1)]
[SerializeField]
public class RoomData : ScriptableObject
{
    [Tooltip("The sceen that will be loaded associated with this room data")]
    public SceneField RoomScene;
    public bool bIsRoomDiscovered;
    public DoorData[] ConnnectedDoors;

    /// <summary>
    /// Returns whether or not the door that is passed in is contained within our room
    /// </summary>
    /// <param name="Door"></param>
    /// <returns></returns>
    public bool DoesRoomContainDoor(DoorData Door)
    {
        foreach (DoorData RoomDoor in ConnnectedDoors)
        {
            if (RoomDoor == Door)
            {
                return true;
            }
        }
        return false;
    }
}
