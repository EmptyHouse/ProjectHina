using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The attack component can apply damage to DamageableComponents.
/// </summary>
public class EHAttackComponent : MonoBehaviour
{
    public AttackDataTable AssociatedAttackTable;

    [Tooltip("The damage that we will apply to the DamageableComponent that we interact with")]
    public int DamageToApply = 5;
    [Tooltip("The knockback force that will be applied to a character that is hit")]
    public float KnockbackIntensity = 0;
    [Tooltip("The direction that we will knock our character back ")]
    public Vector2 KnockbackDirection;

    [Header("Effects Upon Hit")]
    [Tooltip("The amount of time to freeze after performing a hit")]
    public float TimeFreezeOnHit = 0;
    public float CameraShakeIntensity = 0;
    public float CameraShakeDuration = 0;

    /// <summary>
    /// The owner of our Attack component. This will help in determining which object is appropiate to intersect with
    /// </summary>
    private EHGameplayCharacter CharacterOwner;

    /// <summary>
    /// List of all the damageable components that we have interacted with. Anything in this list has already been registered as a hit. We will not apply damage to a component in this list
    /// until it has been removed
    /// </summary>
    private HashSet<EHDamageableComponent> IntersectedDamageableComponents = new HashSet<EHDamageableComponent>();

    private EHPhysics2D Physics2D;

    #region monobehaviour methods
    private void Awake()
    {
        Physics2D = GetComponent<EHPhysics2D>();
        SetCharacterOwner(GetComponent<EHGameplayCharacter>());
    }
    #endregion monobehaviour methods

    public void SetCharacterOwner(EHGameplayCharacter CharacterOwner)
    {
        this.CharacterOwner = CharacterOwner;
    }

    public void ClearAllIntersectedDamageableComponents()
    {
        IntersectedDamageableComponents.Clear();
    }

    public virtual void DealDamageToDamageableComponent(EHDamageableComponent DamageableComponent)
    {
        DamageableComponent.TakeDamage(this, DamageToApply);
    }

    /// <summary>
    /// This method will be call
    /// </summary>
    public void OnInteractWithOtherDamageableComponent(EHDamageableComponent DamageableComponentWeHit)
    {
        DealDamageToDamageableComponent(DamageableComponentWeHit);
    }

    public virtual void OnHitboxEndIntersectingEnemyHurtbox(EHDamageableComponent DamageableComponentHit) { }

    protected IEnumerator StopTimeWhenHit(float SecondsToStop)
    {
        if (SecondsToStop <= 0)
        {
            yield break;
        }

        float TimeThatHasPassed = 0;
        
        while (TimeThatHasPassed < SecondsToStop)
        {
            yield return null;
            TimeThatHasPassed += EHTime.RealDeltaTime;
        }
    }
}

[System.Serializable]
public struct FAttackData
{
    public float AttackDamage;
    public float HitStunTime;
    public float HitFreezeTime;
    public Vector2 LaunchForce;
}