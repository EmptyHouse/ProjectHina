using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EHDamageableComponent : MonoBehaviour
{
    #region events
    public UnityEvent<EHDamageableComponent> OnDied;
    #endregion events

    #region main variables
    public float MaxHealth = 100;
    private float CurrentHealth;
    /// <summary>
    /// The owner of the 
    /// </summary>
    public EHGameplayCharacter Owner { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    private HashSet<EHDamageableComponent> ListOfOwnersWeHaveHit = new HashSet<EHDamageableComponent>();
    #endregion main variables

    #region monobehaviour methods
    private void Awake()
    {

    }
    #endregion monobehaviour methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Owner"></param>
    public void SetDamageableComponentOwner(EHGameplayCharacter Owner)
    {
        this.Owner = Owner;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="DamageComponentThatHitUs"></param>
    /// <param name="DamageTaken"></param>
    public void TakeDamage(EHDamageableComponent DamageComponentThatHitUs, float DamageTaken)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - DamageTaken, 0, MaxHealth);

        if (CurrentHealth <= 0)
        {
            OnDied?.Invoke(DamageComponentThatHitUs);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ReceiveHealing"></param>
    public void ReceiveHealing(float ReceiveHealing)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - ReceiveHealing, 0, MaxHealth);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnResetListOfUnitsThatHitUs()
    {
        ListOfOwnersWeHaveHit.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="OutHitbox"></param>
    /// <param name="OtherHitbox"></param>
    public void OnHitboxIntersectedOurHurtbox(EHHitbox OutHitbox, EHHitbox OtherHitbox)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="OurHitbox"></param>
    /// <param name="OtherHitbox"></param>
    public void OnHurtboxIntersectedOurHitbox(EHHitbox OurHitbox, EHHitbox OtherHitbox)
    {
        ListOfOwnersWeHaveHit.Add(OtherHitbox.DamageableComponent);
    }
}
