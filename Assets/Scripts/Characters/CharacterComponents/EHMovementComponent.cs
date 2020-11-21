using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EHPhysics2D))]
public class EHMovementComponent : MonoBehaviour
{
    #region enums
    public enum MovementType : byte
    {
        STANDING = 0x00,
        CROUCH = 0x01,
        IN_AIR = 0x02,
    }

    #endregion enums

    #region const values
    private const float JOYSTICK_WALK_THRESHOLD = .25f;
    private const float JOYSTICK_RUN_THRESHOLD = .7f;
    #endregion const values

    #region main variables
    [Header("Walking")]
    public float WalkingSpeed = 5f;
    public float RunningSpeed = 11f;
    public float GroundAcceleration = 100f;

    [Header("Jumping")]
    public int DoubleJumpCount = 1;
    public float JumpHeightApex = 5;
    public float TimeToReachApex = .5f;
    private float JumpVelocity;

    #endregion main variables

    private EHPhysics2D Physics2D;
    private Vector2 CurrentMovementInput = Vector2.zero;
    private Vector2 PreviousMovementInput = Vector2.zero;
    private float CurrentSpeed;
    private MovementType CurrentMovementType;
    private int RemainingDoubleJumps;

    #region monobehaviour methods
    protected virtual void Awake()
    {
        Physics2D = GetComponent<EHPhysics2D>();
        GetComponent<EHBaseCollider2D>().OnCollision2DEnter += OnEHCollisionEnter;
    }

    protected virtual void Update()
    {
        UpdateMovementVelocity();
        PreviousMovementInput = CurrentMovementInput;
        if (CurrentMovementType != MovementType.IN_AIR && Mathf.Abs(Physics2D.Velocity.y) > 0)
        {
            SetMovementType(MovementType.IN_AIR);
        }
    }

    protected virtual void OnDestroy()
    {
        EHBaseCollider2D BaseCollider = GetComponent<EHBaseCollider2D>();
        if (BaseCollider)
        {
            BaseCollider.OnCollision2DEnter -= OnEHCollisionEnter;
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
    }
    #endregion monobehaviour methods

    public void SetHorizontalInput(float HorizontalInput)
    {
        CurrentMovementInput.x = Mathf.Clamp(HorizontalInput, -1f, 1f);
    }

    public void SetVerticalInput(float VerticalInput)
    {
        CurrentMovementInput.y = Mathf.Clamp(VerticalInput, -1f, 1f);
    }

    private void UpdateMovementVelocity()
    {
        float GoalSpeed = 0;

        if (Mathf.Abs(CurrentMovementInput.x) > JOYSTICK_RUN_THRESHOLD)
        {
            GoalSpeed = Mathf.Sign(CurrentMovementInput.x) * RunningSpeed;
        }
        else if (Mathf.Abs(CurrentMovementInput.x) > JOYSTICK_WALK_THRESHOLD)
        {
            GoalSpeed = Mathf.Sign(CurrentMovementInput.x) * WalkingSpeed;
        }
        CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, GoalSpeed, EHTime.DELTA_TIME * GroundAcceleration);
        Physics2D.Velocity = new Vector2(CurrentSpeed, Physics2D.Velocity.y);
    }

    /// <summary>
    /// Updates the movement type of our character. If we return true, it means that our movement type has
    /// changed
    /// </summary>
    /// <returns></returns>
    private void SetMovementType(MovementType NewMovementType)
    {
        if (NewMovementType == CurrentMovementType)
        {
            return;
        }

        CurrentMovementType = NewMovementType;

        switch (CurrentMovementType)
        {
            case MovementType.STANDING:
                RemainingDoubleJumps = DoubleJumpCount;
                return;
            case MovementType.CROUCH:

                return;
            case MovementType.IN_AIR:

                return;
        }
    }

    public void BeginJump()
    {
        if (CurrentMovementType == MovementType.STANDING)
        {
            Physics2D.Velocity = new Vector2(Physics2D.Velocity.x, JumpVelocity);
        }
        else if (RemainingDoubleJumps-- > 0)
        {
            Physics2D.Velocity = new Vector2(Physics2D.Velocity.x, JumpVelocity);
        }
    }

    public void EndJump()
    {

    }

    public void OnEHCollisionEnter(EHBaseCollider2D.FHitData HitData)
    {
        if (HitData.HitDirection.y > 0 && CurrentMovementType == MovementType.IN_AIR)
        {
            SetMovementType(MovementType.STANDING);
        }
    }
}
