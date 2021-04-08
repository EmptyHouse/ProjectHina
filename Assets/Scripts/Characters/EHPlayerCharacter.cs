using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a gameplay character that is human controlled. Any logic for a human controlled character should go here
/// </summary>
public class EHPlayerCharacter : EHGameplayCharacter
{
    #region animation triggers
    private const string ANIM_KILL_CANCEL = "KillCancel";
    #endregion animation triggers

    public EHPlayerController PlayerController { get { return playerController; } }
    WallJump CharacterWallJump;
    private EHPlayerController playerController;

    protected override void Awake()
    {
        base.Awake();
        CharacterWallJump = GetComponent<WallJump>();
        playerController = (EHPlayerController)CharacterController;
        AttackComponent.OnAttackCharacterDel += OnKilledCharacter;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Damage"></param>
    private void OnKilledCharacter(FAttackData Damage, EHDamageableComponent DamageComponentWeHit)
    {
        if (DamageComponentWeHit.Health <= 0)
        {
            CharacterAnim.SetTrigger(ANIM_KILL_CANCEL);
        }
    }
}
