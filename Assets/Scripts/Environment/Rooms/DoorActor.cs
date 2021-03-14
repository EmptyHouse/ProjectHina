using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(EHBaseCollider2D))]
public class DoorActor : MonoBehaviour
{
    [SerializeField]
    private DoorData AssociatedDoorData;
    [SerializeField]
    [Tooltip("Upon entering this door, this is the position that the character will spawn into")]
    private SpawnPoint AssociatedSpawnPoint = null;
    [SerializeField]
    private bool bCharacterEntersLeft = false;
    [SerializeField]
    private float SpawnPointOffset = 0;

    #region monobehaviour methods
    private void Awake()
    {
        EHBaseCollider2D TriggerCollider = GetComponent<EHBaseCollider2D>();
        if (!TriggerCollider.GetIsTriggerCollider())
        {
            Debug.LogWarning("The collider component that is attached to this door is set to IsTrigger. Trigger events will not launch");
        }

        TriggerCollider.OnTrigger2DEnter += OnPlayerEnterRoom;
    }

    private void Start()
    {
        if (BaseGameOverseer.Instance != null)
        {
            BaseGameOverseer.Instance.CurrentlyLoadedRoom.AddDoorActor(this);
        }
        else
        {
            Debug.LogWarning("There is no Game Overseer in the scene. This is likely due to no having the main gameplay scene loaded in");
        }
    }

    private void OnDestroy()
    {
        EHBaseCollider2D TriggerCollider = GetComponent<EHBaseCollider2D>();
        TriggerCollider.OnTrigger2DEnter -= OnPlayerEnterRoom;
        if (BaseGameOverseer.Instance)
        {
            BaseGameOverseer.Instance.CurrentlyLoadedRoom.RemoveDoorActor(this);
        }
    }

    private void OnValidate()
    {
        if (AssociatedSpawnPoint)
        {
            AssociatedSpawnPoint.transform.position = transform.position + (bCharacterEntersLeft ? Vector3.right : Vector3.left) * SpawnPointOffset;
            if (AssociatedSpawnPoint.bIsCharacterSpawnedLeft == bCharacterEntersLeft)
            {
                AssociatedSpawnPoint.bIsCharacterSpawnedLeft = !bCharacterEntersLeft;
            }
        }

        if (AssociatedDoorData != null && this.name != AssociatedDoorData.name)
        {
            this.name = AssociatedDoorData.name;
        }
    }
    #endregion monobehaviour methods

    public DoorData GetAssociatedDoorData() { return AssociatedDoorData; }
    
    public Vector3 GetSpawnPosition()
    {
        if (AssociatedSpawnPoint == null)
        {
            Debug.LogError("There was no spawn point assigned to our door actor");
            return Vector3.zero;
        }
        return AssociatedSpawnPoint.transform.position;
    }

    #region trigger events
    /// <summary>
    /// This will transition to another door if a player character has entered
    /// </summary>
    /// <param name="TriggerData"></param>
    private void OnPlayerEnterRoom(FTriggerData TriggerData)
    {
        Debug.Log("Something Entered: " + TriggerData.OtherCollider.name);
        EHPlayerCharacter PlayerCharacter = TriggerData.OtherCollider.GetComponent<EHPlayerCharacter>();
        if (PlayerCharacter != null)
        {
            Debug.Log("Player Has exited");
            BaseGameOverseer.Instance.GameHUD.GetScreenTransition().LoadNewRoomFromDoorActor(PlayerCharacter, AssociatedDoorData.GetConnectedDoorData());
        }
    }
    #endregion trigger events

    
}
