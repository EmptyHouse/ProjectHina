using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EHPhysics2D : MonoBehaviour, ITickableComponent
{
    #region enums
    #endregion enums
    public const float GRAVITATIONAL_CONSTANT = 9.8f;
    public Vector2 Velocity { get; set; }
    public float GravityScale = 1f;
    public bool bUseTerminalVelocity = true;
    public float TerminalVelocity = 10;
    private Vector2 GravityDirection = Vector2.down;


    #region monobehaviour methods
    private void Awake()
    {
        BaseGameOverseer.Instance.PhysicsManager.AddPhysicsComponent(this);
        EHBaseCollider2D AttachedCollider = GetComponent<EHBaseCollider2D>();

        if (AttachedCollider)
        {
            AttachedCollider.OnCollision2DStay += OnEHCollisionStay;
        }
    }


    private void OnDestroy()
    {
        if (BaseGameOverseer.Instance && BaseGameOverseer.Instance.PhysicsManager != null)
        {
            BaseGameOverseer.Instance.PhysicsManager.RemovePhysicsComeponent(this);
        }

        EHBaseCollider2D AttachedCollider = GetComponent<EHBaseCollider2D>();
        if (AttachedCollider)
        {
            AttachedCollider.OnCollision2DStay -= OnEHCollisionStay;
        }
    }
    #endregion monobehaviour methods

    public void SetGravityDirection(Vector2 GravityDirection)
    {
        this.GravityDirection = GravityDirection.normalized;
    }

    /// <summary>
    /// All logic that would run in the update loop of our physics compoonent should be handled here
    /// </summary>
    /// <param name="DeltaTime"></param>
    public void Tick(float DeltaTime)
    {
        Velocity += GravityDirection * (GRAVITATIONAL_CONSTANT * DeltaTime * GravityScale);

        UpdatePositionBasedOnVelociity(DeltaTime);
    }

    public void UpdatePositionBasedOnVelociity(float DeltaTime)
    {
        this.transform.position += new Vector3(Velocity.x, Velocity.y) * DeltaTime;
    }

    /// <summary>
    /// This methods will be called every time we intersect with a collider to indicate wither or not we are currently in the air or not. 
    /// </summary>
    /// <param name="HitData"></param>
    private void OnEHCollisionStay(FHitData HitData)
    {
        if (HitData.HitDirection.y != 0 && Mathf.Sign(HitData.HitDirection.y) != Mathf.Sign(Velocity.y))
        {
            Velocity = new Vector2(Velocity.x, 0);
        }
        if (HitData.HitDirection.x != 0 && Mathf.Sign(HitData.HitDirection.x) != Mathf.Sign(Velocity.x))
        {
            Velocity = new Vector2(0, Velocity.y);
        }
    }
}
