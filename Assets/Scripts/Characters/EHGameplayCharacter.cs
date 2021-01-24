using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Gameplay character is a character component that will contain logic that involves gameplay aspects, such as 
/// </summary>
[RequireComponent(typeof(EHDamageableComponent))]
public class EHGameplayCharacter : EHCharacter
{
    #region const variables
    private const string ANIM_CHARACTER_DIED = "Dead";

    #endregion const varibles

    /// <summary>
    /// Every gameplay character should have a damageable component attached to them. If not, this can be left null
    /// </summary>
    public EHDamageableComponent DamageableComponent { get; private set; }
    public EHAttackComponent AttackComponent { get; private set; }
    public EHHitboxActorComponent HitboxComponent { get; private set; }
    public EHMovementComponent MovementComponent { get; private set; }
    public EHBox2DCollider CharacterCollider { get; private set; }
    public EHBaseController CharacterController { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        DamageableComponent = GetComponent<EHDamageableComponent>();
        AttackComponent = GetComponent<EHAttackComponent>();
        HitboxComponent = GetComponent<EHHitboxActorComponent>();
        MovementComponent = GetComponent<EHMovementComponent>();
        CharacterCollider = GetComponent<EHBox2DCollider>();
        CharacterController = GetComponent<EHBaseController>();

        if (DamageableComponent)
        {
            DamageableComponent.OnCharacterHealthChanged += OnCharacterDied;
        }
        
    }

    /// <summary>
    /// This method will be called when the health of our character in the Damageable Component has reached 0. Any cleanup shoud be done here
    /// </summary>
    public virtual void OnCharacterDied(FDamageData DamageData) 
    {
        // If this is not a Death type attack, then we will skip this method
        if (DamageData.DamageType != EHDamageableComponent.EDamageType.DEATH) return;

        Debug.Log(this.name + " Has Died");
        CharacterAnim.SetTrigger(ANIM_CHARACTER_DIED);
    }

    public void SpawnCharacterToPosition(Vector3 NewPosition, bool bIgnoreSweep = true)
    {
        this.transform.position = NewPosition;
        CharacterCollider.UpdateColliderBounds(bIgnoreSweep);//this is to avoid 
    }
}
