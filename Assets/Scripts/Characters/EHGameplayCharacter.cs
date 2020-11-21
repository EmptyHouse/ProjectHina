using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Gameplay character is a character component that will contain logic that involves gameplay aspects, such as 
/// </summary>
public class EHGameplayCharacter : EHCharacter
{
    public UnityAction<EHDamageableComponent, float> OnCharacterHealthUpdated;
    public UnityAction<EHDamageableComponent, EHDamageableComponent> OnCharacterDied;

    public float MaxHealth;

    /// <summary>
    /// Every gameplay character should have a damageable component attached to them. If not, this can be left null
    /// </summary>
    public EHDamageableComponent DamageableComponent;

    public float CurrentHealth { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        DamageableComponent = GetComponent<EHDamageableComponent>();
        CurrentHealth = MaxHealth;
    }
}
