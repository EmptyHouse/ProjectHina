using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract  class EHBaseAIController : EHController
{
    private Vector3 OriginalPosition;
    private BaseAIState CurrentState;

    #region monobehaviour methods
    protected virtual void Awake()
    {
        OriginalPosition = this.transform.position;
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.OnStateTick(EHTime.DeltaTime);
        }
    }
    #endregion monobehaivour methods

    /// <summary>
    /// Begins the process to start a new State
    /// </summary>
    /// <param name="NextAIState"></param>
    protected void StartNewState(BaseAIState NextAIState)
    {
        if (NextAIState == CurrentState)
        {
            return;
        }

        if (CurrentState != null)
        {
            CurrentState.OnStateEnded();
        }
        CurrentState = NextAIState;
        if (!CurrentState.bIsInitialized)
        {
            CurrentState.InitilalizeState(this);
        }
        CurrentState.OnStateBegin();
    }

    /// <summary>
    /// Returns the position of our Actor's spawn position
    /// </summary>
    /// <returns></returns>
    public Vector3 GetOriginalPosition()
    {
        return OriginalPosition;
    }
}
