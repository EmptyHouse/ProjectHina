using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseAIState
{
    /// <summary>
    /// The owning AI Controller
    /// </summary>
    protected EHBaseAIController AIControllerOwner;
    protected EHPlayerCharacter PlayerReference;
    public bool bIsInitialized { get; private set; }

    public virtual void InitilalizeState(EHBaseAIController AIControllerOwner)
    {
        this.AIControllerOwner = AIControllerOwner;
        PlayerReference = BaseGameOverseer.Instance.PlayerController.GetPlayerCharacter();
        bIsInitialized = true;
    }

    public abstract void OnStateBegin();
    public abstract void OnStateEnded();
    public abstract void OnStateTick(float DeltaTime);
}
