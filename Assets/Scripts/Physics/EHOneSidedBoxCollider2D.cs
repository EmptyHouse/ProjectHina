using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class EHOneSidedBoxCollider2D : EHBox2DCollider
{
    
    [SerializeField]
    private ECollisionDirection CollisionDirection = ECollisionDirection.UP;

    #region monobehaviour methods
    protected override void Awake()
    {
        base.Awake();
        SetCurrentDirection(CollisionDirection);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
    #endregion monobehaviour methods

    public void SetCurrentDirection(ECollisionDirection CollisionDirection)
    {
        CollisionMask = (byte)CollisionDirection;
    }

    public override bool IsRayTraceOverlapping(ref EHRayTraceParams Ray, out EHRayTraceHit RayHit)
    {
        if (!base.IsRayTraceOverlapping(ref Ray, out RayHit))
        {
            return false;
        }
        Vector2 OriginPoint = Ray.RayOrigin;

        switch (CollisionDirection)
        {
            case ECollisionDirection.UP:
                if (OriginPoint.y < RectGeometry.GetBounds().MaxBounds.y)
                {
                    return false;
                }
                break;
            case ECollisionDirection.DOWN:
                if (OriginPoint.y > RectGeometry.GetBounds().MinBounds.y)
                {
                    return false;
                }
                break;
            case ECollisionDirection.RIGHT:
                if (OriginPoint.x < RectGeometry.GetBounds().MaxBounds.x)
                {
                    return false;
                }
                break;
            case ECollisionDirection.LEFT:
                if (OriginPoint.x > RectGeometry.GetBounds().MinBounds.y)
                {
                    return false;
                }
                break;
        }

        return true;
    }
}
