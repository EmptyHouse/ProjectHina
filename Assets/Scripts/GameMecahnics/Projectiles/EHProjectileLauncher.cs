using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHProjectileLauncher : MonoBehaviour
{
    [Tooltip("The projectile that we will launch")]
    [SerializeField]
    private EHBaseProjectile ProjectileToLaunch;
    [Tooltip("This transform determines the position that we will launch our projectile from. The rotation will also determin the direction that we send the projectile")]
    [SerializeField]
    private Transform[] ProjectileLaunchPointTransform = new Transform[1];
    [Tooltip("The speed at which our projectile will be launched at")]
    [SerializeField]
    private float LaunchSpeed;
    private EHGameplayCharacter CharacterOwner;

    private void Awake()
    {
        CharacterOwner = GetComponent<EHGameplayCharacter>();
    }

    /// <summary>
    /// Call this method to launch our projectiles
    /// </summary>
    public virtual void OnLaucnhProjectile()
    {
        foreach (Transform LaunchTransform in ProjectileLaunchPointTransform)
        {
            if (LaunchTransform.gameObject.activeSelf)
            {
                EHBaseProjectile NewProjectile = Instantiate<EHBaseProjectile>(ProjectileToLaunch);
                NewProjectile.transform.position = LaunchTransform.position;
                NewProjectile.LaunchProjectile(LaunchTransform.up * LaunchSpeed);
                NewProjectile.SetCharacterOwner(CharacterOwner);
            }
        }
    }
}
