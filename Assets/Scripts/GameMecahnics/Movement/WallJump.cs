using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJump : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField]
    private float FrictionScale = 0.5f;
    [SerializeField]
    private float MaxFallSpeed = 3f;
    private EHPhysics2D Physics2D;
    private EHMovementComponent CharacterMovement;
    private EHBox2DCollider CharacterCollider;

    private EHBaseCollider2D ColliderWeAreOn;

    #region monobehaviour methods
    private void Awake()
    {
        CharacterMovement = GetComponent<EHMovementComponent>();
        CharacterCollider = GetComponent<EHBox2DCollider>();
        Physics2D = GetComponent<EHPhysics2D>();
        if (CharacterCollider)
        {
            CharacterCollider.OnCollision2DBegin += OnCharacterCollision;
            //To-do: looks like this is not being called.
            CharacterCollider.OnCollision2DEnd += OnCharacterCollisionEnd;
        }
    }

    private void Update()
    {
        if (ColliderWeAreOn)
        {
            if (Physics2D.Velocity.y < -MaxFallSpeed)
            {
                Physics2D.Velocity = new Vector2(0, -MaxFallSpeed);
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
    }

    private void OnCharacterCollisionEnd(FHitData HitData)
    {
        print(HitData.OtherCollider.name);
        if (HitData.OtherCollider == ColliderWeAreOn)
        {
            ColliderWeAreOn = null;
        }
    }
}
