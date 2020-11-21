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
        MOVABLE,
        PHYSICS,
        TRIGGER,
    }
    #endregion enums
    public UnityAction<EHBaseCollider2D, FHitData> OnCollision2DStay;
    public UnityAction<EHBaseCollider2D> OnCollision2DEnter;

    private EHGeometry.BaseGeometry BaseColliderGeometry { get { return GetColliderGeometry(); } }
    private EHGeometry.BaseGeometry PreviousColliderGeometry { get { return GetPreviousColliderGeometry(); } }
    [Tooltip("Toggles whether or not we treat this as a character collider. Meaning that that origin point is at the base of the collider instead of the center")]
    public bool bIsCharacterCollider;
    [Tooltip("The type of our collider. This will determine how we update our collider as well as certain interactions we will have with other colliders")]
    public EColliderType ColliderType = EColliderType.STATIC;

    protected HashSet<EHBaseCollider2D> CollisionSet = new HashSet<EHBaseCollider2D>();

    #region monobehaviour methods
    protected virtual void Awake()
    {
        UpdateColliderBounds(true);
        PreviousColliderGeometry.CopyGeometry(BaseColliderGeometry);//This is to start the current and base collider at the same place
    }

    protected virtual void Start()
    {
        if (BaseGameOverseer.Instance)
        {
            BaseGameOverseer.Instance.PhysicsManager.AddCollisionComponent(this);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            UpdateColliderBounds(false);
        }
        else
        {
            if (ColliderType == EColliderType.PHYSICS)
            {
                GetPreviousColliderGeometry().DebugDrawGeometry(Color.red);
            }
        }
        BaseColliderGeometry.DebugDrawGeometry(Color.green);
    }

    public bool IsColliderOverlapping(EHBaseCollider2D OtherCollider)
    {
        return BaseColliderGeometry.IsOverlapping(OtherCollider.BaseColliderGeometry);
    }

    protected virtual void OnDestroy()
    {
        if (BaseGameOverseer.Instance)
        {
            BaseGameOverseer.Instance.PhysicsManager.RemoveCollisionComponent(this);
        }
    }
    #endregion monobehaviour methods

    public virtual void UpdateColliderBounds(bool bUpdatePreviousBounds)
    {
        if (bUpdatePreviousBounds)
        {
            PreviousColliderGeometry.CopyGeometry(BaseColliderGeometry);
        }
    }

    public float GetShortestDistance(EHBaseCollider2D OtherCollider)
    {
        return BaseColliderGeometry.ShortestDistance(OtherCollider.BaseColliderGeometry);
    }

    public abstract EHGeometry.BaseGeometry GetColliderGeometry();
    public abstract EHGeometry.BaseGeometry GetPreviousColliderGeometry();
    public abstract bool PushOutCollider(EHBaseCollider2D ColliderToPushOut);



    public struct FHitData
    {
        public Vector2 HitDirection;
        public float HitForce;
        public EHBaseCollider2D OtherCollider;
    }
}
