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
