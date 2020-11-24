using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EHBaseCollider2D : MonoBehaviour
{
    #region const values
    private const string ANIM_MOVEMENT_STATE = "MovementState";
    #endregion const values

    #region enums
    public enum EColliderType
    {
        STATIC,
        MOVEABLE,
        PHYSICS,
        TRIGGER,
    }
    #endregion enums
    public UnityAction<FHitData> OnCollision2DStay;
    public UnityAction<FHitData> OnCollision2DEnter;
    [Tooltip("Toggles whether or not we treat this as a character collider. Meaning that that origin point is at the base of the collider instead of the center")]
    public bool bIsCharacterCollider;
    [Tooltip("The type of our collider. This will determine how we update our collider as well as certain interactions we will have with other colliders")]
    public EColliderType ColliderType = EColliderType.STATIC;

    private HashSet<EHBaseCollider2D> OverlappingColliders = new HashSet<EHBaseCollider2D>();
    private Animator CharacterAnimator;

    #region monobehaviour methods
    protected virtual void Awake()
    {
        UpdateColliderBounds(false);
        CharacterAnimator = GetComponent<Animator>();
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

    public bool IsColliderOverlapping(EHBaseCollider2D OtherCollider)
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

    protected abstract bool ValidateColliderOverlapping(EHBaseCollider2D OtherCollider);

    protected virtual void OnDestroy()
    {
        if (BaseGameOverseer.Instance)
        {
            BaseGameOverseer.Instance.PhysicsManager.RemoveCollisionComponent(this);
        }
    }
    #endregion monobehaviour methods

    public abstract void UpdateColliderBounds(bool bUpdatePreviousBounds);

    public abstract float GetShortestDistanceToGeometry(EHBaseCollider2D OtherCollider);

    public abstract float GetShortestDistanceFromPreviousPosition(EHBaseCollider2D OtherCollider);

    public abstract bool PushOutCollider(EHBaseCollider2D ColliderToPushOut);

    protected void AddColliderToHitSet(EHBaseCollider2D OtherCollider)
    {
        if (OtherCollider != null && OverlappingColliders.Add(OtherCollider))
        {
            OtherCollider.OverlappingColliders.Add(this);
        }
    }

    protected void RemoveColliderFromHitSet(EHBaseCollider2D OtherCollider)
    {
        if (OtherCollider != null && OverlappingColliders.Remove(OtherCollider))
        {
            OtherCollider.OverlappingColliders.Remove(this);
        }    
    }

    protected bool ContainOverlappingCollider(EHBaseCollider2D OtherCollider)
    {
        return OverlappingColliders.Contains(OtherCollider);
    }

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

    public struct FHitData
    {
        public Vector2 HitDirection;
        public float HitForce;
        public EHBaseCollider2D OwningCollider;
        public EHBaseCollider2D OtherCollider;
    }
}
