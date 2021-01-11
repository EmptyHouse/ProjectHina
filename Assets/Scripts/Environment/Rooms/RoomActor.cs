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


    #region monobehaviour methods
    private void Awake()
    {

    }
    #endregion monobeahviour methods

    public RoomData GetAssociatedRoomData() { return AssociatedRoomData; }

    
}
