using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJump : MonoBehaviour
{
    #region const variables
    private const string ANIM_WALL_JUMP = "Jump";
    private const string ANIM_WALL_HOLD = "CanWallRide";

    #endregion const variables

    #region main varaibles
    [Tooltip("The speed that our character will fall when they are riding along the wall")]
    [SerializeField]
    private float MaxFallSpeed = 3f;
    [Tooltip("The velocity that our character will jump off of the wall from. Keep in mind that this value assumes that the character is jumping to the right")]
    [SerializeField]
    private Vector2 WallJumpVelocity = Vector2.one;

    // Component References
    private EHPhysics2D Physics2D;
    private EHCharacterMovementComponent CharacterMovement;
    private EHBox2DCollider CharacterCollider;
    private Animator CharacterAnim;

    private EHBaseCollider2D ColliderWeAreOn;
    private float CachedWallDirection;

    #endregion main varaibles

    #region animation variables
    public bool bAnimIsWallRiding;
    #endregion animation variables

    #region monobehaviour methods
    private void Awake()
    {
        CharacterMovement = GetComponent<EHCharacterMovementComponent>();
        CharacterCollider = GetComponent<EHBox2DCollider>();
        Physics2D = GetComponent<EHPhysics2D>();
        CharacterAnim = GetComponent<Animator>();
        if (CharacterCollider)
        {
            CharacterCollider.OnCollision2DBegin += OnCharacterCollision;
            CharacterCollider.OnCollision2DEnd += OnCharacterCollisionEnd;
        }
    }

    private void Update()
    {
        if (bAnimIsWallRiding)
        {
            float xInput = CharacterMovement.GetMovementInput().x;
            if (xInput * CachedWallDirection <= EHCharacterMovementComponent.JOYSTICK_WALK_THRESHOLD)
            {
                if (CharacterAnim.GetBool(ANIM_WALL_HOLD))
                {
                    CharacterAnim.SetBool(ANIM_WALL_HOLD, false);
                    return;
                }
            }
            if (Physics2D.Velocity.y < -MaxFallSpeed)
            {
                Physics2D.Velocity = new Vector2(Physics2D.Velocity.x, -MaxFallSpeed);
            }
        }
        else if (ColliderWeAreOn)
        {
            if (CachedWallDirection * CharacterMovement.GetMovementInput().x > EHCharacterMovementComponent.JOYSTICK_WALK_THRESHOLD)
            {
                if (!CharacterAnim.GetBool(ANIM_WALL_HOLD))
                {
                    CharacterAnim.SetBool(ANIM_WALL_HOLD, true);
                }
            }
        }
    }

    private void OnValidate()
    {
        if (MaxFallSpeed < 0)
        {
            MaxFallSpeed = 0;
        }
    }
    #endregion monobehaviour methods
    /// <summary>
    /// Begins our trigger that will allow us to perform a wall jump
    /// </summary>
    public void InputWallJump()
    { 
        if (bAnimIsWallRiding)
        {
            CharacterAnim.SetTrigger(ANIM_WALL_JUMP);
        }
    }

    /// <summary>
    /// Cancels our trigger to perform a wall jump
    /// </summary>
    public void InputCancelWallJump()
    {
        if (bAnimIsWallRiding)
        {
            CharacterAnim.ResetTrigger(ANIM_WALL_JUMP);
        }
    }

    /// <summary>
    /// Call this to actually perform a wall jump 
    /// </summary>
    public void PerformWallJump()
    {
        if (ColliderWeAreOn)
        {
            CharacterMovement.ResetCachedGravityScaled();
            Vector2 AdjustedLaunchVelocity = new Vector2(WallJumpVelocity.x * (CharacterMovement.GetIsFacingLeft() ? 1f : -1f), WallJumpVelocity.y);
            Physics2D.Velocity = AdjustedLaunchVelocity;

            CharacterMovement.SetIsFacingLeft(!CharacterMovement.GetIsFacingLeft(), true);
        }
    }

    private void OnCharacterCollision(FHitData HitData)
    {
        if (HitData.HitDirection.x > 0)
        {
            ColliderWeAreOn = HitData.OtherCollider;
            if (!CharacterMovement.GetIsFacingLeft())
            {
                CharacterMovement.SetIsFacingLeft(true, true);
            }
        }
        else if (HitData.HitDirection.x < 0)
        {
            ColliderWeAreOn = HitData.OtherCollider;
            if (CharacterMovement.GetIsFacingLeft())
            {
                CharacterMovement.SetIsFacingLeft(false, true);
            }
        }
        else
        {
            return;
        }
        CachedWallDirection = Mathf.Sign(ColliderWeAreOn.transform.position.x - this.transform.position.x);
        CharacterAnim.SetBool(ANIM_WALL_HOLD, true);
    }

    private void OnCharacterCollisionEnd(FHitData HitData)
    {
        if (HitData.OtherCollider == ColliderWeAreOn)
        {
            ColliderWeAreOn = null;
            CharacterAnim.SetBool(ANIM_WALL_HOLD, false);
        }
    }
}
