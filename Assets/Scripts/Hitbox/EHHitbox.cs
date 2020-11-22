using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class EHHitbox : MonoBehaviour
{
    public enum EHitboxType
    {
        HITBOX,
        HURTBOX,
    }

    #region events
    public UnityAction<EHHitbox, EHHitbox> OnHitboxOverlap;
    public UnityAction<EHHitbox, EHHitbox> OnHitboxEndOverlap;

    #endregion events

    [SerializeField]
    [Tooltip("The hitbox type that will be assigned to this hitbox component")]
    private EHitboxType HitboxType = EHitboxType.HITBOX;
    private HashSet<EHHitbox> IntersectingHitboxList = new HashSet<EHHitbox>();
    public EHDamageableComponent DamageableComponent { get; protected set; }

    #region monobehaviour methods
    protected virtual void Awake()
    {
        DamageableComponent = GetComponentInParent<EHDamageableComponent>();
    }

    protected virtual void Start()
    {
        BaseGameOverseer.Instance.HitboxManager.AddHitboxToManager(this);
    }

    protected virtual void OnDestroy()
    {
        if (DamageableComponent)
        {
            DamageableComponent = null;
        }

        if (BaseGameOverseer.Instance)
        {
            BaseGameOverseer.Instance.HitboxManager.RemoveHitboxFromManager(this);
        }
    }

    protected virtual void Update()
    {
        UpdateHitbox();
    }


    protected virtual void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            UpdateHitbox();
        }
    }

    protected virtual void OnDisable()
    {
        EHHitbox[] IntersectingHitboxArray = IntersectingHitboxList.ToArray();
        for (int i = IntersectingHitboxArray.Length - 1; i >= 0; --i)
        {
            HitboxEndOverlap(IntersectingHitboxArray[i]);
        }
    }

    public virtual EHGeometry.ShapeType GetHitbxoShape() { return EHGeometry.ShapeType.None; }


    public bool CheckForHitboxOverlap(EHHitbox OtherHitbox)
    {
        if (IsHitboxOverlapping(OtherHitbox))
        {
            if (!IntersectingHitboxList.Contains(OtherHitbox))
            {
                HitboxOverlap(OtherHitbox);
            }
            return true;
        }
        if (IntersectingHitboxList.Contains(OtherHitbox))
        {
            HitboxEndOverlap(OtherHitbox);
        }
        return false;
    }

    protected abstract bool IsHitboxOverlapping(EHHitbox OtherHitbox);

    private void HitboxOverlap(EHHitbox OtherHitbox)
    {
        IntersectingHitboxList.Add(OtherHitbox);
        OtherHitbox.IntersectingHitboxList.Add(this);

        OnHitboxOverlap?.Invoke(this, OtherHitbox);
        OtherHitbox.OnHitboxEndOverlap?.Invoke(OtherHitbox, this);
    }

    private void HitboxEndOverlap(EHHitbox OtherHitbox)
    {
        IntersectingHitboxList.Remove(OtherHitbox);
        OtherHitbox.IntersectingHitboxList.Remove(this);

        OnHitboxEndOverlap?.Invoke(this, OtherHitbox);
        OtherHitbox.OnHitboxEndOverlap?.Invoke(OtherHitbox, this);
    }

    #endregion monobehaviour methods
    /// <summary>
    /// This is where we should update the hitboxes bounds
    /// </summary>
    public abstract void UpdateHitbox();

    #region debug methods
    public readonly Color DEBUG_HITBOX_COLOR = Color.red;
    public readonly Color DEBUG_HURTBOX_COLOR = Color.cyan;
    public readonly Color DEBUG_INTERSECT_COLOR = new Color(.73f, .33f, .83f);
    
    
    protected Color DebugGetColor()
    {
        if (IntersectingHitboxList.Count > 0)
        {
            return DEBUG_INTERSECT_COLOR;
        }
        switch (HitboxType)
        {
            case EHitboxType.HITBOX:
                return DEBUG_HITBOX_COLOR;
            case EHitboxType.HURTBOX:
                return DEBUG_HURTBOX_COLOR;
            default:
                return Color.white;
        }
    }
    #endregion debug methods
}
