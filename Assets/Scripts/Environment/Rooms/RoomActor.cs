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
    private HashSet<DoorActor> DoorTriggers = new HashSet<DoorActor>();

    #region monobehaviour methods
    private void Awake()
    {
        BaseGameOverseer.Instance.CurrentlyLoadedRoom = this;
    }
    #endregion monobeahviour methods

    public RoomData GetAssociatedRoomData() { return AssociatedRoomData; }
    
    public DoorActor GetRoomDoorTriggerFromDoorData(DoorData Door)
    {
        foreach (DoorActor DoorActor in DoorTriggers)
        {
            if (DoorActor.GetAssociatedDoorData() == Door)
            {
                return DoorActor;
            }
        }
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Door"></param>
    public void AddDoorActor(DoorActor Door)
    {
        if (Door == null)
        {
            Debug.LogWarning("A null door actor was passedin into our RoomActor...");
            return;
        }
        if (!DoorTriggers.Add(Door))
        {
            Debug.LogWarning("This doo was already added to our room actor container");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Door"></param>
    public void RemoveDoorActor(DoorActor Door)
    {
        if (Door == null)
        {
            Debug.LogWarning("A null door actor was passed into our RoomActor...");
            return;
        }
        if (!DoorTriggers.Remove(Door))
        {
            Debug.LogWarning("The door that was passed in was not found in our room actor container");
        }
    }
}
