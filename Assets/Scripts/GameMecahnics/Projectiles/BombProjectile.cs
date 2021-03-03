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
    #endregion monobeahviour methods
    /// <summary>
    /// 
    /// </summary>
    protected override void CheckForIntersectionWithOtherColliders()
    {
        const float BombAngle = 180f;
        float AngleIncrement = BombAngle / (RayTraceCount - 1);
        Vector2 BombTransformPosition = this.transform.position;
        float CurrentAngle = Mathf.Atan2(Physics2D.Velocity.y, Physics2D.Velocity.x) * Mathf.Rad2Deg - 90;

        EHRayTraceHit RayHit;
        for (int i = 0; i < RayTraceCount; ++i)
        {
            Vector2 OriginPosition = BombTransformPosition + new Vector2(Mathf.Cos(CurrentAngle * Mathf.Deg2Rad) * BombRadius, Mathf.Sin(CurrentAngle * Mathf.Deg2Rad)) * BombRadius;
            if (CastRayFromVelocity(OriginPosition, out RayHit, 0, true))
            {
                EHGameplayCharacter CharacterThatWeHit = RayHit.HitCollider.GetComponent<EHGameplayCharacter>();
                if (CharacterThatWeHit == null || CharacterThatWeHit != CharacterThatLaunchedProjectile)
                {
                    Explode();
                    return;
                }
            }
            CurrentAngle += AngleIncrement;
        }
    }

    /// <summary>
    /// Call this method when we have collided with any other collider
    /// </summary>
    public void Explode()
    {
        Physics2D.enabled = false;
        ProjectileAnim.SetTrigger(EXPLODE_ANIM);
        this.enabled = false;
        EHSpawnPool.Instance.DespawnAfterTime(this, 1);
    }
}
