using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles all the logic for our character's movement. This include movemet in the air as well as specific movement options while grounded
/// </summary>
[RequireComponent(typeof(EHPhysics2D))]
[RequireComponent(typeof(EHBox2DCollider))]
public class EHMovementComponent : MonoBehaviour
{
    #region const values
    private const string ANIM_MOVEMENT_STATE = "MovementState";
    private const string ANIM_MOVEMENT_STATE_UPDATED = "MovementStateTrigger";

    private const string ENVIRONMENT_LAYER = "Environment";

    private const string ANIM_HORIZONTAL_INPUT = "HorizontalInput";

    private const string ANIM_HORIZONTAL_VELOCITY = "HorizontalSpeed";
    private const string ANIM_VERTICAL_VELOCITY = "VerticalSpeed";
    #endregion const values

    #region enums
    public enum EMovementType : byte
    {
        STANDING = 0x00,
        CROUCH = 0x01,
        IN_AIR = 0x02,
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

    #region main variables
    [Header("Orientation Variables")]
    [SerializeField]
    private bool IsFacingLeft = false;
    [SerializeField]
    private SpriteRenderer CharacterSpriteRenderer;

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

    //Animation Controlled variables
    [HideInInspector]
    public bool bIsAnimationControlled = false;
    [HideInInspector]
    public Vector2 AnimatedGoalVelocity = Vector2.zero;

    #endregion main variables

    private EHPhysics2D Physics2D;
    private Vector2 PreviousVelocity;
    private EHBox2DCollider AssociatedCollider;
    private Vector2 CurrentMovementInput = Vector2.zero;
    private Vector2 PreviousMovementInput = Vector2.zero;

    // The current movement type of our character
    private EMovementType CurrentMovementType;
    // The number of double jumps remainging. This should reset every time we land
    private int RemainingDoubleJumps;
    // Reference to the animator component
    private Animator CharacterAnimator;

    #region animation values

    #endregion animation values


    #region monobehaviour methods
    protected virtual void Awake()
    {
        Physics2D = GetComponent<EHPhysics2D>();
        AssociatedCollider = GetComponent<EHBox2DCollider>();
        StandingHeight = AssociatedCollider.ColliderSize.y;

        if (Physics2D == null) Debug.LogError("There is no associated with physics component with our movement component");
        if (AssociatedCollider == null) Debug.LogError("There is no associated Collider2D component associated with our movement component");

        AssociatedCollider.OnCollision2DBegin += OnEHCollisionEnter;
        CharacterAnimator = GetComponent<Animator>();
        CachedGravityScale = Physics2D.GravityScale;
        SetIsFacingLeft(IsFacingLeft, true);
    }

