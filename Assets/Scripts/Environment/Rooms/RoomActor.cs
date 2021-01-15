using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Be sure to include this into every room with the associated RoomData uasset
/// </summary>
public class RoomActor : MonoBehaviour
{
    [SerializeField]
    private RoomData AssociatedRoomData = null;
    private List<RoomDoorTrigger> DoorTriggers = new List<RoomDoorTrigger>();

    #region monobehaviour methods
    private void Awake()
    {
        BaseGameOverseer.Instance.CurrentlyLoadedRoom = this;
    }
    #endregion monobeahviour methods

    public RoomData GetAssociatedRoomData() { return AssociatedRoomData; }
    
    public RoomDoorTrigger GetRoomDoorTriggerFromDoorData(DoorData Door)
    {
        foreach (RoomDoorTrigger Room in DoorTriggers)
        {
            //TO-DO Search for doors and select the correct actor to spawn from
        }
        return null;
    }

    
}
