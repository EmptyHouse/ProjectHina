using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHHitboxActorComponent : MonoBehaviour
{
    // Attached attack component for our character. If null attacks cannot be registered if our hitbox intersects anything
    public EHAttackComponent AttackComponent { get; private set; }
    // Attached damageable component for our character. If null our actor will not be able to take damage if our hurtbox is intersected
    public EHDamageableComponent DamageableComponent { get; private set; }
    // The character owner for our hitbox component. Typically it will be gameplay character that will be taking damage in our game. May be instances where that is not the case
    public EHGameplayCharacter CharacterOwner { get; private set; }

    #region animation varaibles
    [HideInInspector]
    public bool bAnimationIsInvincible = false;
    #endregion animation variables

    #region monobehaviour methods
    private void Awake()
    {
        AttackComponent = GetComponent<EHAttackComponent>();
        DamageableComponent = GetComponent<EHDamageableComponent>();
        CharacterOwner = GetComponent<EHGameplayCharacter>();
    }
    #endregion monobehaviour methods

    public void SetCharacterOwner(EHGameplayCharacter CharacterOwner)
    {
        this.CharacterOwner = CharacterOwner;
    }

    /// <summary>
    /// This method should be called from an EHHitbox
    /// </summary>
    /// <param name="OurHitbox"></param>
    /// <param name="OtherHitbox"></param>
    public void OnHitboxBeginIntersectOtherHitbox(EHHitbox OurHitbox, EHHitbox OtherHitbox)
    {
        if (OurHitbox.GetHitboxType() == EHHitbox.EHitboxType.HITBOX)
        {
            if (OtherHitbox.GetHitboxType() == EHHitbox.EHitboxType.HURTBOX)
            {
                OurHitboxIntersectOtherHurtbox(OurHitbox, OtherHitbox);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="OurHitbox"></param>
    /// <param name="OtherHitbox"></param>
    public void OnHitboxEndintersectOtherHitbox(EHHitbox OurHitbox, EHHitbox OtherHitbox)
    {
        if (OurHitbox.GetHitboxType() == EHHitbox.EHitboxType.HITBOX)
        {
            if (OtherHitbox.GetHitboxType() == EHHitbox.EHitboxType.HURTBOX)
            {
                
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="OurHitbox"></param>
    /// <param name="OtherHurtbox"></param>
    private void OurHitboxIntersectOtherHurtbox(EHHitbox OurHitbox, EHHitbox OtherHurtbox)
    {
        if (OurHitbox == null || OtherHurtbox == null)
        {
            return; 
        }

        if (OtherHurtbox.HitboxActorComponent.bAnimationIsInvincible)
        {
            return;//Other character was invincible when we attempted to hit them, may in the future want to add some kind of wiff affect to indicate that you made contact but couldn't hurt them
        }

        EHDamageableComponent OtherDamageableComponent = OtherHurtbox.HitboxActorComponent.DamageableComponent;
        
        if (OtherDamageableComponent == null)
        {
            Debug.LogWarning("There is no Damageable component associated with the hurtbox we are intersecting");
            return;
        }
        if (AttackComponent == null)
        {
            Debug.LogWarning("There is no Attack component associated with our intersecting Hitbox");
            return;
        }
        AttackComponent.OnDamageableComponentIntersectionBegin(OtherDamageableComponent);
    }

    private void OurHitboxEndintersectOtherHurtbox(EHHitbox OurHitbox, EHHitbox OtherHitbox)
    {

    }

    public EHCharacter.ECharacterTeam GetAllignedCharacterTeam()
    {
        if (CharacterOwner == null)
        {
            return EHCharacter.ECharacterTeam.NONE;
        }
        return CharacterOwner.CharacterTeam;
    }
}
