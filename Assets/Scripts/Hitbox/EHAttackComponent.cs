using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The attack component can apply damage to DamageableComponents.
/// </summary>
public class EHAttackComponent : MonoBehaviour
{
    [Tooltip("The damage that we will apply to the DamageableComponent that we interact with")]
    public int DamageToApply = 5;
    [Tooltip("The amount of time to freeze after performing a hit")]
    public float TimeFreezeOnHit = 0;

    /// <summary>
    /// The owner of our Attack component. This will help in determining which object is appropiate to intersect with
    /// </summary>
    private EHGameplayCharacter CharacterOwner;
    /// <summary>
    /// List of all the damageable components that we have interacted with. Anything in this list has already been registered as a hit. We will not apply damage to a component in this list
    /// until it has been removed
    /// </summary>
    private HashSet<EHDamageableComponent> IntersectedDamageableComponents = new HashSet<EHDamageableComponent>();

    #region monobehaviour methods
    private void Awake()
    {
        SetCharacterOwner(GetComponent<EHGameplayCharacter>());
    }
    #endregion monobehaviour methods

    public void SetCharacterOwner(EHGameplayCharacter CharacterOwner)
    {
    }

    public void ClearAllIntersectedDamageableComponents()
    {
    }

    public virtual void DealDamageToDamageableComponent(EHDamageableComponent DamageableComponent)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void OnHitboxBeginIntersectEnemyHurtbox(EHHitbox OurHitbox, EHHitbox OtherHitbox)
    {

    }

    public virtual void OnHitboxEndIntersectingEnemyHurtbox(EHHitbox OurHitbox, EHHitbox OtherHitbox)
    {

    }

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
