using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHParry : MonoBehaviour
{
    private const string ANIM_PARRY = "Parry";
    private const string ANIM_PARRY_SUCCESS = "ParrySuccess";

    [SerializeField]
    private float CooldownTimeForParry = .8f; 
    private EHDamageableComponent DamageComponent;
    private Animator CharacterAnimator;
    private bool bCanParry;

    #region animation variables
    public bool bIsAnimParrying;
    #endregion animation variables

    private void Awake()
    {
        DamageComponent = GetComponent<EHDamageableComponent>();
        DamageComponent.OnCharacterHealthChanged += OnCharacterHit;
        CharacterAnimator = GetComponent<Animator>();

        bCanParry = true;
    }



    public void ParryInputDown()
    {
        if (bCanParry)
        {
            CharacterAnimator.SetTrigger(ANIM_PARRY);
        }
    }

    public void ParryInputReleased()
    {
        CharacterAnimator.ResetTrigger(ANIM_PARRY);
    }

    /// <summary>
    /// Call this function to begin a parry stance from the animator
    /// </summary>
    public void OnBeginParry()
    {
        bCanParry = false;
        DamageComponent.SetIsInvincible(true);
        StartCoroutine(ParryCoroutine());
    }

    /// <summary>
    /// Safely ends the parry stance
    /// </summary>
    private void EndParry()
    {
        DamageComponent.SetIsInvincible(false);
    }

    private void ParrySuccess()
    {

    }

    private IEnumerator ParryCoroutine()
    {
        yield return null;

        while (bIsAnimParrying)
        {
            yield return null;
        }
        EndParry();
        float TimeThatHasPassed = 0;
        while (TimeThatHasPassed < CooldownTimeForParry)
        {
            yield return null;
            TimeThatHasPassed += EHTime.DeltaTime;

        }
        bCanParry = true;
        print("I made it here");
    }

    private void OnCharacterHit(FDamageData DamageData)
    {
        if (bIsAnimParrying)
        {
            CharacterAnimator.SetTrigger(ANIM_PARRY_SUCCESS);
        }
    }
}
