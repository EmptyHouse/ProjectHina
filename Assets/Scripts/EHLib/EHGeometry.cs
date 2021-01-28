using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EHGeometry
{
    public enum ShapeType
    {
        None,
        Rect2D,
        Circle2D,
    }
    #region debug methods


    public static void DebugDrawRect(EHRect2D Rect) { DebugDrawRect(Rect, Color.red); }
    public static void DebugDrawRect(EHRect2D Rect, Color DebugColor, bool Fill = false)
    {
#if UNITY_EDITOR
        EHBounds2D Bounds = Rect.GetBounds();
        Rect UnityRect = new Rect(Bounds.MinBounds, Rect.RectSize);
        Color FillColor = DebugColor;

        if (Fill) { FillColor.a *= .25f; }
        else { FillColor.a = 0; }
        UnityEditor.Handles.DrawSolidRectangleWithOutline(UnityRect, FillColor, DebugColor);
#endif
    }

    public static void DebugDrawCircle2D(EHCircle2D Circle)
    {

    }


    #endregion debug methods
}

public struct EHBounds2D
{
    public Vector2 MinBounds;
    public Vector2 MaxBounds;

    public override string ToString()
    {
        return "Min: " + MinBounds + " Max: " + MaxBounds;
    }
}

public struct EHRect2D
{
    public Vector2 RectPosition;
    public Vector2 RectSize;

    private Vector2 MinBounds { get { return RectPosition; } }
    private Vector2 MaxBounds { get { return RectPosition + RectSize; } }

    public bool IsOverlappingRect(EHRect2D Other)
    {
        if (this.MinBounds.x >= Other.MaxBounds.x || Other.MinBounds.x >= this.MaxBounds.x)
        {
            return false;
        }
        if (this.MinBounds.y >= Other.MaxBounds.y || Other.MinBounds.y >= this.MaxBounds.y)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the line intersect with tht rect geometry and returns the intersection collision closest to point1
    /// </summary>
    /// <param name="OriginPoint"></param>
    /// <param name="EndPoint"></param>
    /// <param name="IntersectPoint"></param>
    /// <returns></returns>
    public bool DoesLineIntersect(Vector2 OriginPoint, Vector2 EndPoint, out Vector2 IntersectPoint)
    {
        Vector2 Min = MinBounds;
        Vector2 Max = MaxBounds;
        if ((OriginPoint.x <= Min.x  && EndPoint.x <= Min.x) || (OriginPoint.y <= Min.y && EndPoint.y <= Min.y) ||
            (OriginPoint.x >= Max.x && EndPoint.x >= Max.x) || (OriginPoint.y <= Min.y && EndPoint.y <= Min.y))
        {
            IntersectPoint = default;
            return false;
        }

        // Condition where our line begins inside the box bounds
        if (OriginPoint.x > Min.x && OriginPoint.x < Max.x && OriginPoint.y > Min.y && OriginPoint.y < Max.y)
        {
            IntersectPoint = OriginPoint;
            return true;
        }

        float m = (EndPoint.y - OriginPoint.y) / (EndPoint.x - OriginPoint.x);

        float y = m * (Min.x - OriginPoint.x) + OriginPoint.y;
        if (y > Min.y && y < Max.y)
        {
            IntersectPoint = new Vector2(Min.x, y);
            return true;
        }
        y = m * (Max.x - OriginPoint.x) + OriginPoint.y;
        if (y > Min.y && y < Max.y)
        {
            IntersectPoint = new Vector2(Max.x, y);
            return true;
        }

        float x = (Min.y - OriginPoint.y) / m + OriginPoint.x;
        if (x > Min.x && x < Max.x)
        {
            IntersectPoint = new Vector2(x, Min.y);
            return true;
        }
        x = (Max.y - OriginPoint.y) / m + OriginPoint.x;
        if (x > Min.x && x < Max.x)
        {
            IntersectPoint = new Vector2(x, Max.y);
            return true;
        }

        IntersectPoint = default;
        return false;
    }


    public float GetShortestDistance(EHRect2D OtherRect)
    {
        bool Left = OtherRect.MaxBounds.x < MinBounds.x;
        bool Right = MaxBounds.x < OtherRect.MinBounds.x;
        bool Bottom = OtherRect.MaxBounds.y < MinBounds.y;
        bool Top = MaxBounds.y < OtherRect.MinBounds.y;

        if (Top && Left)
        {
            return Vector2.Distance(new Vector2(MinBounds.x, MaxBounds.y), new Vector2(OtherRect.MaxBounds.x, OtherRect.MinBounds.y));
        }
        else if (Left && Bottom)
        {
            return Vector2.Distance(MinBounds, OtherRect.MaxBounds);
        }
        else if (Bottom && Right)
        {
            return Vector2.Distance(new Vector2(MaxBounds.x, MinBounds.y), new Vector2(OtherRect.MinBounds.x, OtherRect.MaxBounds.y));
        }
        else if (Right && Top)
        {
            return Vector2.Distance(MaxBounds, OtherRect.MinBounds);
        }
        else if (Left)
        {
            return OtherRect.MaxBounds.x - MinBounds.x;
        }
        else if (Right)
        {
            return MaxBounds.x - OtherRect.MinBounds.x;
        }
        else if (Bottom)
        {
            return MinBounds.y - OtherRect.MaxBounds.y;
        }
        else if (Top)
        {
            return OtherRect.MinBounds.y - MaxBounds.y;
        }
        return 0;
    }

    public EHBounds2D GetBounds()
    {
        EHBounds2D Bounds = default;
        Bounds.MinBounds = MinBounds;
        Bounds.MaxBounds = MaxBounds;
        return Bounds;
    }

    public override string ToString()
    {
        return "Pos: " + RectPosition + " Size: " + RectSize;
    }
}

public struct EHCircle2D
{
    public Vector2 Center;
    public float Radius;

    private Vector2 MinBounds { get { return Center - Vector2.one * Radius; } }
    private Vector2 MaxBounds { get { return Center + Vector2.one * Radius; } }

    public bool IsOverlappingCircle(EHCircle2D Other)
    {
        return Vector2.Distance(Other.Center, Center) < Mathf.Abs(Other.Radius + Radius);
    }

    public EHBounds2D GetBounds()
    {
        EHBounds2D Bounds = default;
        Bounds.MinBounds = MinBounds;
        Bounds.MaxBounds = MaxBounds;
        return Bounds;
    }
}

/// <summary>
/// 
/// </summary>
public struct EHLine
{
    public Vector2 Point1;
    public Vector2 Point2;
}

