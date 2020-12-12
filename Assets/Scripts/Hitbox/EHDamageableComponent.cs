using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EHDamageableComponent : MonoBehaviour
{
    [Tooltip("The maximum health of our Damageable component")]
    public int MaxHealth = 100;
    /// <summary>
    /// The ccurrent health of our character component
    /// </summary>
    private int CurrentHealth;

    /// <summary>
    /// The owning character for our damageable component
    /// </summary>
    private EHGameplayCharacter CharacterOwner;


    #region monobehaviour methods
    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    private void OnValidate()
    {
        if (MaxHealth < 0)
        {
            MaxHealth = 0;
        }
    }
    #endregion monobehaviour methods

    /// <summary>
    /// This method will be called to apply damage to 
    /// </summary>
    /// <param name="AttackComponentThatHurtUs"></param>
    /// <param name="DamageToTake"></param>
    public void TakeDamage(EHAttackComponent AttackComponentThatHurtUs, int DamageToTake)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - DamageToTake, 0, MaxHealth);

        if (CurrentHealth <= 0)
        {
            CharacterOwner.OnCharacterDied(AttackComponentThatHurtUs);
        }
    }

    /// <summary>
    /// Call this method to add health to oour character
    /// </summary>
    /// <param name="HealthToRecieve"></param>
    public void ReceiveHealth(int HealthToRecieve)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + HealthToRecieve, 0, MaxHealth);
    }

    /// <summary>
    /// This will assign the character that owns the damageable component that this is attached to
    /// </summary>
    /// <param name="CharacterOwner"></param>
    public void SetCharacterOwner(EHGameplayCharacter CharacterOwner)
    {
        this.CharacterOwner = CharacterOwner;
    }
}
