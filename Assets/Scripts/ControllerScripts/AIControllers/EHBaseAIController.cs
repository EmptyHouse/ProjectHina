using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract  class EHBaseAIController : EHController
{
    private List<BaseAIState> AIStateList = new List<BaseAIState>();
    private BaseAIState CurrentState;
    #region monobehaviour methods
    protected virtual void Awake()
    {
        InitializeAIController();
    }

    protected virtual void Start()
    {
        StartNewState(AIStateList[0]);
    }

    protected virtual void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.OnStateTick(EHTime.DeltaTime);
        }
    }
    #endregion monobehaivour methods
    protected abstract void InitializeAIController();

    protected void AddState(BaseAIState AIState)
    {
        AIStateList.Add(AIState);
    }

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
