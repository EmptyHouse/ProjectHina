using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Gameplay character is a character component that will contain logic that involves gameplay aspects, such as 
/// </summary>
[RequireComponent(typeof(EHDamageableComponent))]
public class EHGameplayCharacter : EHCharacter
{
    /// <summary>
    /// Every gameplay character should have a damageable component attached to them. If not, this can be left null
    /// </summary>
    public EHDamageableComponent DamageableComponent;

    protected override void Awake()
    {
        base.Awake();
        DamageableComponent = GetComponent<EHDamageableComponent>();
    }

    /// <summary>
    /// This method will be called when the health of our character in the Damageable Component has reached 0. Any cleanup shoud be done here
    /// </summary>
    public virtual void OnCharacterDied(EHAttackComponent AttackComponentThatKilledUs) { }
}
