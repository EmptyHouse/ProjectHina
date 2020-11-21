using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// Hitbox manager. Contains all the logic check which hitboxes should interact with each other
/// </summary>
public class EHHitboxManager : ITickableComponent
{
    private Dictionary<EHDamageableComponent, HashSet<EHHitbox>> HitboxDictionary = new Dictionary<EHDamageableComponent, HashSet<EHHitbox>>();
    private HashSet<EHDamageableComponent> AllValidDamageComponentSet = new HashSet<EHDamageableComponent>();

    public void Tick(float DeltaTime)
    {
        EHDamageableComponent[] DamageComponentArray = AllValidDamageComponentSet.ToArray();

        for (int i = 0; i < DamageComponentArray.Length - 1; ++i)
        {
            for (int j = i + 1; j < DamageComponentArray.Length; ++j)
            {
                CheckForCollisions(DamageComponentArray[i], DamageComponentArray[j]);
            }
        }
    }

    public void AddHitboxToManager(EHHitbox HitboxToAdd)
    {
        if (!HitboxToAdd || !HitboxToAdd.DamageableComponent)
        {
            Debug.LogError("The hitbox we are trying to add to our manager is invalid");
            return;
        }

        if (!HitboxDictionary.ContainsKey(HitboxToAdd.DamageableComponent))
        {
            HitboxDictionary.Add(HitboxToAdd.DamageableComponent, new HashSet<EHHitbox>());
            AllValidDamageComponentSet.Add(HitboxToAdd.DamageableComponent);
        }
        
        HashSet<EHHitbox> HitboxSet = HitboxDictionary[HitboxToAdd.DamageableComponent];
        if (!HitboxSet.Add(HitboxToAdd))
        {
            Debug.LogWarning("There was a problem adding our hitbox to our list");
        }
    }

    /// <summary>
    /// Remove a hitbox from our manager. If the count of hitboxes has reached 0 we will remove the
    /// DamageableComponent from our list as well
    /// </summary>
    /// <param name="HitboxToRemove"></param>
    public void RemoveHitboxFromManager(EHHitbox HitboxToRemove)
    {
        if (HitboxToRemove == null || HitboxToRemove.DamageableComponent == null)
        {
            return;
        }

        if (!HitboxDictionary.ContainsKey(HitboxToRemove.DamageableComponent))
        {
            Debug.LogWarning("There is no associated Damageable Component found in our hitbox manager");
            return;
        }

        HashSet<EHHitbox> HitboxSet = HitboxDictionary[HitboxToRemove.DamageableComponent];
        if (!HitboxSet.Remove(HitboxToRemove))
        {
            Debug.LogWarning("There was a problem removing our hitbox from our list");
        }
        if (HitboxSet.Count <= 0)
        {
            HitboxDictionary.Remove(HitboxToRemove.DamageableComponent);
            AllValidDamageComponentSet.Remove(HitboxToRemove.DamageableComponent);
        }
    }

    private void CheckForCollisions(EHDamageableComponent DComponent1, EHDamageableComponent DComponent2)
    {
        foreach (EHHitbox Hitbox1 in HitboxDictionary[DComponent1])
        {
            foreach (EHHitbox Hitbox2 in HitboxDictionary[DComponent2])
            {
                if (Hitbox1.gameObject.activeInHierarchy && Hitbox2.gameObject.activeInHierarchy)
                {
                    Hitbox1.CheckForHitboxOverlap(Hitbox2);
                }
            }
        }
    }
}
