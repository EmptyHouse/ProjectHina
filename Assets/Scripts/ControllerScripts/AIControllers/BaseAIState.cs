using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAIState : ScriptableObject
{
    /// <summary>
    /// The owning AI Controller
    /// </summary>
    protected EHBaseAIController AIControllerOwner;
    protected EHPlayerCharacter PlayerReference;

    public virtual void InitilalizeState(EHBaseAIController AIControllerOwner)
    {
        this.AIControllerOwner = AIControllerOwner;
        PlayerReference = BaseGameOverseer.Instance.PlayerController.GetPlayerCharacter();
    }

    public abstract void OnStateBegin();
    public abstract void OnStateEnded();
    public abstract void OnStateTick(float DeltaTime);
}
