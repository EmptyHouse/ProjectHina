﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The attack component can apply damage to DamageableComponents.
/// </summary>
public class EHAttackComponent : MonoBehaviour
{

    private const string ATTACK_ANIM = "Attack";
    /// <summary>
    /// 
    /// </summary>
    public UnityAction<FAttackData, EHDamageableComponent> OnAttackCharacterDel;
    /// <summary>
    /// This will be called when a new attack has begun
    /// </summary>
    public UnityAction OnBeginNewAttackDel;
    [Tooltip("The assigned DataTable that will contain all the information for each attack.")]
    public AttackDataTable AssociatedAttackTable;

    [Tooltip("If there is not attack data found for our character, we will default values in this struct. Good if you have an object or character that only has one attack")]
    public FAttackData DefaultAttackData;

    /// <summary>
    /// List of all the damageable components that we have interacted with. Anything in this list has already been registered as a hit. We will not apply damage to a component in this list
    /// until it has been removed
    /// </summary>
    private HashSet<EHDamageableComponent> IntersectedDamageableComponents = new HashSet<EHDamageableComponent>();

    private EHHitboxActorComponent AssociatedHitboxComponent;

    /// <summary>
    /// 
    /// </summary>
    private Animator AssociatedAnimator;

    #region monobehaviour methods
    private void Awake()
    {
        AssociatedAnimator = GetComponent<Animator>();
        if (AssociatedAttackTable != null)
        {
            BaseGameOverseer.Instance.DataTableManager.AddAttackDataTable(AssociatedAttackTable);
        }
        AssociatedHitboxComponent = GetComponent<EHHitboxActorComponent>();
    }
    #endregion monobehaviour methods

    public EHCharacter.ECharacterTeam GetAllignedCharacterTeam()
    {
        if (AssociatedHitboxComponent != null)
        {
            return AssociatedHitboxComponent.GetAllignedCharacterTeam();
        }
        return EHCharacter.ECharacterTeam.NONE;
    }

    /// <summary>
    /// This method should be called anytime a new hit should begin. if you are setting up a multihit move, this should be called between each hit in the animation
    /// </summary>
    public void OnClearAllIntersectedDamageableComponents()
    {
        IntersectedDamageableComponents.Clear();
    }
    
    /// <summary>
    /// This method will be called when a hitbox component enters the hurtbox component of another damageable component
    /// </summary>
    public virtual void OnDamageableComponentIntersectionBegin(EHDamageableComponent DamageableComponentWeHit)
    { 
        if (IntersectedDamageableComponents.Contains(DamageableComponentWeHit))
        {
            // Skip if we have already hit this component before resetting...
            return;
        }

        if (DamageableComponentWeHit.GetAllignedCharacterTeam() == GetAllignedCharacterTeam())
        {
            return;//Don't hit characters that are on the same team as us
        }

        FAttackData AttackData;
        if (AssociatedAttackTable == null)
        {
            AttackData = DefaultAttackData;
        }
        else if (BaseGameOverseer.Instance.DataTableManager.GetAttackDataFromAttackDataTable(AssociatedAttackTable, AssociatedAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, out AttackData))
        {
        }
        else
        {
            Debug.LogWarning("This animation has not be set up in our attack Data table: " + AssociatedAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash.ToString());
            AttackData = DefaultAttackData;
        }
        IntersectedDamageableComponents.Add(DamageableComponentWeHit);
        if (!DamageableComponentWeHit.TakeDamage(this, AttackData.AttackDamage))
        {
            return;//If we were not able to successfully hurt the other character we will skip all other effects
        }
        
        if (BaseGameOverseer.Instance.MainGameCamera && BaseGameOverseer.Instance.MainGameCamera.CameraShake)
        {
            BaseGameOverseer.Instance.MainGameCamera.CameraShake.BeginCameraShake(AttackData.CameraShakeDuration, AttackData.CameraShakeIntensity);
            BaseGameOverseer.Instance.GlobalEffectManager.StartFreezeTimeForSeconds(AttackData.HitFreezeTime);
        }

        float ScaleX = Mathf.Sign(DamageableComponentWeHit.transform.position.x - transform.position.x);
        if (ScaleX == 0) ScaleX = 1;
        DamageableComponentWeHit.ApplyKnockback(AttackData.HitStunTime, new Vector2(ScaleX, 1) * AttackData.LaunchForce);
        if (AttackData.bIsConstantHitbox)
        {
            StartCoroutine(ClearHitListNextFrame());
        }

        OnAttackCharacterDel?.Invoke(AttackData, DamageableComponentWeHit);
    }

    public virtual void OnDamageableComponentIntersectionEnd(EHDamageableComponent DamageableComponentHit) { }

    /// <summary>
    /// Since the ability to perform an attack will primarily be animation driven this method will call a trigger 
    /// on the animation controller
    /// </summary>
    /// <param name="AttackID"></param>
    public void AttemptAttack(int AttackID)
    {
        if (!AssociatedAnimator)
        {
            Debug.LogWarning("Attempting to attack with a null animator. Be sure there is one assigned");
        }
        string AttackName = ATTACK_ANIM + AttackID.ToString("00");
        AssociatedAnimator.SetTrigger(AttackName);
    }

    /// <summary>
    /// Release the attack button. This will make it so that there is a buffer that is applied to the attack without having to keep the
    /// active the entire time
    /// </summary>
    /// <param name="AttackID"></param>
    public void ReleaseAttack(int AttackID)
    {
        string AttackName = ATTACK_ANIM + AttackID.ToString("00");
        AssociatedAnimator.SetBool(AttackName, false);
    }

    /// <summary>
    /// Coroutine that will pause time when an attack is made. This is to give a more satisfying feel to the hit ideally
    /// </summary>
    /// <param name="SecondsToPauseGameWhenHitConnected"></param>
    /// <returns></returns>
    private IEnumerator StopTimeWhenHitCoroutine(float SecondsToPauseGameWhenHitConnected, EHDamageableComponent DamageComponentThatWeHit, FAttackData AttackData)
    {

        float TimeThatHasPassed = 0;

        

        while (TimeThatHasPassed < SecondsToPauseGameWhenHitConnected)
        {
            TimeThatHasPassed += EHTime.RealDeltaTime;
            yield return null;
        }
    }

    private IEnumerator ClearHitListNextFrame()
    {
        yield return null;
        OnClearAllIntersectedDamageableComponents();
    }
}

/// <summary>
/// Information that relates to an attack that will do damage to a damageable component
/// </summary>
[System.Serializable]
public struct FAttackData
{
    [Tooltip("The damage that will be applied to the damageable component that we hit")]
    public int AttackDamage;
    [Tooltip("The amount of time the actor we hit will be locked in hitstun")]
    public float HitStunTime;
    [Tooltip("The time in which time will freeze when hitting the other opponent")]
    public float HitFreezeTime;
    [Tooltip("The time in seconds that our camera will shake.")]
    public float CameraShakeDuration;
    [Tooltip("Intensity of the camera shake that will be applied")]
    public float CameraShakeIntensity;
    [Tooltip("The force at which we will lauch the damageable component actor that we hit")]
    public Vector2 LaunchForce;
    [Tooltip("Indicates whether or not we can keep hitting a character")]
    public bool bIsConstantHitbox;
}