using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHBox2DCollider : EHBaseCollider2D
{
    #region main variables
    public Vector2 ColliderOffset;
    public Vector2 ColliderSize = Vector2.one;

    private EHGeometry.Rect2D RectColliderGeometry = new EHGeometry.Rect2D();
    private EHGeometry.Rect2D PreviousRectColliderGeometry = new EHGeometry.Rect2D();
    private readonly Vector2 BUFFER = Vector2.one * 0.001f;
    #endregion main variables

    #region override methods
    public override void UpdateColliderBounds(bool bUpdatePreviousBounds)
    {
        if (bUpdatePreviousBounds)
        {
            PreviousRectColliderGeometry.RectPosition = RectColliderGeometry.RectPosition;
            PreviousRectColliderGeometry.RectSize = RectColliderGeometry.RectSize;
            if (ColliderType == EColliderType.PHYSICS)
            {
                PreviousRectColliderGeometry.RectPosition += BUFFER;
                PreviousRectColliderGeometry.RectSize -= (2 * BUFFER);
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
        RectColliderGeometry.RectSize = RectSize;
        RectColliderGeometry.RectPosition = RectPosition;
    }

    public override EHGeometry.BaseGeometry GetColliderGeometry() { return RectColliderGeometry; }
    public override EHGeometry.BaseGeometry GetPreviousColliderGeometry() { return PreviousRectColliderGeometry; }

    public override bool PushOutCollider(EHBaseCollider2D ColliderToPushOut)
    {
        if (RectColliderGeometry.IsOverlapping(ColliderToPushOut.GetColliderGeometry()))
        {
            FHitData HitData = new FHitData();
            HitData.OwningCollider = this;
            HitData.OtherCollider = ColliderToPushOut;

            if (PreviousRectColliderGeometry.MaxBounds.y < ColliderToPushOut.GetPreviousColliderGeometry().MinBounds.y)
            {
                float OffsetY = RectColliderGeometry.MaxBounds.y - ColliderToPushOut.GetColliderGeometry().MinBounds.y;
                ColliderToPushOut.transform.position += Vector3.up * OffsetY;
                HitData.HitDirection = Vector2.down;
            }
            else if (PreviousRectColliderGeometry.MinBounds.y > ColliderToPushOut.GetPreviousColliderGeometry().MaxBounds.y)
            {
                float OffsetY = RectColliderGeometry.MinBounds.y - ColliderToPushOut.GetColliderGeometry().MaxBounds.y;
                ColliderToPushOut.transform.position += Vector3.up * OffsetY;
                HitData.HitDirection = Vector2.up;
            }
            else if (PreviousRectColliderGeometry.MaxBounds.x < ColliderToPushOut.GetPreviousColliderGeometry().MinBounds.x)
            {
                float OffsetX = RectColliderGeometry.MaxBounds.x - ColliderToPushOut.GetColliderGeometry().MinBounds.x;
                ColliderToPushOut.transform.position += Vector3.right * OffsetX;
                HitData.HitDirection = Vector2.left;
            }
            else if (PreviousRectColliderGeometry.MinBounds.x > ColliderToPushOut.GetPreviousColliderGeometry().MaxBounds.x)
            {
                float OffsetX = RectColliderGeometry.MinBounds.x - ColliderToPushOut.GetColliderGeometry().MaxBounds.x;
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
    #endregion override methods
}
