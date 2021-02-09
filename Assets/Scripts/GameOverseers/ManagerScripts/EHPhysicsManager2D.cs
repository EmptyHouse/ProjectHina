using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHPhysicsManager2D : ITickableComponent
{
    /// <summary>
    /// 
    /// </summary>
    private static EHPhysicsManager2D CachedInstance;
    /// <summary>
    /// Set containing all physics components that our found in our game
    /// </summary>
    private HashSet<EHPhysics2D> PhysicsComponentSet = new HashSet<EHPhysics2D>();
    /// <summary>
    /// Dictionary containing all physical colliders in our game
    /// </summary>
    private Dictionary<EHBaseCollider2D.EColliderType, HashSet<EHBaseCollider2D>> ColliderComponentDictionary = new Dictionary<EHBaseCollider2D.EColliderType, HashSet<EHBaseCollider2D>>();

    /// <summary>
    /// Hash set containing all trigger boxes in our scene
    /// </summary>
    private HashSet<EHBaseCollider2D> TriggerColliderSet = new HashSet<EHBaseCollider2D>();

    public EHPhysicsManager2D()
    {
        ColliderComponentDictionary.Add(EHBaseCollider2D.EColliderType.STATIC, new HashSet<EHBaseCollider2D>());
        ColliderComponentDictionary.Add(EHBaseCollider2D.EColliderType.MOVEABLE, new HashSet<EHBaseCollider2D>());
        ColliderComponentDictionary.Add(EHBaseCollider2D.EColliderType.PHYSICS, new HashSet<EHBaseCollider2D>());
        CachedInstance = this;
    }

    public void AddPhysicsComponent(EHPhysics2D PhysicsComponent)
    {
        if (!PhysicsComponentSet.Add(PhysicsComponent))
        {
            Debug.LogWarning("This Physics was already added to our Physics manager list. Please be sure to only add this component once");
            return;
        }
    }

    public void RemovePhysicsComeponent(EHPhysics2D PhysicsComponent)
    {
        if (!PhysicsComponentSet.Remove(PhysicsComponent))
        {
            Debug.LogWarning("This phsyics component was already added to our Physics Manager. Please be sure to only remove this component once.");
            return;
        }
    }

    public void AddCollisionComponent(EHBaseCollider2D ColliderComponent)
    {
        if (ColliderComponent == null)
        {
            Debug.LogWarning("A null collider was passed in to our physics manager. It has not been added...");
            return;
        }

        if (ColliderComponent.GetIsTriggerCollider())
        {
            if (!TriggerColliderSet.Add(ColliderComponent))
            {
                Debug.LogWarning("The trigger component that was passed in has already been added to our physics manager...");
            }

            return;
        }

        if (!ColliderComponentDictionary.ContainsKey(ColliderComponent.ColliderType))
        {
            Debug.LogWarning("Collider type was not added to dictionary. Please remember to add it in the Physics Manager Constructor");
            return;
        }

        if (!ColliderComponentDictionary[ColliderComponent.ColliderType].Add(ColliderComponent))
        {
            Debug.LogWarning("The Collider component has already been added to our physics manager");
            return;
        }
    }

    public void RemoveCollisionComponent(EHBaseCollider2D ColliderComponent)
    {
        if (ColliderComponent == null)
        {
            Debug.LogWarning("A null collider was passed into our physics manager");
            return;
        }

        if (ColliderComponent.GetIsTriggerCollider())
        {
            if (!TriggerColliderSet.Remove(ColliderComponent))
            {
                Debug.LogWarning("The trigger you are attempting to remove was not found in our physics manager. Perhaps you have already removed it?");
            }

            return;
        }

        if (!ColliderComponentDictionary.ContainsKey(ColliderComponent.ColliderType))
        {
            Debug.LogWarning("Collider type was not added to dictionary. Please remember to add it in the Physics Manager Constructor");
            return;
        }

        if (!ColliderComponentDictionary[ColliderComponent.ColliderType].Remove(ColliderComponent))
        {
            Debug.LogWarning("This collider component was not found in our manager");
            return;
        }
    }

    public void ClearAllPhysicsSets()
    {
        PhysicsComponentSet.Clear();
    }

    public void Tick(float DeltaTime)
    {
        foreach (EHPhysics2D PhysicsComponent in PhysicsComponentSet)
        {
            if (PhysicsComponent.gameObject.activeInHierarchy && PhysicsComponent.enabled) PhysicsComponent.Tick(DeltaTime);
        }

        foreach (EHBaseCollider2D MoveableCollider in ColliderComponentDictionary[EHBaseCollider2D.EColliderType.MOVEABLE])
        {
            if (MoveableCollider.gameObject.activeInHierarchy)
            {
                MoveableCollider.UpdateColliderBounds(true);
                MoveableCollider.DragIntersectingColliders();
            }
        }
        foreach (EHBaseCollider2D PhysicsCollider in ColliderComponentDictionary[EHBaseCollider2D.EColliderType.PHYSICS])
        {
            if (PhysicsCollider.gameObject.activeInHierarchy) PhysicsCollider.UpdateColliderBounds(true);
        }
        foreach (EHBaseCollider2D TriggerCollider in TriggerColliderSet)
        {
            TriggerCollider.UpdateColliderBounds(false);
        }

        CheckPhysicsCollidersAgainstCategory();
        CheckTriggerIntersections();
    }

    /// <summary>
    /// 
    /// </summary>
    private EHPriorityQueue<CollisionNode> CollisionNodeHeap = new EHPriorityQueue<CollisionNode>(true);

    private void CheckPhysicsCollidersAgainstCategory()
    {
        foreach (EHBaseCollider2D PhysicsCollider in ColliderComponentDictionary[EHBaseCollider2D.EColliderType.PHYSICS])
        {
            if (PhysicsCollider.gameObject.activeInHierarchy)
            {
                CollisionNodeHeap.Clear();
                foreach (EHBaseCollider2D Static in ColliderComponentDictionary[EHBaseCollider2D.EColliderType.STATIC])
                {
                    if (Static.gameObject.activeInHierarchy)
                    {
                        if (PhysicsCollider.IsPhysicsColliderOverlapping(Static) && !Physics2D.GetIgnoreLayerCollision(PhysicsCollider.gameObject.layer, Static.gameObject.layer))
                        {
                            CollisionNodeHeap.Push(new CollisionNode(PhysicsCollider.GetShortestDistanceFromPreviousPosition(Static), Static));
                        }
                    }
                }

                foreach (EHBaseCollider2D Moveable in ColliderComponentDictionary[EHBaseCollider2D.EColliderType.MOVEABLE])
                {
                    if (Moveable.gameObject.activeInHierarchy && !Physics2D.GetIgnoreLayerCollision(PhysicsCollider.gameObject.layer, Moveable.gameObject.layer))
                    {
                        if (PhysicsCollider.IsPhysicsColliderOverlapping(Moveable))
                        {
                            CollisionNodeHeap.Push(new CollisionNode(PhysicsCollider.GetShortestDistanceFromPreviousPosition(Moveable), Moveable));
                        }
                    }
                }

                while (!CollisionNodeHeap.IsEmpty())
                {
                    EHBaseCollider2D IntersectedCollider = CollisionNodeHeap.Pop().Collider;
                    if (IntersectedCollider.PushOutCollider(PhysicsCollider))
                    {
                        PhysicsCollider.UpdateColliderBounds(false);
                    }
                }
            }
        }
        CollisionNodeHeap.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    private void CheckTriggerIntersections()
    {
        foreach (EHBaseCollider2D TriggerCollider in TriggerColliderSet)
        {
            if (TriggerCollider.gameObject.activeInHierarchy)
            {
                foreach (EHBaseCollider2D PhysicsCollider in ColliderComponentDictionary[EHBaseCollider2D.EColliderType.PHYSICS])
                {
                    if (PhysicsCollider.gameObject.activeInHierarchy)
                    {
                        if (!Physics2D.GetIgnoreLayerCollision(PhysicsCollider.gameObject.layer, TriggerCollider.gameObject.layer))
                        { 
                            TriggerCollider.IsTriggerOverlappingCollider(PhysicsCollider);
                        }
                    }
                }
            }
        }
    }

    private struct CollisionNode : System.IComparable
    {
        public float CollisionDistance;
        public EHBaseCollider2D Collider;

        public CollisionNode(float CollisionDistance, EHBaseCollider2D Collider)
        {
            this.CollisionDistance = CollisionDistance;
            this.Collider = Collider;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            CollisionNode OtherNode = (CollisionNode)obj;
            return (int)Mathf.Sign(CollisionDistance - OtherNode.CollisionDistance);
        }
    }

    public static bool RayTrace2D(ref EHRayTraceParams Params, out EHRayTraceHit RayHit, int LayerMask = 0, bool bDebugDraw = false)
    {
        if (CachedInstance == null)
        {
            Debug.LogWarning("Game Overseer has not been initialized");
            RayHit = default;
            return false;
        }
        RayHit = default;
        EHRayTraceHit TempRayTraceHit;
        float ClosestDistance = -1;
        bool bMadeCollision = false;

        foreach (KeyValuePair<EHBaseCollider2D.EColliderType, HashSet<EHBaseCollider2D>> ColliderSet in CachedInstance.ColliderComponentDictionary)
        {
            if (RayTrace2D(ref Params, ColliderSet.Key, out TempRayTraceHit, LayerMask, bDebugDraw))
            {
                float CollisionDistance = Vector2.Distance(Params.RayOrigin, TempRayTraceHit.HitPoint);
                if (!bMadeCollision || ClosestDistance > CollisionDistance)
                {
                    ClosestDistance = CollisionDistance;
                    RayHit = TempRayTraceHit;
                    bMadeCollision = true;
                }
            }
        }
        return bMadeCollision;
    }

    public static bool RayTrace2D(ref EHRayTraceParams Params, EHBaseCollider2D.EColliderType ColliderType, out EHRayTraceHit RayHit, int LayerMask = 0, bool DebugDraw = false)
    {
        if (!CachedInstance.ColliderComponentDictionary.ContainsKey(ColliderType))
        {
            RayHit = default;
            return false;
        }

        bool bCollisionMade = false;
        float ClosestDistance = -1;
        RayHit = default;
        EHRayTraceHit TempRayHit;
        foreach (EHBaseCollider2D Collider in CachedInstance.ColliderComponentDictionary[ColliderType])
        {
            if (Collider.IsRayTraceOverlapping(ref Params, out TempRayHit))
            {
                float CollisionDistance = Vector2.Distance(Params.RayOrigin, TempRayHit.HitPoint);
                if (!bCollisionMade || ClosestDistance > CollisionDistance)
                {
                    ClosestDistance = CollisionDistance;
                    RayHit = TempRayHit;
                    bCollisionMade = true;
                }
            }
        }

#if UNITY_EDITOR
        if (DebugDraw)
        {
            EHDebug.RayTraceDrawLine(Params, bCollisionMade ? Color.red : Color.yellow);
        }
#endif

        return bCollisionMade;
    }

    /// <summary>
    /// Runs a box cast for every collider that is in our scene.
    /// 
    /// NOTE: Keep in mind this will ignore colliders that are labled triggers
    /// </summary>
    /// <param name="BoxToCast"></param>
    /// <param name="HitCollider"></param>
    /// <param name="LayerMask"></param>
    /// <returns></returns>
    public static bool BoxCast2D(ref EHRect2D BoxToCast, out EHBaseCollider2D HitCollider, int LayerMask = 0)
    {
        if (CachedInstance == null)
        {
            Debug.LogWarning("Game Overseer not initialized");
            HitCollider = null;
            return false;
        }

        foreach (KeyValuePair<EHBaseCollider2D.EColliderType, HashSet<EHBaseCollider2D>> ColliderSet in CachedInstance.ColliderComponentDictionary)
        {
            if (BoxCast2D(ref BoxToCast, ColliderSet.Key, out HitCollider, LayerMask))
            {
                return true;
            }
        }
        HitCollider = null;
        return false;
    }

    public static bool BoxCast2D(ref EHRect2D BoxToCast, EHBaseCollider2D.EColliderType ColliderType, out EHBaseCollider2D HitCollider, int LayerMask = 0)
    {
        if (CachedInstance == null)
        {
            Debug.LogWarning("Game Overseer not initialized");
            HitCollider = null;
            return false;
        }
        if (!CachedInstance.ColliderComponentDictionary.ContainsKey(ColliderType))
        {
            HitCollider = null;
            return false;
        }

        foreach (EHBaseCollider2D Collider in CachedInstance.ColliderComponentDictionary[ColliderType])
        {
            if (Collider.gameObject.activeInHierarchy && (LayerMask & 1 << Collider.gameObject.layer) != 0)
            {
                if (Collider.IsOverlappingRect2D(BoxToCast))
                {
                    HitCollider = Collider;
                    return true;
                }
            }
        }
        HitCollider = null;
        return false;
    }

    /// <summary>
    /// Returns a list of all colliders that are intersected
    /// </summary>
    /// <param name="BoxToCast"></param>
    /// <param name="ColliderType"></param>
    /// <param name="HitColliderList"></param>
    /// <param name="LayerMask"></param>
    /// <returns></returns>
    public static bool BoxCastAll2D(ref EHRect2D BoxToCast, EHBaseCollider2D.EColliderType ColliderType, out EHBaseCollider2D[] HitColliderList, int LayerMask)
    {
        if (CachedInstance == null)
        {
            Debug.LogWarning("The game overseer has not been initialized");
            HitColliderList = null;
            return false;
        }
        List<EHBaseCollider2D> BaseColliderList = new List<EHBaseCollider2D>();
        foreach (EHBaseCollider2D Collider2D in CachedInstance.ColliderComponentDictionary[ColliderType])
        {
            if (Collider2D.gameObject.activeInHierarchy && (LayerMask & 1 << Collider2D.gameObject.layer) != 0)
            {
                if (Collider2D.IsOverlappingRect2D(BoxToCast))
                {
                    BaseColliderList.Add(Collider2D);
                }
            }
        }

        HitColliderList = BaseColliderList.ToArray();
        if (BaseColliderList.Count > 0)
        {
            return true;
        }

        return false;
    }
}

public struct EHRayTraceParams
{
    // Ray Origin Position
    public Vector2 RayOrigin;
    // Direction of ray. Whatever value is placed here will be normalized
    public Vector2 RayDirection { get { return rayDirection; } set { rayDirection = value.normalized; } }
    private Vector2 rayDirection;
    // The length of the ray
    public float RayLength;
}

public struct EHRayTraceHit
{
    public EHBaseCollider2D HitCollider;
    /// <summary>
    /// The point where we intersected with our collider. This will be the closest point to the origin position of
    /// the ray trace hit
    /// </summary>
    public Vector2 HitPoint;
    //TO-DO This is not in yet, but maybe something I want to ad in the future
    public Vector2 HitNormal;
}