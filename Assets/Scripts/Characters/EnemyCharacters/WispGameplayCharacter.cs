using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispGameplayCharacter : EHGameplayCharacter
{

    public override void OnCharacterDied(FDamageData DamageData)
    {
        base.OnCharacterDied(DamageData);
        CharacterCollider.enabled = true;
        CharacterCollider.UpdateColliderBounds(false);
        CharacterCollider.UpdateColliderBounds(true);
        Physics2D.GravityScale = 1f;
    }
}
