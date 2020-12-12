using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHAttackComponent : MonoBehaviour
{
    /// <summary>
    /// The damage that we will apply to the DamageableComponent that we interact with
    /// </summary>
    public int DamageToApply = 5;

    /// <summary>
    /// The owner of our Attack component. This will help in determining which object is appropiate to intersect with
    /// </summary>
    private EHGameplayCharacter CharacterOwner;
    private HashSet<EHDamageableComponent> IntersectedDamageableComponents = new HashSet<EHDamageableComponent>();

    #region monobehaviour methods
    private void Awake()
    {
        SetCharacterOwner(GetComponent<EHGameplayCharacter>());
    }
    #endregion monobehaviour methods

    public void SetCharacterOwner(EHGameplayCharacter CharacterOwner)
    {
        this.CharacterOwner = CharacterOwner;
    }

    public void DealDamageToDamageableComponent(EHDamageableComponent DamageableComponent)
    {
        DamageableComponent.TakeDamage(this, DamageToApply);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnHitboxIntersectEnemyHurtbox()
    {

    }
}
