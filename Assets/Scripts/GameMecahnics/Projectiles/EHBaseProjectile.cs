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

    protected virtual void OnValidate()
    {
        if (RayTraceCount < 2)
        {
            RayTraceCount = 2;
        }
    }
    #endregion monobehaviour methods

    public abstract void LaunchProjectile(Vector2 VelocityOfLaucnh);
    public virtual void CleanUpProjectile()
    {
        Destroy(this.gameObject);
    }
}
