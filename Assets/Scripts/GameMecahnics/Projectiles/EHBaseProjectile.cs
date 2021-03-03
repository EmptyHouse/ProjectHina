using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EHPhysics2D))]
public abstract class EHBaseProjectile : MonoBehaviour, ISpawnable
{
    [SerializeField]
    [Tooltip("The number of rays we will cast to determine whether or not we collide with something")]
    protected byte RayTraceCount;

    protected Animator ProjectileAnim;
    protected EHPhysics2D Physics2D;
    protected Vector2 LaunchDirection;
    protected float LaunchSpeed;
    protected Vector2 CurrentMovementDirection;
    protected EHHitboxActorComponent HitboxActorComponent;
    protected EHGameplayCharacter CharacterThatLaunchedProjectile;


    #region monobehaviour methods
    protected virtual void Awake()
    {
        ProjectileAnim = GetComponent<Animator>();
        Physics2D = GetComponent<EHPhysics2D>();
        HitboxActorComponent = GetComponent<EHHitboxActorComponent>();
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
        Vector2 ProjectileVelocity = Physics2D.Velocity;
        EHRayTraceParams Params = new EHRayTraceParams();
        Params.RayOrigin = OriginPosition;
        Params.RayDirection = ProjectileVelocity.normalized;
        Params.RayLength = ProjectileVelocity.magnitude * EHTime.DeltaTime;
        return EHPhysicsManager2D.RayTrace2D(ref Params, out RayHit, LayerMask, bDebugDrawLines);

    }
    #endregion monobehaviour methods

    public virtual void LaunchProjectile(Vector2 VelocityOfLaucnh)
    {
        Physics2D.Velocity = VelocityOfLaucnh;
    }
    public virtual void OnCleanUpProjectile()
    {
        Destroy(this.gameObject);
    }

    public void SetCharacterOwner(EHGameplayCharacter CharacterOwner)    
    {
        this.CharacterThatLaunchedProjectile = CharacterOwner;
        if (HitboxActorComponent)
        {
            HitboxActorComponent.SetCharacterOwner(CharacterOwner);
        }
    }

    public virtual void OnSpawn()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnDespawn()
    {
        throw new System.NotImplementedException();
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}
