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
    private Dictionary<EHHitboxActorComponent, HashSet<EHHitbox>> HitboxDictionary = new Dictionary<EHHitboxActorComponent, HashSet<EHHitbox>>();
    private List<EHHitboxActorComponent> AllHitboxActorComponentsList = new List<EHHitboxActorComponent>();

    public void Tick(float DeltaTime)
    {
        foreach (HashSet<EHHitbox> HitboxSet in HitboxDictionary.Values)
        {
            foreach (EHHitbox Hitbox in HitboxSet)
            {
                if (Hitbox.gameObject.activeInHierarchy)
                {
                    Hitbox.Tick(DeltaTime);//Update all active hitboxes
                }
            }
        }

        for (int i = 0; i < AllHitboxActorComponentsList.Count - 1; ++i)
        {
            for (int j = i + 1; j < AllHitboxActorComponentsList.Count; ++j)
            {
                CheckForCollisions(AllHitboxActorComponentsList[i], AllHitboxActorComponentsList[j]);
            }
        }
    }

    /// <summary>
    /// Safely adds a hitbox to our hitbox manager
    /// </summary>
    /// <param name="HitboxToAdd"></param>
    public void AddHitboxToManager(EHHitbox HitboxToAdd)
    {
        if (HitboxToAdd == null || HitboxToAdd.HitboxActorComponent == null)
        {
            Debug.LogWarning("Attempted to add an invalid hitbox to the hitbox manager. Please make sure that there is a HitboxActorComponent in the parent object of our hitbox");
            return;
        }

        if (!HitboxDictionary.ContainsKey(HitboxToAdd.HitboxActorComponent))
        {
            HitboxDictionary.Add(HitboxToAdd.HitboxActorComponent, new HashSet<EHHitbox>());
            AllHitboxActorComponentsList.Add(HitboxToAdd.HitboxActorComponent);
        }
        
        if (!HitboxDictionary[HitboxToAdd.HitboxActorComponent].Add(HitboxToAdd))
        {
            Debug.LogWarning("You are attempting to add a hitbox that has already been added to the hitbox manager.");
        }
    }

    /// <summary>
    /// Remove a hitbox from our manager. If the count of hitboxes has reached 0 we will remove the
    /// DamageableComponent from our list as well
    /// </summary>
    /// <param name="HitboxToRemove"></param>
    public void RemoveHitboxFromManager(EHHitbox HitboxToRemove)
    {
        if (HitboxToRemove == null || HitboxToRemove.HitboxActorComponent == null)
        {
            Debug.LogWarning("Attempted to remove an invalid hitbox from our manager. Please make sure that there is a HitboxActorComponent in the parent object of our Hitbox");
            return;
        }

        if (!HitboxDictionary.ContainsKey(HitboxToRemove.HitboxActorComponent))
        {
            Debug.LogWarning("There was no HitboxActorComponent associated with this hitbox. Perhaps you have already removed it?");
            return;
        }

        if (HitboxDictionary[HitboxToRemove.HitboxActorComponent].Remove(HitboxToRemove))
        {
            if (HitboxDictionary[HitboxToRemove.HitboxActorComponent].Count == 0)
            {
                HitboxDictionary.Remove(HitboxToRemove.HitboxActorComponent);
            }
        }
        else
        {
            Debug.LogWarning("The hitbox you are trying to remove was not found in the hitbox manager. Perhaps it was already removed?");
        }
    }

    /// <summary>
    /// Returns whether or not the hitbox passed in has been added to our Hitbox manager
    /// </summary>
    /// <param name="Hitbox"></param>
    /// <returns></returns>
    public bool ManagerContainsHitbox(EHHitbox Hitbox)
    {
        if (Hitbox == null || Hitbox.HitboxActorComponent == null)
        {
            return false;
        }
        
        if (!HitboxDictionary.ContainsKey(Hitbox.HitboxActorComponent))
        {
            return false;
        }

        return HitboxDictionary[Hitbox.HitboxActorComponent].Contains(Hitbox);
    }

    private void CheckForCollisions(EHHitboxActorComponent DComponent1, EHHitboxActorComponent DComponent2)
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
