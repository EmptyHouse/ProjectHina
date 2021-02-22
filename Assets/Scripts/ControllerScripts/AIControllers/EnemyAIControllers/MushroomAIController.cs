using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomAIController : EHBaseAIController
{
    [SerializeField]
    private MushroomIdle MushroomIdleState;
    [SerializeField]
    private MushroomAgro MushroomAgroState;

    protected override void Awake()
    {
        base.Awake();
        StartNewState(MushroomIdleState);
    }

    private void OnValidate()
    {
        if (!MushroomIdleState)
        {
            MushroomIdleState = new MushroomIdle(this);
            MushroomAgroState = new MushroomAgro(this);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private class MushroomIdle : BaseAIState
    {
        [SerializeField]
        private float PatrolRange = 5f;


        public MushroomIdle(EHBaseAIController AIControllerOwner) : base(AIControllerOwner)
        {
        }

        public override void OnStateBegin()
        {
            throw new System.NotImplementedException();
        }

        public override void OnStateEnded()
        {
            throw new System.NotImplementedException();
        }

        public override void OnStateTick(float DeltaTime)
        {
            throw new System.NotImplementedException();
        }
    }


    private class MushroomAgro : BaseAIState
    {
        public MushroomAgro(EHBaseAIController AIControllerOwner) : base(AIControllerOwner)
        { 

        }
        
        public override void OnStateBegin()
        {

        }

        public override void OnStateTick(float DeltaTime)
        {

        }

        public override void OnStateEnded()
        {

        }
    }
}
