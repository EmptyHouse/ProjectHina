using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EHBox2DCollider : EHBaseCollider2D
{
    public enum ECollisionDirection : byte
    {
        UP = 0x01,
        DOWN = 0x02,
        LEFT = 0x04,
        RIGHT = 0x08,
    }

    #region main variables
    public Vector2 ColliderOffset;
    public Vector2 ColliderSize = Vector2.one;

    protected EHRect2D RectGeometry = new EHRect2D();
    protected EHRect2D PreviousRectGeometry = new EHRect2D();
    protected EHRect2D PhysicsSweepGeometry = new EHRect2D();
    public Vector2 DefaultColliderSize { get; private set; }
    protected byte CollisionMask = 0x0f;
    private readonly Vector2 BUFFER = Vector2.one * 0.02f;
    #endregion main variables

    #region monobehaviour methods
    protected override void Awake()
    {
        base.Awake();
        PreviousRectGeometry = RectGeometry;
        DefaultColliderSize = ColliderSize;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (Application.isPlaying && ColliderType == EColliderType.PHYSICS)
        {
            EHGeometry.DebugDrawRect(PreviousRectGeometry, Color.red);
            EHGeometry.DebugDrawRect(PhysicsSweepGeometry, Color.yellow);
        }
        EHGeometry.DebugDrawRect(RectGeometry, GetDebugColor());
    }
    #endregion monobehaviour methods

    #region override methods
    public override void UpdateColliderBounds(bool bUpdatePreviousBounds)
    {
        if (bUpdatePreviousBounds)
        {
            PreviousRectGeometry = RectGeometry;
            if (ColliderType == EColliderType.PHYSICS)
            {
                PreviousRectGeometry.RectPosition += BUFFER;
                PreviousRectGeometry.RectSize -= (2 * BUFFER);
            }
        }
        
        Vector2 RectSize = ColliderSize * transform.localScale;
        Vector2 WorldPosition = transform.position;
        Vector2 RectPosition;
        if (bIsCharacterCollider)
        {
            RectPosition = ColliderOffset + WorldPosition - (Vector2.right * RectSize.x / 2);
        }
        else
        {
            RectPosition = ColliderOffset + WorldPosition - (RectSize / 2);
        }
        RectGeometry.RectSize = RectSize;
        RectGeometry.RectPosition = RectPosition;

        if (ColliderType == EColliderType.PHYSICS)//Create a sweep bounds so that we do not phase through collisions when going at top speeds
        {
            EHBounds2D RectBounds = RectGeometry.GetBounds();
            EHBounds2D PreviousBounds = PreviousRectGeometry.GetBounds();

            Vector2 MinBounds = new Vector2(Mathf.Min(RectBounds.MinBounds.x, PreviousBounds.MinBounds.x), Mathf.Min(RectBounds.MinBounds.y, PreviousBounds.MinBounds.y));
            Vector2 MaxBounds = new Vector2(Mathf.Max(RectBounds.MaxBounds.x, PreviousBounds.MaxBounds.x), Mathf.Max(RectBounds.MaxBounds.y, PreviousBounds.MaxBounds.y));

            PhysicsSweepGeometry.RectPosition = MinBounds;
            PhysicsSweepGeometry.RectSize = MaxBounds - MinBounds;
        }
    }

    public override bool PushOutCollider(EHBaseCollider2D ColliderToPushOut)
    {
        EHBox2DCollider OtherRectCollier = (EHBox2DCollider)ColliderToPushOut;
        if (OtherRectCollier == null) return false;

        if (RectGeometry.IsOverlappingRect(OtherRectCollier.PhysicsSweepGeometry))
        {

            FHitData HitData = new FHitData();
            HitData.OwningCollider = this;
            HitData.OtherCollider = ColliderToPushOut;

            EHBounds2D ThisCurrentBounds = RectGeometry.GetBounds();
            EHBounds2D OtherCurrentBounds = OtherRectCollier.RectGeometry.GetBounds();
            EHBounds2D ThisPreviousBounds = PreviousRectGeometry.GetBounds();
            EHBounds2D OtherPreviousBounds = OtherRectCollier.PreviousRectGeometry.GetBounds();

            Vector2 RightUpOffset = ThisCurrentBounds.MaxBounds - OtherCurrentBounds.MinBounds;
            Vector2 LeftBottomOffset = ThisCurrentBounds.MinBounds - OtherCurrentBounds.MaxBounds;



            if (ThisPreviousBounds.MaxBounds.y < OtherPreviousBounds.MinBounds.y && RightUpOffset.y > 0 && (CollisionMask & (byte)ECollisionDirection.UP) != 0)
            {
                ColliderToPushOut.transform.position += Vector3.up * RightUpOffset.y;
                HitData.HitDirection = Vector2.down;
            }
            else if (ThisPreviousBounds.MaxBounds.x < OtherPreviousBounds.MinBounds.x && RightUpOffset.x > 0 && (CollisionMask & (byte)ECollisionDirection.RIGHT) != 0)
            {
                ColliderToPushOut.transform.position += Vector3.right * RightUpOffset.x;
                HitData.HitDirection = Vector2.left;
            }
            else if (ThisPreviousBounds.MinBounds.x > OtherPreviousBounds.MaxBounds.x && LeftBottomOffset.x < 0 && (CollisionMask & (byte)ECollisionDirection.LEFT) != 0)
            {
                ColliderToPushOut.transform.position += Vector3.right * LeftBottomOffset.x;
                HitData.HitDirection = Vector2.right;
            }
            else if (ThisPreviousBounds.MinBounds.y > OtherPreviousBounds.MaxBounds.y && LeftBottomOffset.y < 0 && (CollisionMask & (byte)ECollisionDirection.DOWN) != 0)
            {
                ColliderToPushOut.transform.position += Vector3.up * LeftBottomOffset.y;
                HitData.HitDirection = Vector2.up;
            }

            if (!ContainOverlappingCollider(ColliderToPushOut) || MatchesOverlappingHitData(ColliderToPushOut, ref HitData))
            {
                AddColliderToHitSet(ColliderToPushOut, HitData);
            }

            HitCollisionStay(ColliderToPushOut, HitData);

            return true;
        }
        return false;
    }

    public override float GetShortestDistanceToGeometry(EHBaseCollider2D OtherCollider)
    {
        switch (OtherCollider.GetColliderShape())
        {
            case EHGeometry.ShapeType.Rect2D:
                return RectGeometry.GetShortestDistance(((EHBox2DCollider)OtherCollider).RectGeometry);
        }
        return -1;
    }

    public override float GetShortestDistanceFromPreviousPosition(EHBaseCollider2D OtherCollider)
    {
        switch (OtherCollider.GetColliderShape())
        {
            case EHGeometry.ShapeType.Rect2D:
                return PreviousRectGeometry.GetShortestDistance(((EHBox2DCollider)OtherCollider).RectGeometry);
        }
        return -1;
    }

    public override bool IsRayTraceOverlapping(ref EHRayTraceParams Ray, out EHRayTraceHit RayHit)
    {
        if (RectGeometry.DoesLineIntersect(Ray.RayOrigin, Ray.RayOrigin + Ray.RayDirection * Ray.RayLength, out Vector2 IntersectPoint))
        {
            RayHit = new EHRayTraceHit();
            RayHit.HitPoint = IntersectPoint;
            RayHit.HitCollider = this;
            return true;
        }
        RayHit = default;
        return false;
    }

    public override bool IsOverlappingRect2D(EHRect2D Rect)
    {
        return RectGeometry.IsOverlappingRect(Rect);
    }

    public override bool IsOverlappingRect2DSweep(EHRect2D Rect)
    {
        if (ColliderType == EColliderType.PHYSICS)
        {
            return PhysicsSweepGeometry.IsOverlappingRect(Rect);
        }
        return false;
    }

    protected override bool SweepColliderOverlap(EHBaseCollider2D OtherCollider)
    {
        switch (OtherCollider.GetColliderShape())
        {
            case EHGeometry.ShapeType.Rect2D:
                return IsOverlappingRect2DSweep(((EHBox2DCollider)OtherCollider).RectGeometry);
        }
        return false;
    }

    protected override bool IsColliderOverlapping(EHBaseCollider2D OtherCollider)
    {
        switch (OtherCollider.GetColliderShape())
        {
            case EHGeometry.ShapeType.Rect2D:
                return IsOverlappingRect2D(((EHBox2DCollider)OtherCollider).RectGeometry);
        }
        return false;
    }

    public override EHGeometry.ShapeType GetColliderShape() { return EHGeometry.ShapeType.Rect2D; }
    public override EHBounds2D GetBounds() { return RectGeometry.GetBounds(); }
    protected override Vector2 GetOffsetFromPreviousPosition()
    {
        return RectGeometry.RectPosition - PreviousRectGeometry.RectPosition;
    }
    #endregion override methods

#if UNITY_EDITOR

#endif
}

