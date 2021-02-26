using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Handles all the logic for our character's movement. This include movemet in the air as well as specific movement options while grounded
/// </summary>
[RequireComponent(typeof(EHBox2DCollider))]
public class EHMovementComponent : EHBaseMovementComponent
{
    #region const values
    private const string ANIM_MOVEMENT_STATE = "MovementState";
    private const string ANIM_MOVEMENT_STATE_UPDATED = "MovementStateTrigger";

    private const string ENVIRONMENT_LAYER = "Environment";

    private const string ANIM_HORIZONTAL_INPUT = "HorizontalInput";

    private const string ANIM_HORIZONTAL_VELOCITY = "HorizontalSpeed";
    private const string ANIM_VERTICAL_VELOCITY = "VerticalSpeed";
    private const string ANIM_JUMP = "Jump";
    #endregion const values

    #region enums
    public enum EMovementType : byte
    {
        STANDING = 0x00,
        CROUCH = 0x01,
        IN_AIR = 0x02,
        FLYING = 0x03
    }
    #endregion enums

    #region const values
    // Value required before we can register that our character should begin walking
    private const float JOYSTICK_WALK_THRESHOLD = .25f;
    // Value reuired on our joystick before we register that our character is at running speed
    private const float JOYSTICK_RUN_THRESHOLD = .7f;
    // Value required in the down position before we register that our character is crouching
    private const float JOYSTICK_CROUCH_THRESHOLD = .6f;
    #endregion const values

    #region events
    public UnityAction OnCharacterLanded;
    #endregion events

    #region main variables

    [Header("Walking")]
    [Tooltip("The max walking speed of our character")]
    public float WalkingSpeed = 5f;
    [Tooltip("The max running speed of our character")]
    public float RunningSpeed = 11f;
    [Tooltip("Acceleration of movement while our character is standing")]
    public float GroundAcceleration = 100f;

    [Header("Crouching")]
    [Tooltip("The acceleration of our character while they are crouching")]
    public float CrouchingAcceleration = 100f;
    [Tooltip("The max speed of our character while they are crouched")]
    public float CrouchingSpeed = 5f;
    [Tooltip("The percentage at which we will shorten the height of our character's collider while they are crouched")]
    public float CrouchingHeight = .6f;
    private float StandingHeight;

    [Header("Jumping")]
    [Tooltip("The acceleration or control that the character will have while they are in the air")]
    public float HorizontalAirAcceleration = 100f;
    [Tooltip("The speed at which our character will try to move while in the air")]
    public float AirSpeed;
    public int DoubleJumpCount = 1;
    [Tooltip("The max height of our jump")]
    public float JumpHeightApex = 5;
    [Tooltip("The time it will take us in seconds to reach that height")]
    public float TimeToReachApex = .5f;
    [Tooltip("The multiplier to gravity that we will apply when we are falling")]
    public float LowJumpMultiplier = 2f;

    // The velocity at which we will perform our jump
    [HideInInspector]
    [SerializeField]
    private float JumpVelocity;
    // The original gravity multiplier before applying any multipliers
    private float CachedGravityScale;
    private Vector2 PreviousVelocity;
    private EHBox2DCollider AssociatedCollider;

    // The current movement type of our character
    private EMovementType CurrentMovementType;
    // The number of double jumps remainging. This should reset every time we land
    private int RemainingDoubleJumps;
    #endregion main variables


    #region monobehaviour methods
    protected override void Awake()
    {
        base.Awake();
        AssociatedCollider = GetComponent<EHBox2DCollider>();
        StandingHeight = AssociatedCollider.ColliderSize.y;

        if (Physics2D == null) Debug.LogError("There is no associated physics component with our movement component");
        if (AssociatedCollider == null)
        {
            Debug.LogError("There is no associated Collider2D component associated with our movement component");
            return;
        }
        AssociatedCollider.OnCollision2DBegin += OnEHCollisionEnter;
        CachedGravityScale = Physics2D.GravityScale;
    }

    protected override void Update()
    {
        base.Update();
        if (CurrentMovementType != EMovementType.IN_AIR && Mathf.Abs(Physics2D.Velocity.y) > 0)
        {
            SetMovementType(EMovementType.IN_AIR);
        }
        PreviousVelocity = Physics2D.Velocity;
    }

