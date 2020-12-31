using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The attack component can apply damage to DamageableComponents.
/// </summary>
public class EHAttackComponent : MonoBehaviour
{
    private const string ATTACK_ANIM = "Attack";

    public AttackDataTable AssociatedAttackTable;

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
    private Animator AssociatedAnimator;

    #region monobehaviour methods
    private void Awake()
    {
        Physics2D = GetComponent<EHPhysics2D>();
        AssociatedAnimator = GetComponent<Animator>();
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

    
    /// <summary>
    /// This method will be call
    /// </summary>
    public void OnInteractWithOtherDamageableComponent(EHDamageableComponent DamageableComponentWeHit)
    {
        Debug.Log(this.name + " Hit " + DamageableComponentWeHit.name);
        int DamageToApply = 0;
        DamageableComponentWeHit.TakeDamage(this, DamageToApply);
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
}

[System.Serializable]
public struct FAttackData
{
    public float AttackDamage;
    public float HitStunTime;
    public float HitFreezeTime;
    public Vector2 LaunchForce;
}