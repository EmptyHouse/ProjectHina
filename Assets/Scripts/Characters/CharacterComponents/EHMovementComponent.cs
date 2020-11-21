using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EHPhysics2D))]
public class EHMovementComponent : MonoBehaviour
{
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
    public float JumpHeightApex = 5;
    public float TimeToReachApex = .5f;
    private float JumpVelocity;

    #endregion main variables

    private EHPhysics2D Physics2D;
    private Vector2 CurrentMovementInput = Vector2.zero;
    private Vector2 PreviousMovementInput = Vector2.zero;
    private float CurrentSpeed;

    #region monobehaviour methods
    protected virtual void Awake()
    {
        Physics2D = GetComponent<EHPhysics2D>();
    }

    protected virtual void Update()
    {
        UpdateMovementVelocity();
        PreviousMovementInput = CurrentMovementInput;
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

    public void UpdateMovementVelocity()
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

    public void BeginJump()
    {
        Physics2D.Velocity = new Vector2(Physics2D.Velocity.x, JumpVelocity);
    }

    public void EndJump()
    {

    }
}