    protected virtual void OnDestroy()
    {
        EHBaseCollider2D BaseCollider = GetComponent<EHBaseCollider2D>();
        if (BaseCollider)
        {
            BaseCollider.OnCollision2DBegin -= OnEHCollisionEnter;
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        if (WalkingSpeed < 0)
        {
            WalkingSpeed = 0;
        }

        if (RunningSpeed < 0)
        {
            RunningSpeed = 0;
        }

        if (DoubleJumpCount < 0)
        {
            DoubleJumpCount = 0;
        }

        if (JumpHeightApex < 0)
        {
            JumpHeightApex = 0;
        }

        if (TimeToReachApex < 0)
        {
            TimeToReachApex = 0;
        }

        if (!Physics2D)
        {
            Physics2D = GetComponent<EHPhysics2D>();
        }

        if (TimeToReachApex != 0)
        {
            Physics2D.GravityScale = (2 * JumpHeightApex) / (EHPhysics2D.GRAVITATIONAL_CONSTANT * Mathf.Pow(TimeToReachApex, 2));
            JumpVelocity = 2 * JumpHeightApex / TimeToReachApex;
        }
    }
    #endregion monobehaviour methods

    #region input methods
    public void InputJump()
    {
        if (RemainingDoubleJumps > 0 || CurrentMovementType != EMovementType.IN_AIR)
        {
            CharacterAnimator.SetTrigger(ANIM_JUMP);
        }
    }

    public void ReleaseInputJump()
    {
        CharacterAnimator.SetBool(ANIM_JUMP, false);
    }

    /// <summary>
    /// This method will make our character jump if it is valid
    /// </summary>
    /// <returns></returns>
    public void AttemptJump()
    {
        switch (CurrentMovementType)
        {
            case EMovementType.STANDING:
                Jump();
                return;
            case EMovementType.CROUCH:
                if (AttemptDropDownJump())
                {

                }
                else if (AttemptStand())
                {
                    Jump();
                }
                return;
            case EMovementType.IN_AIR:
                if (RemainingDoubleJumps-- > 0)
                {
                    Jump();
                }
                return;
        }
    }

    private bool CanJump()
    {
        return false;
    }
    #endregion input methods

    /// <summary>
    /// TO-DO why do we use CurrentSpeed? Look into this later
    /// </summary>
    protected override void UpdateVelocityFromInput(Vector2 CurrentInput, Vector2 PreviousInput)
    {
        float GoalSpeed = 0;
        float Acceleration = 0;
        float CurrentSpeed = Physics2D.Velocity.x;
        switch (CurrentMovementType)
        {
            case EMovementType.STANDING:
                Acceleration = GroundAcceleration;
                if (Mathf.Abs(CurrentInput.x) > JOYSTICK_RUN_THRESHOLD)
                {
                    GoalSpeed = RunningSpeed;
                }
                else if (Mathf.Abs(CurrentInput.x) > JOYSTICK_WALK_THRESHOLD)
                {
                    GoalSpeed = WalkingSpeed;
                }
                break;
            case EMovementType.CROUCH:
                Acceleration = GroundAcceleration;
                if (Mathf.Abs(CurrentInput.x) > JOYSTICK_WALK_THRESHOLD)
                {
                    GoalSpeed = CrouchingSpeed;
                }
                break;
            case EMovementType.IN_AIR:
                Acceleration = HorizontalAirAcceleration;
                if (Mathf.Abs(CurrentInput.x) > JOYSTICK_WALK_THRESHOLD)
                {
                    GoalSpeed = AirSpeed;
                }
                else
                {
                    GoalSpeed = CurrentSpeed;
                }
                break;
        }
       if (bIsAnimationControlled)
        {
            GoalSpeed = AnimatedGoalVelocity.x * Mathf.Sign(CharacterSpriteRenderer.transform.localScale.x);
            Acceleration /= 3;//Remember to remove this. Just for testing.
        }
        else
        {
            GoalSpeed *= Mathf.Sign(CurrentInput.x);
        }

        CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, GoalSpeed, EHTime.DeltaTime * Acceleration);
        Physics2D.Velocity = new Vector2(CurrentSpeed, Physics2D.Velocity.y);
        if (CharacterAnimator)
        {
            CharacterAnimator.SetFloat(ANIM_HORIZONTAL_VELOCITY, Physics2D.Velocity.x);
            CharacterAnimator.SetFloat(ANIM_VERTICAL_VELOCITY, Physics2D.Velocity.y);
        }
    }

    /// <summary>
    /// Updates the movmeent component based on the state
    /// </summary>
    protected override void UpdateMovementTypeFromInput(Vector2 CurrentInput)
    {
        switch (CurrentMovementType)
        {
            case EMovementType.STANDING:
                if (CurrentInput.y < -JOYSTICK_CROUCH_THRESHOLD)
                {
                    SetMovementType(EMovementType.CROUCH);
                }
                return;
            case EMovementType.CROUCH:
                if (CurrentInput.y > -JOYSTICK_CROUCH_THRESHOLD)
                {
                    //Attempt to stand if the player has gone above the crouch threshold
                    AttemptStand();
                }
                return;
            case EMovementType.IN_AIR:
                if (PreviousVelocity.y >= 0 && Physics2D.Velocity.y < 0)
                {
                    EndJump();
                }
                return;
        }
    }

