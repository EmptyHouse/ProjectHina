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
    private const string ANIM_CHARACTER_DIED = "IsDead";
    private const string DEAD_LAYER = "Dead";
    #endregion const varibles

    /// <summary>
    /// Every gameplay character should have a damageable component attached to them. If not, this can be left null
    /// </summary>
    public EHDamageableComponent DamageableComponent { get; private set; }
    /// <summary>
    /// Attack component. A gameplay character that can deal damage to another character should have this component attached
    /// </summary>
    public EHAttackComponent AttackComponent { get; private set; }
    /// <summary>
    /// A character that has hurtboxes or hitboxes should have this component attached to them to properly interpret Hitbox intersections
    /// </summary>
    public EHHitboxActorComponent HitboxComponent { get; private set; }
    /// <summary>
    /// The base movement component for a character. Any character that can freely move should have this component
    /// </summary>
    public EHBaseMovementComponent MovementComponent { get; private set; }
    /// <summary>
    /// 
    /// </summary>
    public EHPhysics2D Physics2D { get; private set; }
    /// <summary>
    /// Collider component that is attached to our character. This should be attached to character to prevent them from going through environment collider
    /// as well as activate triggers
    /// </summary>
    public EHBox2DCollider CharacterCollider { get; private set; }
    /// <summary>
    /// The controller that determines various actions that our character will take. Primary use only for gameplay characters
    /// </summary>
    public EHController CharacterController { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        DamageableComponent = GetComponent<EHDamageableComponent>();
        AttackComponent = GetComponent<EHAttackComponent>();
        HitboxComponent = GetComponent<EHHitboxActorComponent>();
        MovementComponent = GetComponent<EHBaseMovementComponent>();
        CharacterCollider = GetComponent<EHBox2DCollider>();
        CharacterController = GetComponent<EHController>();
        Physics2D = GetComponent<EHPhysics2D>();

        if (DamageableComponent)
        {
            DamageableComponent.OnCharacterHealthChanged += OnCharacterDied;
            DamageableComponent.OnHitStunStart += OnHitStunStart;
            DamageableComponent.OnHitStunEnd += OnHitStunEnd;
        }
    }

    protected virtual void OnDestroy()
    {
        if (DamageableComponent)
        {
            DamageableComponent.OnCharacterHealthChanged -= OnCharacterDied;
            DamageableComponent.OnHitStunStart -= OnHitStunStart;
            DamageableComponent.OnHitStunEnd -= OnHitStunEnd;
        }
    }

    /// <summary>
    /// This method will be called when the health of our character in the Damageable Component has reached 0. Any cleanup shoud be done here
    /// </summary>
    public virtual void OnCharacterDied(FDamageData DamageData) 
    {
        // If this is not a Death type attack, then we will skip this method
        if (DamageData.DamageType != EHDamageableComponent.EDamageType.DEATH) return;

        if (CharacterAnim)
        {
            CharacterAnim.SetTrigger(ANIM_CHARACTER_DIED);
        }
        this.gameObject.layer = LayerMask.NameToLayer(DEAD_LAYER);
        if (CharacterController)
            CharacterController.enabled = false;
        if (MovementComponent)
            MovementComponent.enabled = false;
        if (HitboxComponent)
            HitboxComponent.enabled = false;
        if (DamageableComponent)
            DamageableComponent.enabled = false;
        if (AttackComponent)
            AttackComponent.enabled = false;
    }

    public void SpawnCharacterToPosition(Vector3 NewPosition, bool bIgnoreSweep = true)
    {
        this.transform.position = NewPosition;
        CharacterCollider.UpdateColliderBounds(bIgnoreSweep);//this is to avoid 
    }

    private void OnHitStunStart()
    {
        if (MovementComponent)
        {
            MovementComponent.enabled = false;
        }
    }

    private void OnHitStunEnd()
    {
        if (MovementComponent)
        {
            MovementComponent.enabled = true;
        }
    }
}
