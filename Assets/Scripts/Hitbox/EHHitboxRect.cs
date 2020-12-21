using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Derives our hitbox component. This handles logic specifcially for Rect shaped hitboxes
/// </summary>
public class EHHitboxRect : EHHitbox
{
    /// <summary>
    /// Offset position from the center of our transform component
    /// </summary>
    public Vector2 HitboxOffset = Vector2.zero;

    /// <summary>
    /// Size of our hitbox. Keep in mind that the actual size can also be impacted by the local scale of our transform
    /// </summary>
    public Vector2 HitboxSize = Vector2.one;

    /// <summary>
    /// The geometry component that is associated with this hitbox
    /// </summary>
    private EHRect2D RectGeometry;

    #region monobehaviour methods
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        EHGeometry.DebugDrawRect(RectGeometry, DebugGetColor(), true);
    }
    #endregion monobehaviour methods


    #region override methods
    public override void UpdateHitbox()
    {
        Vector2 AdjustedSize = HitboxSize * new Vector2(transform.localScale.x, transform.localScale.y);
        Vector2 AdjustedPosition = new Vector2(transform.position.x, transform.position.y) + HitboxOffset - AdjustedSize / 2;

        RectGeometry.RectPosition = AdjustedPosition;
        RectGeometry.RectSize = AdjustedSize;
    }

    public override EHGeometry.ShapeType GetHitbxoShape()
    {
        return EHGeometry.ShapeType.Rect2D;
    }

    protected override bool IsHitboxOverlapping(EHHitbox OtherHitbox)
    {
        switch (OtherHitbox.GetHitbxoShape())
        {
            case EHGeometry.ShapeType.Rect2D:
                return RectGeometry.IsOverlappingRect(((EHHitboxRect)OtherHitbox).RectGeometry);
        }

        return false;
    }
    #endregion override methods
}
