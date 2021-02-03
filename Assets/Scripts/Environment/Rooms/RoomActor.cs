using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Be sure to include this into every room with the associated RoomData uasset
/// </summary>
public class RoomActor : MonoBehaviour
{
    [SerializeField] 
    private Vector2 RoomBoundsMin = Vector2.one * 10;
    [SerializeField]
    private Vector2 RoomBoundsMax = Vector2.one * -10;
    

    [SerializeField]
    private RoomData AssociatedRoomData = null;
    private HashSet<DoorActor> DoorTriggers = new HashSet<DoorActor>();

    #region monobehaviour methods
    private void Awake()
    {
        if (BaseGameOverseer.Instance != null)
        {
            BaseGameOverseer.Instance.CurrentlyLoadedRoom = this;
        }
        else
        {
            Debug.Log("There is no GameOverseer in the room. More than likely you have not added the main game scene to the hierachry");
        }
    }

    private void OnValidate()
    {
        if (RoomBoundsMin.x > RoomBoundsMax.x) { RoomBoundsMin.x = RoomBoundsMax.x;}
        if (RoomBoundsMin.y > RoomBoundsMax.y) {RoomBoundsMin.y = RoomBoundsMax.y;}
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Rect BoundsRect = new Rect(RoomBoundsMin.x, RoomBoundsMin.y, RoomBoundsMax.x - RoomBoundsMin.x, RoomBoundsMax.y- RoomBoundsMin.y);
        Handles.DrawSolidRectangleWithOutline(BoundsRect, Color.clear, Color.white);
    }
#endif

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

    public Vector2 GetMinRoomBounds()
    {
        return RoomBoundsMin;
    }

    // 
    public Vector2 GetMaxRoomBounds()
    {
        return RoomBoundsMax;
    }
}
