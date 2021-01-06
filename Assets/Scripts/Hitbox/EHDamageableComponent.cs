using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EHDamageableComponent : MonoBehaviour
{
    #region enum 
    public enum EDamageType
    {
        DAMAGE,
        HEALING,
        DEATH,
    }
    #endregion enum

    /// <summary>
    /// This delegate will be called whenevver our character's health falls below 0.
    /// </summary>
    public UnityAction<FDamageData> OnCharacterHealthChanged;


    [Tooltip("The maximum health of our Damageable component")]
    public int MaxHealth = 100;
    /// <summary>
    /// The ccurrent health of our character component
    /// </summary>
    private int CurrentHealth;

    #region monobehaviour methods
    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    private void OnValidate()
    {
        if (MaxHealth < 1)
        {
            MaxHealth = 1;//Health must be at least one. If they take any hit, they will die
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
        FDamageData DamageData = new FDamageData();
        DamageData.AttackSource = AttackComponentThatHurtUs;
        DamageData.DamageAmount = DamageToTake;
        int PreviousHealth = CurrentHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth - DamageToTake, 0, MaxHealth);

        if (CurrentHealth <= 0 && PreviousHealth > 0)
        {
            DamageData.DamageType = EDamageType.DEATH;
        }
        else
        {
            DamageData.DamageType = EDamageType.DAMAGE;
        }
        OnCharacterHealthChanged?.Invoke(DamageData);
    }

    /// <summary>
    /// Call this method to add health to oour character
    /// </summary>
    /// <param name="HealthToRecieve"></param>
    public void ReceiveHealth(int HealthToRecieve)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + HealthToRecieve, 0, MaxHealth);
    }
}

public struct FDamageData
{
    /// <summary>
    /// The type of damage that we received
    /// </summary>
    public EHDamageableComponent.EDamageType DamageType;
    /// <summary>
    /// The amount of damage that was received
    /// </summary>
    public int DamageAmount;
    /// <summary>
    /// The source that caused our damage. This value can be null
    /// </summary>
    public EHAttackComponent AttackSource;
}
