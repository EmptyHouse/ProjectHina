using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHPhysicsManager2D : ITickableComponent
{
    private static EHPhysicsManager2D CachedInstance;
    private HashSet<EHPhysics2D> PhysicsComponentSet = new HashSet<EHPhysics2D>();
    private Dictionary<EHBaseCollider2D.EColliderType, HashSet<EHBaseCollider2D>> ColliderComponentDictionary = new Dictionary<EHBaseCollider2D.EColliderType, HashSet<EHBaseCollider2D>>();

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
            PhysicsComponent.Tick(DeltaTime);
        }

        foreach (EHBaseCollider2D MoveableCollider in ColliderComponentDictionary[EHBaseCollider2D.EColliderType.MOVEABLE])
        {
            if (MoveableCollider.gameObject.activeInHierarchy) MoveableCollider.UpdateColliderBounds(true);
        }
        foreach (EHBaseCollider2D PhysicsCollider in ColliderComponentDictionary[EHBaseCollider2D.EColliderType.PHYSICS])
        {
            if (PhysicsCollider.gameObject.activeInHierarchy) PhysicsCollider.UpdateColliderBounds(true);
        }

        CheckPhysicsCollidersAgainstCategory();
        
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
                        if (PhysicsCollider.IsPhysicsColliderOverlapping(Static))
                        {
                            CollisionNodeHeap.Push(new CollisionNode(PhysicsCollider.GetShortestDistanceFromPreviousPosition(Static), Static));
                        }
                    }
                }

                foreach (EHBaseCollider2D Moveable in ColliderComponentDictionary[EHBaseCollider2D.EColliderType.MOVEABLE])
                {
                    if (Moveable.gameObject.activeInHierarchy)
                    {
                        if (PhysicsCollider.IsPhysicsColliderOverlapping(Moveable))
                        {
                            CollisionNodeHeap.Push(new CollisionNode(PhysicsCollider.GetShortestDistanceFromPreviousPosition(Moveable), Moveable));
                        }
                    }
                }

                while (!CollisionNodeHeap.IsEmpty())
                {
                    if (CollisionNodeHeap.Pop().Collider.PushOutCollider(PhysicsCollider))
                    {
                        PhysicsCollider.UpdateColliderBounds(false);
                    }
                }
            }
        }
        CollisionNodeHeap.Clear();
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
