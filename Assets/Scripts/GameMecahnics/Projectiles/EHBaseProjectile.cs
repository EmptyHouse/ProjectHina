using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EHPhysics2D))]
public abstract class EHBaseProjectile : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The number of rays we will cast to determine whether or not we collide with something")]
    protected byte RayTraceCount;

    protected Animator ProjectileAnim;
    protected EHPhysics2D Physics;
    protected Vector2 LaunchDirection;
    protected float LaunchSpeed;
    protected Vector2 CurrentMovementDirection;


    #region monobehaviour methods
    protected virtual void Awake()
    {
        ProjectileAnim = GetComponent<Animator>();
        Physics = GetComponent<EHPhysics2D>();
    }

    protected virtual void Update()
    {
        CheckForIntersectionWithOtherColliders();
    }

    protected virtual void OnValidate()
    {
        if (RayTraceCount < 2)
        {
            RayTraceCount = 2;
        }
    }

    protected abstract void CheckForIntersectionWithOtherColliders();

    protected bool CastRayFromVelocity(Vector2 OriginPosition, out EHRayTraceHit RayHit, int LayerMask = 0, bool bDebugDrawLines = false)
    {
        Vector2 ProjectileVelocity = Physics.Velocity;
        EHRayTraceParams Params = new EHRayTraceParams();
        Params.RayOrigin = OriginPosition;
        Params.RayDirection = ProjectileVelocity;
        Params.RayLength = ProjectileVelocity.magnitude;
        return EHPhysicsManager2D.RayTrace2D(ref Params, out RayHit, LayerMask, bDebugDrawLines);

    }
    #endregion monobehaviour methods

    public abstract void LaunchProjectile(Vector2 VelocityOfLaucnh);
    public virtual void CleanUpProjectile()
    {
        Destroy(this.gameObject);
    }
}
