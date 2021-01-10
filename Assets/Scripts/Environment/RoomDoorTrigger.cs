using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EHUtilities;

[RequireComponent(typeof(EHBaseCollider2D))]
public class RoomDoorTrigger : MonoBehaviour
{
    private SceneField RoomToEnter;

    #region monobehaviour methods
    private void Awake()
    {
        EHBaseCollider2D TriggerCollider = GetComponent<EHBaseCollider2D>();
        if (!TriggerCollider.GetIsTriggerCollider())
        {
            Debug.LogWarning("The collider component that is attached to this door is set to IsTrigger. Trigger events will not launch");
        }

        TriggerCollider.OnTrigger2DEnter += OnPlayerEnterRoom;
        TriggerCollider.OnTrigger2DExit += OnPlayerLeftRoom;
    }

    private void OnDestroy()
    {
        EHBaseCollider2D TriggerCollider = GetComponent<EHBaseCollider2D>();
        TriggerCollider.OnTrigger2DEnter -= OnPlayerEnterRoom;
        TriggerCollider.OnTrigger2DExit -= OnPlayerLeftRoom;
    }
    #endregion monobehaviour methods

    #region trigger events
    /// <summary>
    /// 
    /// </summary>
    /// <param name="TriggerData"></param>
    private void OnPlayerEnterRoom(FTriggerData TriggerData)
    {
        Debug.Log("Something Entered: " + TriggerData.OtherCollider.name);
        EHPlayerCharacter PlayerCharacter = TriggerData.OtherCollider.GetComponent<EHPlayerCharacter>();
        if (PlayerCharacter != null)
        {
            Debug.Log("Player Has exited");
        }
    }

    private void OnPlayerLeftRoom(FTriggerData TriggerData)
    {
        Debug.Log("Something exited: " + TriggerData.OtherCollider.name);
        EHPlayerCharacter PlayerCharacter = TriggerData.OtherCollider.GetComponent<EHPlayerCharacter>();
        if (PlayerCharacter != null)
        {
            Debug.Log("A Player Exited");
        }
    }
    #endregion trigger events

    private IEnumerator StartRoomTransition(EHPlayerCharacter PlayerCharacter)
    {
        yield break;
    }
}
