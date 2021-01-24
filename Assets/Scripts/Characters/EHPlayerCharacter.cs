using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a gameplay character that is human controlled. Any logic for a human controlled character should go here
/// </summary>
public class EHPlayerCharacter : EHGameplayCharacter
{
    public EHPlayerController PlayerController { get { return playerController; } }
    private EHPlayerController playerController;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        playerController = (EHPlayerController)CharacterController;
    }
}
