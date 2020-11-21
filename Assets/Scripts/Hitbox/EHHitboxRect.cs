using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHHitboxRect : EHHitbox
{
    public Vector2 HitboxOffset = Vector2.zero;
    public Vector2 HitboxSize = Vector2.one;

    /// <summary>
    /// 
    /// </summary>
    private EHGeometry.Rect2D AssociatedRect2D = new EHGeometry.Rect2D();

    #region override methods
    public override void UpdateHitbox()
    {
        Vector2 AdjustedSize = HitboxSize * new Vector2(transform.localScale.x, transform.localScale.y);
        Vector2 AdjustedPosition = new Vector2(transform.position.x, transform.position.y) + HitboxOffset - AdjustedSize / 2;

        AssociatedRect2D.RectPosition = AdjustedPosition;
        AssociatedRect2D.RectSize = AdjustedSize;
    }


    protected override EHGeometry.BaseGeometry GetHitboxBaseGeometry()
    {
        return AssociatedRect2D;
    }
    #endregion override methods
}
