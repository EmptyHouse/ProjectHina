using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHGeometry : MonoBehaviour
{
    public enum ShapeType
    {
        NONE,
        BOX,
    }

    /// <summary>
    /// Abstract base class that can be used for all 2D geometry in our game
    /// </summary>
    public abstract class BaseGeometry
    {
        public Vector2 MaxBounds { get { return GetMaxBounds(); } }
        public Vector2 MinBounds { get { return GetMinBounds(); } }

        public abstract bool IsOverlapping(BaseGeometry Geometry);
        public abstract ShapeType GetShapeType();
        public abstract void DebugDrawGeometry(Color DebugColor, bool FillGeometry = false);
        protected abstract Vector2 GetMaxBounds();
        protected abstract Vector2 GetMinBounds();
        public abstract void CopyGeometry(BaseGeometry GeometryToCopy);
        public abstract Vector2 GetClosestPointOnGeometry(BaseGeometry OtherGeometry);
        public abstract float ShortestDistance(BaseGeometry OtherGeometry);
    }

    /// <summary>
    /// Custom Rect2D geometry that can be used for colliders and hitboxes in a 2d game
    /// </summary>
    public class Rect2D : BaseGeometry
    {
        #region main variables
        /// <summary>
        /// The min point on our rect geometry
        /// </summary>
        public Vector2 RectPosition;
        public Vector2 RectSize;
        #endregion main variables

        #region override methods
        public override bool IsOverlapping(BaseGeometry Geometry)
        {
            switch (Geometry.GetShapeType())
            {
                case ShapeType.NONE:
                    return false;
                case ShapeType.BOX:
                    if (this.MinBounds.x >= Geometry.MaxBounds.x || Geometry.MinBounds.x >= this.MaxBounds.x)
                    {
                        return false;
                    }
                    if (this.MinBounds.y >= Geometry.MaxBounds.y || Geometry.MinBounds.y >= this.MaxBounds.y)
                    {
                        return false;
                    }
                    return true;
            }

            return false;
        }

        protected override Vector2 GetMaxBounds()
        {
            return RectPosition + RectSize;
        }

        protected override Vector2 GetMinBounds()
        {
            return RectPosition;
        }

        public override ShapeType GetShapeType()
        {
            return ShapeType.BOX;
        }

        public override void DebugDrawGeometry(Color DebugColor, bool bFillGeometry = false)
        {
            #if UNITY_EDITOR
            Vector2[] Vertices = GetVertices(true);
            if (bFillGeometry)
            {
                Rect r = new Rect(MinBounds, MaxBounds - MinBounds);
                UnityEditor.Handles.DrawSolidRectangleWithOutline(r, new Color(DebugColor.r, DebugColor.g, DebugColor.b, 0.15f), DebugColor);
            }
            else
            {
                UnityEditor.Handles.color = DebugColor;
                for (int i = 0; i < Vertices.Length - 1; ++i)
                {
                    UnityEditor.Handles.DrawLine(Vertices[i], Vertices[i + 1]);
                }
            }
            #endif
        }

        public override void CopyGeometry(BaseGeometry GeometryToCopy)
        {
            if (GetShapeType() == ShapeType.BOX)
            {
                Rect2D RectGeometry = (Rect2D)GeometryToCopy;
                this.RectPosition = RectGeometry.RectPosition;
                this.RectSize = RectGeometry.RectSize;
                return;
            }
            Debug.LogError("Invalid Shape to copy to our box 2d geometry");
        }

        public override Vector2 GetClosestPointOnGeometry(BaseGeometry OtherGeometry)
        {
            switch (OtherGeometry.GetShapeType())
            {
                case ShapeType.BOX:
                    if (OtherGeometry.MinBounds.y > MaxBounds.y)
                    {
                        return new Vector2(RectPosition.x, MaxBounds.y);
                    }
                    else if (OtherGeometry.MaxBounds.y < MinBounds.y)
                    {
                        return new Vector2(RectPosition.x, MinBounds.y);
                    }
                    if (OtherGeometry.MinBounds.x > MaxBounds.x)
                    {
                        return new Vector2(MaxBounds.x, RectPosition.y);
                    }
                    else if (OtherGeometry.MaxBounds.x < MinBounds.y)
                    {
                        return new Vector2(MinBounds.x, RectPosition.y);
                    }

                    return Vector2.zero;
            }
            return Vector2.zero;
        }

        public override float ShortestDistance(BaseGeometry OtherGeometry)
        {
            bool Left = OtherGeometry.MaxBounds.x < MinBounds.x;
            bool Right = MaxBounds.x < OtherGeometry.MinBounds.x;
            bool Bottom = OtherGeometry.MaxBounds.y < MinBounds.y;
            bool Top = MaxBounds.y < OtherGeometry.MinBounds.y;

            if (Top && Left)
            {
                return Vector2.Distance(new Vector2(MinBounds.x, MaxBounds.y), new Vector2(OtherGeometry.MaxBounds.x, OtherGeometry.MinBounds.y));
            }
            else if (Left && Bottom)
            {
                return Vector2.Distance(MinBounds, OtherGeometry.MaxBounds);
            }
            else if (Bottom && Right)
            {
                return Vector2.Distance(new Vector2(MaxBounds.x, MinBounds.y), new Vector2(OtherGeometry.MinBounds.x, OtherGeometry.MaxBounds.y));
            }
            else if (Right && Top)
            {
                return Vector2.Distance(MaxBounds, OtherGeometry.MinBounds);
            }
            else if (Left)
            {
                return OtherGeometry.MaxBounds.x - MinBounds.x;
            }
            else if (Right)
            {
                return MaxBounds.x - OtherGeometry.MinBounds.x;
            }
            else if (Bottom)
            {
                return MinBounds.y - OtherGeometry.MaxBounds.y;
            }
            else if (Top)
            {
                return OtherGeometry.MinBounds.y - MaxBounds.y;
            }
            return 0;
        }
        #endregion override methods

        #region helper methods
        /// <summary>
        /// Helper method to get a list of vertices assocated with our rect geometry
        /// </summary>
        /// <param name="bIncludeFinalVertice"></param>
        /// <returns></returns>
        public Vector2[] GetVertices(bool bIncludeFinalVertice = false)
        {
            Vector2[] Vertices;
            if (bIncludeFinalVertice)
            {
                Vertices = new Vector2[5];
                Vertices[4] = MinBounds;
            }
            else
            {
                Vertices = new Vector2[4];
            }
            Vertices[0] = MinBounds;
            Vertices[1] = new Vector2(MinBounds.x, MaxBounds.y);
            Vertices[2] = MaxBounds;
            Vertices[3] = new Vector2(MaxBounds.x, MinBounds.y);
            return Vertices;
        }
        #endregion helper methods
    }
}

// NOTE: Plan on moving geometry to be structs rather than having them as classes

public struct EHBounds2D
{
    public Vector2 MinBounds;
    public Vector2 MaxBounds;
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

    public EHBounds2D GetBounds()
    {
        EHBounds2D Bounds = default;
        Bounds.MinBounds = MinBounds;
        Bounds.MaxBounds = MaxBounds;
        return Bounds;
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
        return Vector2.Distance(Other.Center, Center) < (Other.Radius + Radius);
    }

    public EHBounds2D GetBounds()
    {
        EHBounds2D Bounds = default;
        Bounds.MinBounds = MinBounds;
        Bounds.MaxBounds = MaxBounds;
        return Bounds;
    }
}

