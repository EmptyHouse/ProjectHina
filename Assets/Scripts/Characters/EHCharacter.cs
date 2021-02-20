using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all character that will be in our game. 
/// </summary>
public class EHCharacter : MonoBehaviour
{
    public enum ECharacterTeam
    {
        NONE,
        PLAYER,
        ENEMY,
    }

    [Tooltip("The name of our character.")]
    public string CharacterName;

    [Tooltip("The team allignment of our character")]
    public ECharacterTeam CharacterTeam = ECharacterTeam.NONE;

    /// <summary>
    /// Associated character animator.
    /// </summary>
    public Animator CharacterAnim { get; private set; }

    #region monobehaviour methods
    protected virtual void Awake()
    {
        CharacterAnim = GetComponent<Animator>();
    }
    #endregion monobehaviour methods
}
