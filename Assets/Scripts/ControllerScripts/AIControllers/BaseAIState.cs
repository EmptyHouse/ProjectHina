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
    [Tooltip("Mark this value true to visually draw our debug state in OnDrawGizmos")]
    public bool bDebugDrawState;

    public virtual void InitilalizeState(EHBaseAIController AIControllerOwner)
    {
        this.AIControllerOwner = AIControllerOwner;
        PlayerReference = BaseGameOverseer.Instance.PlayerController.GetPlayerCharacter();
        bIsInitialized = true;
    }

    public abstract void OnStateBegin();
    public abstract void OnStateEnded();
    public abstract void OnStateTick(float DeltaTime);

    #region debug methods
    public virtual void DebugDrawState() { }
    protected void DrawSquareArea()
    {

    }

    protected void DrawCircleArea()
    {

    }

    protected void DrawTargetPosition(Vector2 TargetPosition)
    {

    }
    #endregion debug methods
}
