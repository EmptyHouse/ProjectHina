using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EHBaseProjectile : MonoBehaviour
{
    public Animator ProjectileAnim;
    protected Vector2 LaunchDirection;
    protected float LaunchSpeed;
    protected Vector2 CurrentMovementDirection;


    #region monobehaviour methods
    protected virtual void Awake()
    {
        ProjectileAnim = GetComponent<Animator>();
    }
    #endregion monobehaviour methods

    public abstract void LaunchProjectile(Vector2 DirectionToLaunch, float SpeedOfLaunch);

}
