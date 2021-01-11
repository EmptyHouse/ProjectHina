using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Tooltip("This value must be unique per room")]
    public uint SpawnPointID;
    [Tooltip("Sets the GameplayCharacter's direction")]
    public bool bIsCharacterSpawnedLeft;
    [Tooltip("If it does not matter what direction the player enters. for example, if this is not a ground door, but a door that they fall through we do not need to set the direction of the player")]
    public bool bIgnoreDirection;

    #region monobehaivour methods
    private void Awake()
    {
        
    }


    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.cyan;
        float SpawnPointHeight = .5f;
        UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.forward, .10f);
        Vector3 TopPoint = transform.position + Vector3.up * SpawnPointHeight;
        Vector3 MidPoint = transform.position + Vector3.up * SpawnPointHeight / 2;
        Vector3 RightPoint = (TopPoint + MidPoint) / 2 + ((bIsCharacterSpawnedLeft ? Vector3.left : Vector3.right) * SpawnPointHeight / 2);
        UnityEditor.Handles.DrawLine(transform.position, TopPoint);
        UnityEditor.Handles.DrawPolyLine(new Vector3[] { TopPoint, MidPoint, RightPoint, TopPoint });
#endif
    }
    #endregion monobehavoiur methods

    public void SpawnAtPoint(EHGameplayCharacter CharacterToPlace)
    {
        CharacterToPlace.transform.position = this.transform.position;
        CharacterToPlace.MovementComponent.SetIsFacingLeft(bIsCharacterSpawnedLeft);
    }
}
