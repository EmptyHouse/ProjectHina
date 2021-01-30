using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all bomb type projectiles
/// </summary>
public class BombProjectile : EHBaseProjectile
{
    private const string EXPLODE_ANIM = "Explode";

    #region main variables
    [SerializeField]
    protected float BombRadius;
    #endregion main varaibles

    #region monobehavoiur methods
    protected override void Awake()
    {
        base.Awake();
        BombRadius = GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2f;
    }

    private void Update()
    {
        CheckIntersectWithOtherCollider();
    }
    #endregion monobeahviour methods
    /// <summary>
    /// 
    /// </summary>
    private void CheckIntersectWithOtherCollider()
    {
        const float BombAngle = 180f;
        float AngleIncrement = BombAngle / (RayTraceCount - 1);
        Vector2 InitialMovementDirection = Physics.Velocity * EHTime.DeltaTime;
        Vector2 BombTransformPosition = this.transform.position;
        float CurrentAngle = Mathf.Atan2(InitialMovementDirection.y, InitialMovementDirection.x) * Mathf.Rad2Deg - 90;

        EHRayTraceParams RayParams = default;
        EHRayTraceHit RayHit;
        for (int i = 0; i < RayTraceCount; ++i)
        {
            RayParams.RayOrigin = BombTransformPosition + new Vector2(Mathf.Cos(CurrentAngle * Mathf.Deg2Rad) * BombRadius, Mathf.Sin(CurrentAngle * Mathf.Deg2Rad)) * BombRadius;
            RayParams.RayDirection = InitialMovementDirection;
            RayParams.RayLength = InitialMovementDirection.magnitude;
            if (EHPhysicsManager2D.RayTrace2D(ref RayParams, out RayHit, 0, true))
            {
                Explode();
                return;
            }
            CurrentAngle += AngleIncrement;
        }
    }

    public override void LaunchProjectile(Vector2 VelocityOfLaunch)
    {
        
    }

    /// <summary>
    /// Call this method when we have collided with any other collider
    /// </summary>
    public void Explode()
    {
        Physics.enabled = false;
        ProjectileAnim.SetTrigger(EXPLODE_ANIM);
    }
}
