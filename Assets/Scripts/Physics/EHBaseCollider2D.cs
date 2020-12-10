using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EHBaseCollider2D : MonoBehaviour
{
    #region enums
    public enum EColliderType
    {
        STATIC,
        MOVEABLE,
        PHYSICS,
        TRIGGER,
    }
    #endregion enums
    /// <summary>
    /// Delegate that is called every frame that we are in contace with another collider
    /// </summary>
    [HideInInspector]
    public UnityAction<FHitData> OnCollision2DStay;
    /// <summary>
    /// Delegate that is calle every time we begin a collision with a new collider. This will not be called again until the collision has ended
    /// </summary>
    [HideInInspector]
    public UnityAction<FHitData> OnCollision2DBegin;
    /// <summary>
    /// Delegate that will be called whenever a collision has ended. Meaning that a collider is no longer in direct contact with this one.
    /// </summary>
    [HideInInspector]
    public UnityAction<FHitData> OnCollision2DEnd;
    [Tooltip("Toggles whether or not we treat this as a character collider. Meaning that that origin point is at the base of the collider instead of the center")]
    public bool bIsCharacterCollider;
    [Tooltip("The type of our collider. This will determine how we update our collider as well as certain interactions we will have with other colliders")]
    public EColliderType ColliderType = EColliderType.STATIC;
    private HashSet<EHBaseCollider2D> OverlappingColliders = new HashSet<EHBaseCollider2D>();

    #region monobehaviour methods
    protected virtual void Awake()
    {
        UpdateColliderBounds(false);
    }

    protected virtual void Start()
    {
        if (BaseGameOverseer.Instance)
        {
            BaseGameOverseer.Instance.PhysicsManager.AddCollisionComponent(this);
        }
    }

    private void OnDisable()
    {
        List<EHBaseCollider2D> ColliderIterator = new List<EHBaseCollider2D>(OverlappingColliders);
        foreach (EHBaseCollider2D OtherCollider in ColliderIterator)
        {
            RemoveColliderFromHitSet(OtherCollider);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            UpdateColliderBounds(false);
        }
    }

    protected virtual void OnDestroy()
    {
        if (BaseGameOverseer.Instance)
        {
            BaseGameOverseer.Instance.PhysicsManager.RemoveCollisionComponent(this);
        }
    }
    #endregion monobehaviour methods

    #region abstract methods
    /// <summary>
    /// This method will be called every frame to update the bounds of the collider
    /// NOTE: This method will not be called for static colliders. Static colliders will only call this method once upon first being instantiated
    /// </summary>
    /// <param name="bUpdatePreviousBounds"></param>
    public abstract void UpdateColliderBounds(bool bUpdatePreviousBounds);

    /// <summary>
    /// Returns the shortest distance to our collider. Returns 0 if the colliders are touching or intersecting
    /// </summary>
    /// <param name="OtherCollider"></param>
    /// <returns></returns>
    public abstract float GetShortestDistanceToGeometry(EHBaseCollider2D OtherCollider);

    /// <summary>
    /// Returns the shortest distance to the collider's previous position. Should only really be usedfor calculating closes collider to our 
    /// physics based colliders
    /// </summary>
    /// <param name="OtherCollider"></param>
    /// <returns></returns>
    public abstract float GetShortestDistanceFromPreviousPosition(EHBaseCollider2D OtherCollider);

    /// <summary>
    /// This method will push our a collider and place it in the appropriate position. Returns true if the collider was pushed out
    /// </summary>
    /// <param name="ColliderToPushOut"></param>
    /// <returns></returns>
    public abstract bool PushOutCollider(EHBaseCollider2D ColliderToPushOut);

    /// <summary>
    /// Returns the offset from the previous position.
    /// </summary>
    /// <returns></returns>
    protected abstract Vector2 GetOffsetFromPreviousPosition();

    /// <summary>
    /// Returns true if the collider that is passed in is overlapping with this collider. This will not push out the collider. You will have to call
    /// PushOutCollider() for that
    /// </summary>
    /// <param name="OtherCollider"></param>
    /// <returns></returns>
    protected abstract bool ValidateColliderOverlapping(EHBaseCollider2D OtherCollider);

    #endregion abstract methods
    /// <summary>
    /// Returns whether the physics collider that is passed in is overlapping with
    /// this collider
    /// </summary>
    /// <param name="OtherCollider"></param>
    /// <returns></returns>
    public bool IsPhysicsColliderOverlapping(EHBaseCollider2D OtherCollider)
    {
        if (ValidateColliderOverlapping(OtherCollider))
        {
            return true;
        }

        if (ContainOverlappingCollider(OtherCollider))
        {
            RemoveColliderFromHitSet(OtherCollider);
        }
        return false;
    }

    /// <summary>
    /// This should be called anytime we begin a collision with another collider
    /// </summary>
    /// <param name="OtherCollider"></param>
    protected void AddColliderToHitSet(EHBaseCollider2D OtherCollider)
    {
        if (OtherCollider != null && OverlappingColliders.Add(OtherCollider))
        {
            OtherCollider.OverlappingColliders.Add(this);
        }
    }

    /// <summary>
    /// This should be called anytime we end a collision with another collider
    /// </summary>
    /// <param name="OtherCollider"></param>
    protected void RemoveColliderFromHitSet(EHBaseCollider2D OtherCollider)
    {
        if (OtherCollider != null && OverlappingColliders.Remove(OtherCollider))
        {
            OtherCollider.OverlappingColliders.Remove(this);
        }    
    }

    /// <summary>
    /// You can think of this as friction in a way. This will only be called by moveable colliders for the time being
    /// 
    /// This method will shift any collider's transform by the amount that this collider has shifted.
    /// Collider that is being dragged must be a physics type collider. 
    /// NOTE: Update with PhysicsManager only. This should only be used on movable colliders
    /// </summary>
    public void DragIntersectingColliders()
    {
        Vector3 OffsetPosition = GetOffsetFromPreviousPosition();
        foreach (EHBaseCollider2D Collider in OverlappingColliders)
        {
            if (Collider.ColliderType == EColliderType.PHYSICS)
            {
                Collider.transform.position += OffsetPosition;
            }
        }
    }

    /// <summary>
    /// Returns true if the collider that is passed in was already colliding with this collider in the previous frame
    /// </summary>
    /// <param name="OtherCollider"></param>
    /// <returns></returns>
    protected bool ContainOverlappingCollider(EHBaseCollider2D OtherCollider)
    {
        return OverlappingColliders.Contains(OtherCollider);
    }

    /// <summary>
    /// Returns the shape type of the collider being used
    /// </summary>
    /// <returns></returns>
    public virtual EHGeometry.ShapeType GetColliderShape() { return EHGeometry.ShapeType.None; }

    #region debug
    public Color GetDebugColor()
    {
        switch (ColliderType)
        {
            case EColliderType.STATIC:
                return Color.green;
            case EColliderType.MOVEABLE:
                return new Color(.258f, .96f, .761f);
            case EColliderType.PHYSICS:
                return new Color(.761f, .256f, .96f);
            case EColliderType.TRIGGER:
                return new Color(.914f, .961f, .256f);
        }
        return Color.green;
    }
    #endregion debug
    /// <summary>
    /// Struct that contains information about the collision that was experienced. This will only be sent
    /// when the OnCollisionBegin() delegate is called
    /// </summary>
    public struct FHitData
    {
        public Vector2 HitDirection;
        public float HitForce;
        public EHBaseCollider2D OwningCollider;
        public EHBaseCollider2D OtherCollider;
    }

    /// <summary>
    /// Returns whether the Rect2D that is passed in intersects with this collider
    /// </summary>
    /// <param name="Rect"></param>
    /// <returns></returns>
    public abstract bool IsOverlappingRect2D(EHRect2D Rect);

    /// <summary>
    /// Returns whether the Rect that is passed in intersects with the sweep of our collider.
    /// Sweep of our collider is the collision between the previous and current bounds
    /// </summary>
    /// <param name="Rect"></param>
    /// <returns></returns>
    public abstract bool IsOverlappingRect2DSweep(EHRect2D Rect);

    /// <summary>
    /// Returns the bounds of the collider. This is to reduce the required calculations needed for complex shapes
    /// </summary>
    /// <returns></returns>
    public abstract EHBounds2D GetBounds();
}
