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

    protected virtual void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.OnStateTick(EHTime.DeltaTime);
        }
    }
    #endregion monobehaivour methods

    public void StartNewState(BaseAIState NextAIState)
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
        CurrentState.OnStateBegin();
    }
}