    /// <summary>
    /// Updates the movement type of our character. If we return true, it means that our movement type has
    /// changed
    /// </summary>
    /// <returns></returns>
    private void SetMovementType(EMovementType NewMovementType)
    {
        if (NewMovementType == CurrentMovementType)
        {
            return;
        }
        int PreviousMovementType = (int)CurrentMovementType;
        EndMovementType(CurrentMovementType);
        CurrentMovementType = NewMovementType;

        switch (CurrentMovementType)
        {
            case EMovementType.STANDING:
                RemainingDoubleJumps = DoubleJumpCount;
                break;
            case EMovementType.CROUCH:
                AssociatedCollider.ColliderSize.y = CrouchingHeight;
                break;
            case EMovementType.IN_AIR:

                break;
        }
        CharacterAnimator.SetInteger(ANIM_MOVEMENT_STATE, (int)CurrentMovementType);
        CharacterAnimator.SetTrigger(ANIM_MOVEMENT_STATE_UPDATED);
    }

    /// <summary>
    /// Call this method when leaving a movement state
    /// </summary>
    /// <param name="MovementTypeToEnd"></param>
    private void EndMovementType(EMovementType MovementTypeToEnd)
    {
        switch (MovementTypeToEnd)
        {
            case EMovementType.STANDING:

                break;
            case EMovementType.CROUCH:
                AssociatedCollider.ColliderSize.y = StandingHeight;
                break;
            case EMovementType.IN_AIR:

                break;
        }
    }

    /// <summary>
    /// Checks to see if you have enough room to stand and then will change the state to stand if valid
    /// </summary>
    /// <returns></returns>
    private bool AttemptStand()
    {
        EHRect2D CastBox = new EHRect2D();
        CastBox.RectPosition = AssociatedCollider.GetBounds().MinBounds;
        CastBox.RectSize = AssociatedCollider.DefaultColliderSize;
        EHBaseCollider2D HitCollider;
        if (!EHPhysicsManager2D.BoxCast2D(ref CastBox, out HitCollider, LayerMask.GetMask(ENVIRONMENT_LAYER)))
        {
            SetMovementType(EMovementType.STANDING);
            return true;
        }
        return false;
    }

    public bool AttemptDropDownJump()
    {
        if (CurrentMovementType != EMovementType.CROUCH)
        {
            return false;
        }
        EHBounds2D ColliderBounds = AssociatedCollider.GetBounds();
        EHRect2D CastBox = new EHRect2D();
        CastBox.RectPosition = ColliderBounds.MinBounds + Vector2.down * .1f;
        CastBox.RectSize = new Vector2(ColliderBounds.MaxBounds.x - ColliderBounds.MinBounds.x, .1f);
        if (EHPhysicsManager2D.BoxCast2D(ref CastBox, out EHBaseCollider2D ColliderWeHit, LayerMask.GetMask(ENVIRONMENT_LAYER)))
        {
            if (ColliderWeHit is EHOneSidedBoxCollider2D)
            {
                DropDownJump();
                return true;
            }
        }
        return false;
    }

    private void DropDownJump()
    {
        transform.position += Vector3.down * .1f;
        AssociatedCollider.UpdateColliderBounds(true);//Ignores the sweep of our collider so they can move through our platform
    }
    
    /// <summary>
    /// This will set the velocity of our character to perform a jump
    /// </summary>
    private void Jump()
    {
        Physics2D.GravityScale = CachedGravityScale;
        Physics2D.Velocity = new Vector2(Physics2D.Velocity.x, JumpVelocity);
    }

    public bool GetIsInAir()
    {
        return CurrentMovementType == EMovementType.IN_AIR;
    }

    /// <summary>
    /// When this method is called it will begin 
    /// </summary>
    public void EndJump()
    {
        if (CurrentMovementType == EMovementType.IN_AIR)
        {
            Physics2D.GravityScale = CachedGravityScale * LowJumpMultiplier;
        }
    }

    /// <summary>
    /// This method will be called when our character lands after leaving the IN_AIR state
    /// </summary>
    private void OnLanded()
    {
        SetMovementType(EMovementType.STANDING);
        Physics2D.GravityScale = CachedGravityScale;
        OnCharacterLanded?.Invoke();
    }

    /// <summary>
    /// Event that is called when we begin a new collision
    /// </summary>
    /// <param name="HitData"></param>
    public void OnEHCollisionEnter(FHitData HitData)
    {
        if (HitData.HitDirection.y > 0 && CurrentMovementType == EMovementType.IN_AIR)
        {
            OnLanded();
        }
    }

    /// <summary>
    /// Returns whether or not our character can change directions
    /// </summary>
    /// <returns></returns>
    protected override bool CanChangeDirection()
    {
        return CurrentMovementType != EMovementType.IN_AIR && !bIsAnimationControlled;
    }

    public void OnTransformPositionInPixels(int x)
    {
        transform.position += Vector3.right * (Mathf.Sign(CharacterSpriteRenderer.transform.localScale.x) * x / 16f);
    }
}