    protected virtual void Update()
    {
        UpdateMovementVelocity();
        PreviousMovementInput = CurrentMovementInput;
        UpdateMovementBasedOnMovementType();
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

    protected virtual void OnValidate()
    {
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

        if (CharacterSpriteRenderer == null)
        {
            CharacterSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (CharacterSpriteRenderer)
        {
            if (IsFacingLeft && CharacterSpriteRenderer.transform.localScale.x > 0)
            {
                SetIsFacingLeft(IsFacingLeft, true);
            }
            if (!IsFacingLeft && CharacterSpriteRenderer.transform.localScale.x < 0)
            {
                SetIsFacingLeft(IsFacingLeft, true);
            }
        }
    }
    #endregion monobehaviour methods

    #region input methods
    /// <summary>
    /// Sets the horizontal input for our movement component
    /// </summary>
    /// <param name="HorizontalInput"></param>
    public void SetHorizontalInput(float HorizontalInput)
    {
        CurrentMovementInput.x = Mathf.Clamp(HorizontalInput, -1f, 1f);

        if (CurrentMovementInput.x < 0 && !GetIsFacingLeft())
        {
            SetIsFacingLeft(true);
        }
        else if (CurrentMovementInput.x > 0 && GetIsFacingLeft())
        {
            SetIsFacingLeft(false);
        }

        if (CharacterAnimator)
        {
            CharacterAnimator.SetFloat(ANIM_HORIZONTAL_INPUT, Mathf.Abs(CurrentMovementInput.x));
        }
    }

    /// <summary>
    /// Sets the vertical input for our movement component
    /// </summary>
    /// <param name="VerticalInput"></param>
    public void SetVerticalInput(float VerticalInput)
    {
        CurrentMovementInput.y = Mathf.Clamp(VerticalInput, -1f, 1f);
    }

    /// <summary>
    /// Returns the current input for our movement component
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMovementInput()
    {
        return CurrentMovementInput;
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
                if (AttemptStand()) Jump();
                return;
            case EMovementType.IN_AIR:
                if (RemainingDoubleJumps-- > 0)
                {
                    Jump();
                }
                return;
        }
    }
    #endregion input methods

    /// <summary>
    /// TO-DO why do we use CurrentSpeed? Look into this later
    /// </summary>
    private void UpdateMovementVelocity()
    {
        float GoalSpeed = 0;
        float Acceleration = 0;
        float CurrentSpeed = Physics2D.Velocity.x;
        switch (CurrentMovementType)
        {
            case EMovementType.STANDING:
                Acceleration = GroundAcceleration;
                if (Mathf.Abs(CurrentMovementInput.x) > JOYSTICK_RUN_THRESHOLD)
                {
                    GoalSpeed = RunningSpeed;
                }
                else if (Mathf.Abs(CurrentMovementInput.x) > JOYSTICK_WALK_THRESHOLD)
                {
                    GoalSpeed = WalkingSpeed;
                }
                break;
            case EMovementType.CROUCH:
                Acceleration = GroundAcceleration;
                if (Mathf.Abs(CurrentMovementInput.x) > JOYSTICK_WALK_THRESHOLD)
                {
                    GoalSpeed = CrouchingSpeed;
                }
                break;
            case EMovementType.IN_AIR:
                Acceleration = HorizontalAirAcceleration;
                if (Mathf.Abs(CurrentMovementInput.x) > JOYSTICK_WALK_THRESHOLD)
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
        }
        else
        {
            GoalSpeed *= Mathf.Sign(CurrentMovementInput.x);
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
    private void UpdateMovementBasedOnMovementType()
    {
        switch (CurrentMovementType)
        {
            case EMovementType.STANDING:
                if (CurrentMovementInput.y < -JOYSTICK_CROUCH_THRESHOLD)
                {
                    SetMovementType(EMovementType.CROUCH);
                }
                return;
            case EMovementType.CROUCH:
                if (CurrentMovementInput.y > -JOYSTICK_CROUCH_THRESHOLD)
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

    
    /// <summary>
    /// This will set the velocity of our character to perform a jump
    /// </summary>
    private void Jump()
    {
        Physics2D.GravityScale = CachedGravityScale;
        Physics2D.Velocity = new Vector2(Physics2D.Velocity.x, JumpVelocity);
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
    public void OnLanded()
    {
        SetMovementType(EMovementType.STANDING);
        Physics2D.GravityScale = CachedGravityScale;
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
    /// Set the orientation of the character
    /// </summary>
    /// <param name="bIsFacingLeft"></param>
    /// <param name="bForceFaceDirection"></param>
    public void SetIsFacingLeft(bool bIsFacingLeft, bool bForceFaceDirection = false)
    {
        if (bIsFacingLeft == this.IsFacingLeft && !bForceFaceDirection)
        {
            return;
        }
        this.IsFacingLeft = bIsFacingLeft;
        Transform CharacterSpriteTransform = CharacterSpriteRenderer.transform;
        float XScale = Mathf.Abs(CharacterSpriteTransform.localScale.x);
        if (bIsFacingLeft)
        {
            CharacterSpriteTransform.localScale = new Vector3(-XScale, CharacterSpriteTransform.localScale.y, CharacterSpriteTransform.localScale.z);
        }
        else
        {
            CharacterSpriteTransform.localScale = new Vector3(XScale, CharacterSpriteTransform.localScale.y, CharacterSpriteTransform.localScale.z);
        }
    }

    /// <summary>
    /// Returns true if our character facing to the left.
    /// </summary>
    /// <returns></returns>
    public bool GetIsFacingLeft() { return this.IsFacingLeft; }
}
