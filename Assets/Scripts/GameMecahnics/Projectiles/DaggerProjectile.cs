using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerProjectile : EHBaseProjectile
{
    public float ProjectileWidth;
    public float ProjectileHorizontalOffset;

    #region monobehaviour methods
    protected override void Awake()
    {
        base.Awake();
        GetComponent<EHAttackComponent>().OnAttackCharacterDel += OnCharacterHit;
    }
    protected override void Update()
    {
        RotateBasedOnVelocity();
        base.Update();
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        if (ProjectileWidth < 0)
        {
            ProjectileWidth = 0;
        }
    }
    #endregion monobehaviour methods
    #region override methods
    public override void LaunchProjectile(Vector2 VelocityOfLaucnh)
    {
        Physics2D.Velocity = VelocityOfLaucnh;
        RotateBasedOnVelocity();

    }

    protected override void CheckForIntersectionWithOtherColliders()
    {
        Vector2 InitialPosition = transform.position + transform.right * ProjectileHorizontalOffset - transform.up * ProjectileWidth / 2f;
        float IncrementAmount = ProjectileWidth / (RayTraceCount - 1);
        Vector2 TransformUp = transform.up;
        EHRayTraceHit RayHit;
        for (int i = 0; i < RayTraceCount; ++i)
        {
            if (CastRayFromVelocity(InitialPosition += TransformUp * IncrementAmount, out RayHit))
            {
                return;
            }
        }
    }
    #endregion override methods

    private void RotateBasedOnVelocity()
    {
        float ZRoation = Mathf.Atan2(Physics2D.Velocity.y, Physics2D.Velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, ZRoation);
    }

    private void OnCharacterHit(FAttackData AttackData)
    {
        Destroy(this.gameObject);
    }
}
