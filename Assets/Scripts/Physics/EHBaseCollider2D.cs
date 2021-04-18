using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EHBaseCollider2D : MonoBehaviour
{
    #region enums
    public enum EColliderType : byte
    {
        STATIC,
        MOVEABLE,
        PHYSICS,
    }
    #endregion enums

    #region collider events
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

    /// <summary>
    /// Delegate that will be called when a trigger has been entered. Only one of the two interacting colliders needs to be a trigger for this to be called
    /// </summary>
    [HideInInspector]
    public UnityAction<FTriggerData> OnTrigger2DEnter;
    /// <summary>
    /// Delegate that is called when a trigger has exited. Only one of the intersecting triggers needs to be a trigger to be called
    /// </summary>
    [HideInInspector]
    public UnityAction<FTriggerData> OnTrigger2DExit;
    #endregion collider events

    [Tooltip("Toggles whether or not we treat this as a character collider. Meaning that that origin point is at the base of the collider instead of the center")]
    public bool bIsCharacterCollider;
    [Tooltip("The type of our collider. This will determine how we update our collider as well as certain interactions we will have with other colliders")]
    public EColliderType ColliderType = EColliderType.STATIC;
    private Dictionary<EHBaseCollider2D, FHitData> OverlappingColliders = new Dictionary<EHBaseCollider2D, FHitData>();
    [Tooltip("Mark this true if we want to treat this collider as a trigger box insteat of a solid collision")]
    [SerializeField]
    private bool bIsTrigger = false;

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

    protected virtual void OnDisable()
    {
        List<EHBaseCollider2D> ColliderIterator = new List<EHBaseCollider2D>(OverlappingColliders.Keys);
        foreach (EHBaseCollider2D OtherCollider in ColliderIterator)
        {
            if (bIsTrigger)
            {

            }
            else
            {
                FHitData HitData = new FHitData();
                HitData.OtherCollider = OtherCollider;
                HitData.OwningCollider = this;
                RemoveColliderFromHitSet(OtherCollider, HitData);
            }
        }
    }

    protected virtual void OnValidate()
    {
        if (bIsTrigger)
        {
            if (ColliderType == EColliderType.PHYSICS)
            {
                ColliderType = EColliderType.MOVEABLE;
                Debug.LogWarning("A trigger collider cannot be a physics type collider. To save on calculations, please set the collider to static if it is to be a stationary trigger");
            }
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
    protected abstract bool SweepColliderOverlap(EHBaseCollider2D OtherCollider);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="OtherCollider"></param>
    /// <returns></returns>
    protected abstract bool IsColliderOverlapping(EHBaseCollider2D OtherCollider);

    #endregion abstract methods
    /// <summary>
    /// Returns whether the physics collider that is passed in is overlapping with
    /// this collider
    /// </summary>
    /// <param name="OtherCollider"></param>
    /// <returns></returns>
    public bool IsPhysicsColliderOverlapping(EHBaseCollider2D OtherCollider)
    {
        if (SweepColliderOverlap(OtherCollider))
        {
            return true;
        }

        if (ContainOverlappingCollider(OtherCollider))
        {
            FHitData HitData = new FHitData();
            HitData.OtherCollider = OtherCollider;
            HitData.OwningCollider = this;
            RemoveColliderFromHitSet(OtherCollider, HitData);
        }
        return false;
    }

    /// <summary>
    /// This should be called anytime we begin a collision with another collider
    /// </summary>
    /// <param name="OtherCollider"></param>
    protected void AddColliderToHitSet(EHBaseCollider2D OtherCollider, FHitData HitData)
    {
        if (OtherCollider != null && !OverlappingColliders.ContainsKey(OtherCollider))
        {
            OverlappingColliders.Add(OtherCollider, new FHitData());
            OtherCollider.OverlappingColliders.Add(this, new FHitData());
        }
        OverlappingColliders[OtherCollider] = HitData;
        OnCollision2DBegin?.Invoke(HitData);

        ReverseHitData(ref HitData);

        OtherCollider.OverlappingColliders[this] = HitData;
        OtherCollider.OnCollision2DBegin?.Invoke(HitData);
    }

    protected void HitCollisionStay(EHBaseCollider2D OtherCollider, FHitData HitData)
    {
        if (OtherCollider != null)
        {
            OnCollision2DStay?.Invoke(HitData);
            ReverseHitData(ref HitData);
            OtherCollider.OnCollision2DStay?.Invoke(HitData);
        }
    }

    /// <summary>
    /// This should be called anytime we end a collision with another collider
    /// </summary>
    /// <param name="OtherCollider"></param>
    protected void RemoveColliderFromHitSet(EHBaseCollider2D OtherCollider, FHitData HitData)
    {
        if (OtherCollider != null && OverlappingColliders.Remove(OtherCollider))
        {
            OtherCollider.OverlappingColliders.Remove(this);
            OnCollision2DEnd?.Invoke(HitData);
            ReverseHitData(ref HitData);
            OtherCollider.OnCollision2DEnd?.Invoke(HitData);

        }    
    }

    private void ReverseHitData(ref FHitData HitData)
    {
        EHBaseCollider2D OriginalOwning = HitData.OwningCollider;
        HitData.OwningCollider = HitData.OtherCollider;
        HitData.OtherCollider = OriginalOwning;
        HitData.HitDirection *= -1f;
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
        foreach (EHBaseCollider2D Collider in OverlappingColliders.Keys)
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
        return OverlappingColliders.ContainsKey(OtherCollider);
    }

    protected bool MatchesOverlappingHitData(EHBaseCollider2D OtherCollider, ref FHitData HitData)
    {
        if (!OverlappingColliders.ContainsKey(OtherCollider))
        {
            return false;
        }
        return OverlappingColliders[OtherCollider].HitDirection != HitData.HitDirection;
    }

    /// <summary>
    /// Returns the shape type of the collider being used
    /// </summary>
    /// <returns></returns>
    public virtual EHGeometry.ShapeType GetColliderShape() { return EHGeometry.ShapeType.None; }

    #region debug
    public Color GetDebugColor()
    {
        if (bIsTrigger)
        {
            return new Color(.914f, .961f, .256f);
        }
        switch (ColliderType)
        {
            case EColliderType.STATIC:
                return Color.green;
            case EColliderType.MOVEABLE:
                return new Color(.258f, .96f, .761f);
            case EColliderType.PHYSICS:
                return new Color(.761f, .256f, .96f);
        }
        return Color.green;
    }
    #endregion debug
   

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
    /// Checks to see if the ray that is passed in is overlapping the collider
    /// </summary>
    /// <param name="Ray"></param>
    /// <returns></returns>
    public abstract bool IsRayTraceOverlapping(ref EHRayTraceParams Ray, out EHRayTraceHit RayHit);

    /// <summary>
    /// Returns the bounds of the collider. This is to reduce the required calculations needed for complex shapes
    /// </summary>
    /// <returns></returns>
    public abstract EHBounds2D GetBounds();

    #region trigger methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Collider2D"></param>
    public void IsTriggerOverlappingCollider(EHBaseCollider2D Collider2D)
    {
        if (IsColliderOverlapping(Collider2D))
        {
            if (!ContainOverlappingCollider(Collider2D))
            {
                OnTriggerOverlapBegin(Collider2D);
            }
        }
        else if (ContainOverlappingCollider(Collider2D))
        {
            OnTriggerOverlapEnd(Collider2D);
        }
    }

    /// <summary>
    /// Call this method when a trigger overlap has ended
    /// </summary>
    /// <param name="Collider2D"></param>
    private void OnTriggerOverlapBegin(EHBaseCollider2D Collider2D)
    {
        OverlappingColliders.Add(Collider2D, new FHitData());
        Collider2D.OverlappingColliders.Add(this, new FHitData());
        FTriggerData TriggerData = new FTriggerData();

        TriggerData.OwningCollider = this;
        TriggerData.OtherCollider = Collider2D;
        OnTrigger2DEnter?.Invoke(TriggerData);

        TriggerData.OwningCollider = Collider2D;
        TriggerData.OtherCollider = this;
        Collider2D.OnTrigger2DEnter?.Invoke(TriggerData);
    }

    /// <summary>
    /// Call this method a trigger overlap 
    /// </summary>
    /// <param name="Collider2D"></param>
    private void OnTriggerOverlapEnd(EHBaseCollider2D Collider2D)
    {
        OverlappingColliders.Remove(Collider2D);
        Collider2D.OverlappingColliders.Remove(this);
        FTriggerData TriggerData = new FTriggerData();

        TriggerData.OwningCollider = this;
        TriggerData.OtherCollider = Collider2D;
        OnTrigger2DExit?.Invoke(TriggerData);

        TriggerData.OwningCollider = Collider2D;
        TriggerData.OtherCollider = this;
        Collider2D.OnTrigger2DExit?.Invoke(TriggerData);
    }

    /// <summary>
    /// Safely sets whether or not our collider is a trigger collider
    /// </summary>
    /// <param name="bIsTrigger"></param>
    public void SetIsTriggerCollider(bool bIsTrigger)
    {
        if (this.bIsTrigger == bIsTrigger)
        {
            return;
        }

        BaseGameOverseer.Instance.PhysicsManager.RemoveCollisionComponent(this);
        this.bIsTrigger = bIsTrigger;
        BaseGameOverseer.Instance.PhysicsManager.AddCollisionComponent(this);
    }

    /// <summary>
    /// Returns whether or not this collider is labled as a trigger collider.
    /// </summary>
    /// <returns></returns>
    public bool GetIsTriggerCollider()
    {
        return bIsTrigger;
    }
    #endregion trigger methods
}

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
/// Data that contains information for our trigger overlap event
/// </summary>
public struct FTriggerData
{
    public EHBaseCollider2D OwningCollider;
    public EHBaseCollider2D OtherCollider;
}
