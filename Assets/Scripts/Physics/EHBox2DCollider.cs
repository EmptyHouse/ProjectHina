using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHBox2DCollider : EHBaseCollider2D
{
    #region main variables
    public Vector2 ColliderOffset;
    public Vector2 ColliderSize = Vector2.one;

    private EHRect2D RectGeometry = new EHRect2D();
    private EHRect2D PrevoiusRectGeometry = new EHRect2D();
    public Vector2 DefaultColliderSize { get; private set; }
    private readonly Vector2 BUFFER = Vector2.one * 0.02f;
    #endregion main variables

    #region monobehaviour methods
    protected override void Awake()
    {
        base.Awake();
        PrevoiusRectGeometry = RectGeometry;
        DefaultColliderSize = ColliderSize;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (Application.isPlaying && ColliderType == EColliderType.PHYSICS)
        {
            EHGeometry.DebugDrawRect(PrevoiusRectGeometry, Color.red);
        }
        EHGeometry.DebugDrawRect(RectGeometry, GetDebugColor());
    }
    #endregion monobehaviour methods

    #region override methods
    public override void UpdateColliderBounds(bool bUpdatePreviousBounds)
    {
        if (bUpdatePreviousBounds)
        {
            PrevoiusRectGeometry = RectGeometry;
            if (ColliderType == EColliderType.PHYSICS)
            {
                PrevoiusRectGeometry.RectPosition += BUFFER;
                PrevoiusRectGeometry.RectSize -= (2 * BUFFER);
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
    }

    public override bool PushOutCollider(EHBaseCollider2D ColliderToPushOut)
    {
        EHBox2DCollider OtherRectCollier = (EHBox2DCollider)ColliderToPushOut;
        if (OtherRectCollier == null) return false;

        if (RectGeometry.IsOverlappingRect(OtherRectCollier.RectGeometry))
        {
            FHitData HitData = new FHitData();
            HitData.OwningCollider = this;
            HitData.OtherCollider = ColliderToPushOut;

            EHBounds2D ThisCurrentBounds = RectGeometry.GetBounds();
            EHBounds2D OtherCurrentBounds = OtherRectCollier.RectGeometry.GetBounds();
            EHBounds2D ThisPreviousBounds = PrevoiusRectGeometry.GetBounds();
            EHBounds2D OtherPreviousBounds = OtherRectCollier.PrevoiusRectGeometry.GetBounds();

            if (ThisPreviousBounds.MaxBounds.y < OtherPreviousBounds.MinBounds.y)
            {
                float OffsetY = ThisCurrentBounds.MaxBounds.y - OtherCurrentBounds.MinBounds.y;
                ColliderToPushOut.transform.position += Vector3.up * OffsetY;
                HitData.HitDirection = Vector2.down;
            }
            else if (ThisPreviousBounds.MinBounds.y > OtherPreviousBounds.MaxBounds.y)
            {
                float OffsetY = ThisCurrentBounds.MinBounds.y - OtherCurrentBounds.MaxBounds.y;
                ColliderToPushOut.transform.position += Vector3.up * OffsetY;
                HitData.HitDirection = Vector2.up;
            }
            else if (ThisPreviousBounds.MaxBounds.x < OtherPreviousBounds.MinBounds.x)
            {
                float OffsetX = ThisCurrentBounds.MaxBounds.x - OtherCurrentBounds.MinBounds.x;
                ColliderToPushOut.transform.position += Vector3.right * OffsetX;
                HitData.HitDirection = Vector2.left;
            }
            else if (ThisPreviousBounds.MinBounds.x > OtherPreviousBounds.MaxBounds.x)
            {
                float OffsetX = ThisCurrentBounds.MinBounds.x - OtherCurrentBounds.MaxBounds.x;
                ColliderToPushOut.transform.position += Vector3.right * OffsetX;
                HitData.HitDirection = Vector2.right;
            }

            FHitData OtherHitData = HitData;
            OtherHitData.OwningCollider = HitData.OtherCollider;
            OtherHitData.OtherCollider = HitData.OwningCollider;

            OtherHitData.HitDirection *= -1;

            OnCollision2DStay?.Invoke(HitData);
            ColliderToPushOut.OnCollision2DStay?.Invoke(OtherHitData);

            if (!ContainOverlappingCollider(ColliderToPushOut))
            {
                AddColliderToHitSet(ColliderToPushOut);
                OnCollision2DEnter?.Invoke(HitData);
                ColliderToPushOut.OnCollision2DEnter?.Invoke(OtherHitData);
            }

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
                return PrevoiusRectGeometry.GetShortestDistance(((EHBox2DCollider)OtherCollider).RectGeometry);
        }
        return -1;
    }

    public override bool IsOverlappingRect2D(EHRect2D Rect)
    {
        return RectGeometry.IsOverlappingRect(Rect);
    }
    

    protected override bool ValidateColliderOverlapping(EHBaseCollider2D OtherCollider)
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
    #endregion override methods
}
