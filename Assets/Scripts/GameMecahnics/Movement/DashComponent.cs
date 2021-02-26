using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dash component handles all the logic required for successfully performing a dash
/// </summary>
[RequireComponent(typeof(EHPhysics2D))]
[RequireComponent(typeof(EHMovementComponent))]
public class DashComponent : MonoBehaviour
{
    #region const variables
    private const float DASH_INPUT_THRESHOLD = .45f;
    private const string ANIM_DASH_TRIGGER = "Dash";
    private const string ANIM_IS_DASHING = "IsDashing";
    #endregion const variables

    [Tooltip("The initial speed of our dash")]
    public float DashSpeed = 5f;
    [Tooltip("The total amount of time that we will perform our dash")]
    public float DashTime = .016f * 4;
    [Tooltip("The delay before we start our dash animation")]
    public float DelayBeforeStartDash = 10f;
    [Tooltip("The amount of time to wait before you can use dash again")]
    public float DashCoolDownTime = .1f;
    [Tooltip("The amount of drag that we will apply to our chaaracter when it is performing our dash cool down")]
    public float DragValueForDashCoolDown = 10;
    [Tooltip("Curve will determine the velocity of our character when using the dash ability")]
    public AnimationCurve DashAnimationCurve;
    public int MaxDashesBeforeLanding = 1;

    /// <summary>
    /// True during our dash. Indicates that we can perform another dash if avaiable. This will be false during our dash cooldown
    /// </summary>
    private bool bIsPerformingDash;
    private EHMovementComponent MovementComponent;
    private EHDamageableComponent DamageComponent;
    private EHPhysics2D Physics2D;
    private Animator CharacterAnim;
    private int DashesRemaining;

    #region monobehaviour methods
    private void Awake()
    {
        MovementComponent = GetComponent<EHMovementComponent>();
        DamageComponent = GetComponent<EHDamageableComponent>();
        Physics2D = GetComponent<EHPhysics2D>();
        CharacterAnim = GetComponent<Animator>();
        MovementComponent.OnCharacterLanded += OnCharacterLanded;
        DashesRemaining = MaxDashesBeforeLanding;
        if (DamageComponent)
        {
            DamageComponent.OnCharacterHealthChanged += OnCharacterHealthChanged;
        }
    }

    private void OnValidate()
    {
        if (MaxDashesBeforeLanding < 1)
        {
            MaxDashesBeforeLanding = 1;
        }
    }
    #endregion monobehaviour methods

    /// <summary>
    /// Peforms our character's dash if valid
    /// </summary>
    public void InputDash()
    {
        if (!bIsPerformingDash && (!MovementComponent.GetIsInAir() || DashesRemaining >= 1))
        {
            print(DashesRemaining);
            CharacterAnim.SetTrigger(ANIM_DASH_TRIGGER);
        }
    }

    public void CancelDash()
    {
        CharacterAnim.ResetTrigger(ANIM_DASH_TRIGGER);
    }

    private void BeginActualDash()
    {
        --DashesRemaining;
        BaseGameOverseer.Instance.GlobalEffectManager.StartFreezeTimeForSeconds(DelayBeforeStartDash);
        StartCoroutine(BeginDash());
    }

    /// <summary>
    /// Carries out the steps of our dash
    /// </summary>
    /// <returns></returns>
    private IEnumerator BeginDash()
    {
        bIsPerformingDash = true;
        CharacterAnim.SetBool(ANIM_IS_DASHING, bIsPerformingDash);
        yield return StartCoroutine(PerformDash());

        if (bIsPerformingDash)
        {
            bIsPerformingDash = false;
            yield return StartCoroutine(PerformDashCoolDown());
        }
        CharacterAnim.SetBool(ANIM_IS_DASHING, bIsPerformingDash);
    }

    /// <summary>
    /// Coroutine that performs our character's dash based on the direction of their input and the dash
    /// animation curve
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformDash()
    {
        Vector2 MovementInputAxis = MovementComponent.GetMovementInput();

        Vector2 DashDirection;
        if (MovementInputAxis == Vector2.zero)
        {
            MovementInputAxis = Vector2.right * (MovementComponent.GetIsFacingLeft() ? -1 : 1);
        }
        DashDirection = MovementInputAxis.normalized;

        float TimeThatHasPassed = 0;
        float CachedGravity = Physics2D.GravityScale;
        
        Physics2D.GravityScale = 0;
        while (TimeThatHasPassed < DashTime && bIsPerformingDash)
        {
            Physics2D.Velocity = DashAnimationCurve.Evaluate(TimeThatHasPassed / DashTime) * DashDirection * DashSpeed;
            yield return null;
            TimeThatHasPassed += EHTime.DeltaTime;
        }
        Physics2D.GravityScale = CachedGravity;
    }

    /// <summary>
    /// Performed after our dash has completed. Adds a much stronger drag for our character's gravity so that they don't just zoom into the sky.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformDashCoolDown()
    {
        float TimeThatHasPassed = 0;
        Vector2 PreviousVelocity = Physics2D.Velocity;
        while (TimeThatHasPassed < DashCoolDownTime && !bIsPerformingDash && PreviousVelocity.y <= Physics2D.Velocity.y)
        {
            TimeThatHasPassed += EHTime.DeltaTime;
            if (Physics2D.Velocity.y > 0)
            {
                Physics2D.Velocity -= EHTime.DeltaTime * DragValueForDashCoolDown * Vector2.up;
            }
            yield return null;
        }
        yield break;
    }

    private void OnCharacterLanded()
    {
        DashesRemaining = MaxDashesBeforeLanding;
    }

    private void OnCharacterHealthChanged(FDamageData DamageData)
    {
        if (DamageData.DamageType != EHDamageableComponent.EDamageType.HEALING)
        {
            bIsPerformingDash = false;
        }
    }
}
