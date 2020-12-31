using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHHitboxActorComponent : MonoBehaviour
{
    public EHAttackComponent AttackComponent { get; private set; }
    public EHDamageableComponent DamageableComponent { get; private set; }

    #region monobehaviour methods
    private void Awake()
    {
        AttackComponent = GetComponent<EHAttackComponent>();
        DamageableComponent = GetComponent<EHDamageableComponent>();
    }
    #endregion monobehaviour methods

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
}
