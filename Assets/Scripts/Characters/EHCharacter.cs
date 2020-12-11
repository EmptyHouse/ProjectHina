using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all character that will be in our game. 
/// </summary>
public class EHCharacter : MonoBehaviour
{
    /// <summary>
    /// Name of the character 
    /// </summary>
    public string CharacterName;

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
