using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJump : MonoBehaviour
{
    #region const variables
    private const string ANIM_WALL_HOLD = "CanWallRide";

    #endregion const variables
    [Range(0f, 1f)]
    [SerializeField]
    private float FrictionScale = 0.5f;
    [SerializeField]
    private float MaxFallSpeed = 3f;
    [SerializeField]
    private Vector2 WallJumpVelocity = Vector2.one;
    private EHPhysics2D Physics2D;
    private EHMovementComponent CharacterMovement;
    private EHBox2DCollider CharacterCollider;
    private Animator CharacterAnim;

    private EHBaseCollider2D ColliderWeAreOn;
    private float CachedWallDirection;
    #region animation variables
    public bool bAnimIsWallRiding;
    #endregion animation variables

    #region monobehaviour methods
    private void Awake()
    {
        CharacterMovement = GetComponent<EHMovementComponent>();
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
            if (xInput * CachedWallDirection < EHMovementComponent.JOYSTICK_WALK_THRESHOLD)
            {
                if (CharacterAnim.GetBool(ANIM_WALL_HOLD))
                {
                     CharacterAnim.SetBool(ANIM_WALL_HOLD, false);
                    return;
                }
            }
            if (Physics2D.Velocity.y < -MaxFallSpeed)
            {
                Physics2D.Velocity = new Vector2(0, -MaxFallSpeed);
            }
        }
        else if (ColliderWeAreOn)
        {
            if (CachedWallDirection * CharacterMovement.GetMovementInput().x > EHMovementComponent.JOYSTICK_WALK_THRESHOLD)
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
    /// Call this 
    /// </summary>
    public void PerformWallJump()
    {
        if (ColliderWeAreOn)
        {
            Vector2 AdjustedLaunchVelocity = new Vector2(WallJumpVelocity.x * (CharacterMovement.GetIsFacingLeft() ? 1f : -1f), WallJumpVelocity.y);
            Physics2D.Velocity = AdjustedLaunchVelocity;
        }
    }

    private void OnCharacterCollision(FHitData HitData)
    {
        if (HitData.HitDirection.x > 0)
        {
            ColliderWeAreOn = HitData.OtherCollider;
        }
        else if (HitData.HitDirection.x < 0)
        {
            ColliderWeAreOn = HitData.OtherCollider;
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
