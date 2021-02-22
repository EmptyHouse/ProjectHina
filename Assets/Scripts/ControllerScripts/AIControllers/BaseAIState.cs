using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAIState : ScriptableObject
{
    /// <summary>
    /// The owning AI Controller
    /// </summary>
    protected EHBaseAIController AIControllerOwner;

    public BaseAIState(EHBaseAIController AIControllerOwner)
    {
        this.AIControllerOwner = AIControllerOwner;
    }

    public abstract void OnStateBegin();
    public abstract void OnStateEnded();
    public abstract void OnStateTick(float DeltaTime);
}
