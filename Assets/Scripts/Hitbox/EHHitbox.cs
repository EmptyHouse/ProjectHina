using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class EHHitbox : MonoBehaviour, ITickableComponent
{
    #region enums
    public enum EHitboxType
    {
        HITBOX,
        HURTBOX,
    }
    #endregion enums

    [SerializeField]
    [Tooltip("The hitbox type that will be assigned to this hitbox component. This should not be changed once the object is instantiated")]
    private EHitboxType HitboxType = EHitboxType.HITBOX;

    /// <summary>
    /// Set of all currently intersecting hitboxes
    /// </summary>
    private HashSet<EHHitbox> IntersectingHitboxSet = new HashSet<EHHitbox>();
    public EHHitboxActorComponent HitboxActorComponent { get; private set; }

    #region monobehaviour methods
    protected virtual void Awake()
    {
        HitboxActorComponent = GetComponentInParent<EHHitboxActorComponent>();

        if (HitboxActorComponent == null)
        {
            Debug.LogWarning("There is no HitboxActor component found in the parent of this hitbox. All interactions will be skipped.");
        }
    }

    protected virtual void Start()
    {
        BaseGameOverseer.Instance.HitboxManager.AddHitboxToManager(this);
    }

    protected virtual void OnDestroy()
    {
        if (BaseGameOverseer.Instance)
        {
            BaseGameOverseer.Instance.HitboxManager.RemoveHitboxFromManager(this);
        }
    }

    public void Tick(float DeltaTime)
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
        EHHitbox[] IntersectingHitboxArray = IntersectingHitboxSet.ToArray();
        for (int i = IntersectingHitboxArray.Length - 1; i >= 0; --i)
        {
            HitboxEndOverlap(IntersectingHitboxArray[i]);
        }
    }
    #endregion monobehaviour methods

    /// <summary>
    /// returns the shape of this hitbox
    /// </summary>
    /// <returns></returns>
    public virtual EHGeometry.ShapeType GetHitbxoShape() { return EHGeometry.ShapeType.None; }

    /// <summary>
    /// Returns the assigned hitbox type
    /// </summary>
    /// <returns></returns>
    public virtual EHitboxType GetHitboxType() { return HitboxType; }

    /// <summary>
    /// Check if the hitbox that is passed in is currently overlapping this hitbox
    /// </summary>
    /// <param name="OtherHitbox"></param>
    /// <returns></returns>
    public bool CheckForHitboxOverlap(EHHitbox OtherHitbox)
    {
        if (IsHitboxOverlapping(OtherHitbox))
        {
            if (!IntersectingHitboxSet.Contains(OtherHitbox))
            {
                HitboxBeginOverlap(OtherHitbox);
            }
            return true;
        }
        if (IntersectingHitboxSet.Contains(OtherHitbox))
        {
            HitboxEndOverlap(OtherHitbox);
        }
        return false;
    }

    protected abstract bool IsHitboxOverlapping(EHHitbox OtherHitbox);

    /// <summary>
    /// This method will be called upon this hitbox entering the hitbox that is passed in if they were not intersecting in the previous frame
    /// </summary>
    /// <param name="OtherHitbox"></param>
    private void HitboxBeginOverlap(EHHitbox OtherHitbox)
    {
        IntersectingHitboxSet.Add(OtherHitbox);
        OtherHitbox.IntersectingHitboxSet.Add(this);
        HitboxActorComponent.OnHitboxBeginIntersectOtherHitbox(this, OtherHitbox);
        OtherHitbox.HitboxActorComponent.OnHitboxBeginIntersectOtherHitbox(OtherHitbox, this);

    }

    /// <summary>
    /// Method will be called if the hitbox that is passed in is not longer intersecting, if it was intersecting with this hitbox in the previous
    /// frame
    /// </summary>
    /// <param name="OtherHitbox"></param>
    private void HitboxEndOverlap(EHHitbox OtherHitbox)
    {
        IntersectingHitboxSet.Remove(OtherHitbox);
        OtherHitbox.IntersectingHitboxSet.Remove(this);

    }

    /// <summary>
    /// This is where we should update the hitboxes bounds
    /// </summary>
    public abstract void UpdateHitbox();

    #region debug methods
    public readonly Color DEBUG_HITBOX_COLOR = Color.red;
    public readonly Color DEBUG_HURTBOX_COLOR = Color.cyan;
    public readonly Color DEBUG_INTERSECT_COLOR = new Color(.73f, .33f, .83f);
    
    /// <summary>
    /// Returns the color that our debug hitbox should display.
    /// </summary>
    /// <returns></returns>
    protected Color DebugGetColor()
    {
        if (IntersectingHitboxSet.Count > 0)
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
