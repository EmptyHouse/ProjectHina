using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHOneSidedBoxCollider2D : EHBox2DCollider
{
    
    [SerializeField]
    private ECollisionDirection CollisionDirection = ECollisionDirection.UP;

    #region monobehaviour methods
    protected override void Awake()
    {
        base.Awake();
        SetCurrentDirection(CollisionDirection);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
    #endregion monobehaviour methods

    public void SetCurrentDirection(ECollisionDirection CollisionDirection)
    {
        CollisionMask = (byte)CollisionDirection;
    }
}
