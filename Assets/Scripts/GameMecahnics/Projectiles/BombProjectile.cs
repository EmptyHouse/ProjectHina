using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProjectile : EHBaseProjectile
{
    #region main variables
    [SerializeField]
    protected float BombRadius;
    #endregion main varaibles

    #region monobehavoiur methods
    protected override void Awake()
    {
        base.Awake();
        BombRadius = GetComponent<SpriteRenderer>().bounds.size.x / 2f;
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
        float CurrentAngle = 0;
        Vector2 InitialMovementDirection = Physics.Velocity;
        Vector2 CastLength = Physics.Velocity / EHTime.DeltaTime;

        for (int i = 0; i < RayTraceCount; ++i)
        {
        }
    }

    public override void LaunchProjectile(Vector2 DirectionToLaunch, float SpeedOfLaunch)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Call this method when we have collided with any other collider
    /// </summary>
    public void Explode()
    {
        Destroy(this.gameObject);
    }
}
