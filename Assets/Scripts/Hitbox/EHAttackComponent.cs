using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHAttackComponent : MonoBehaviour
{
    /// <summary>
    /// The damage that we will apply to the DamageableComponent that we interact with
    /// </summary>
    public float DamageToApply;

    private EHGameplayCharacter CharacterOwner;
    private HashSet<EHDamageableComponent> IntersectedDamageableComponents = new HashSet<EHDamageableComponent>();

    /// <summary>
    /// 
    /// </summary>
    public void OnHitboxIntersectEnemyHurtbox()
    {

    }
}
