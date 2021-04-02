using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EHDamageableComponent : MonoBehaviour
{
    #region const values
    private const string ANIM_HITSTUN = "HitStunTime";
    #endregion const values

    #region enum 
    public enum EDamageType
    {
        NONE,
        DAMAGE,
        HEALING,
        DEATH,
    }
    #endregion enum

    private readonly AnimationCurve HitStunDropOff = AnimationCurve.EaseInOut(0, 1, 1, 0);

    /// <summary>
    /// This delegate will be called whenevver our character's health falls below 0.
    /// </summary>
    public UnityAction<FDamageData> OnCharacterHealthChanged;
    public UnityAction OnHitStunStart;
    public UnityAction OnHitStunEnd;


    [Tooltip("The maximum health of our Damageable component")]
    public int MaxHealth = 1;
    /// <summary>
    /// The ccurrent health of our character component
    /// </summary>
    public int Health { get; private set; }

    public EHPhysics2D Physics2D { get; private set; }

    private EHHitboxActorComponent AssociatedHitboxComponent;

    private Animator AnimReference;

    private bool bIsInvincible;

    #region monobehaviour methods
    private void Awake()
    {
        Health = MaxHealth;
        Physics2D = GetComponent<EHPhysics2D>();
        AnimReference = GetComponent<Animator>();
        AssociatedHitboxComponent = GetComponent<EHHitboxActorComponent>();
        AnimReference.SetFloat(ANIM_HITSTUN, -1);
    }
    private void OnValidate()
    {
        if (MaxHealth < 1)
        {
            MaxHealth = 1;//Health must be at least one. If they take any hit, they will die
        }
    }
    #endregion monobehaviour methods

    public EHCharacter.ECharacterTeam GetAllignedCharacterTeam()
    {
        if (AssociatedHitboxComponent)
        {
            return AssociatedHitboxComponent.GetAllignedCharacterTeam();
        }
        return EHCharacter.ECharacterTeam.NONE;
    }

    /// <summary>
    /// This method will be called to apply damage to
    /// 
    /// This will return true if damage was successfully given to the player
    /// </summary>
    /// <param name="AttackComponentThatHurtUs"></param>
    /// <param name="DamageToTake"></param>
    public bool TakeDamage(EHAttackComponent AttackComponentThatHurtUs, int DamageToTake)
    {
        FDamageData DamageData = new FDamageData();
        if (bIsInvincible)
        { 
            DamageData.DamageType = EDamageType.NONE;
            OnCharacterHealthChanged?.Invoke(DamageData);
            return false;
        }
        
        DamageData.AttackSource = AttackComponentThatHurtUs;
        DamageData.DamageAmount = DamageToTake;
        int PreviousHealth = Health;
        Health = Mathf.Clamp(Health - DamageToTake, 0, MaxHealth);

        if (Health <= 0 && PreviousHealth > 0)
        {
            DamageData.DamageType = EDamageType.DEATH;
        }
        else
        {
            DamageData.DamageType = EDamageType.DAMAGE;
        }
        OnCharacterHealthChanged?.Invoke(DamageData);
        return true;
    }

    /// <summary>
    /// Call this method to add health to oour character
    /// </summary>
    /// <param name="HealthToRecieve"></param>
    public void ReceiveHealth(int HealthToRecieve)
    {
        Health = Mathf.Clamp(Health + HealthToRecieve, 0, MaxHealth);
    }

    public void ApplyKnockback(float HitStunTime, Vector2 KnockbackDirection)
    {
        StartCoroutine(KnockBackCoroutine(HitStunTime, KnockbackDirection));
    }

    public void SetIsInvincible(bool bIsInvincible)
    {
        if (this.bIsInvincible == bIsInvincible)
        {
            return;
        }
        this.bIsInvincible = bIsInvincible;
    }

    private IEnumerator KnockBackCoroutine(float HitStunTime, Vector2 KnockbackForce)
    {
        OnHitStunStart?.Invoke();
        float TimeThatHasPassed = 0;

        while (TimeThatHasPassed < HitStunTime)
        {
            TimeThatHasPassed += EHTime.DeltaTime;
            Physics2D.Velocity = HitStunDropOff.Evaluate(TimeThatHasPassed) * KnockbackForce.x * Vector2.right + Vector2.up * Physics2D.Velocity.y;
            if (AnimReference)
            {
                AnimReference.SetFloat(ANIM_HITSTUN, HitStunTime - TimeThatHasPassed);
            }
            yield return null;
        }
        if (AnimReference)
        {
            AnimReference.SetFloat(ANIM_HITSTUN, -1);
        }
        Physics2D.Velocity = new Vector2(0, Physics2D.Velocity.y);
        OnHitStunEnd?.Invoke();  
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
