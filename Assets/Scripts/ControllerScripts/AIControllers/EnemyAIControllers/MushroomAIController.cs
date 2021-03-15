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
    }

    protected override void Start()
    {
        base.Start();
        StartNewState(MushroomIdleState);
    }

    [System.Serializable]
    /// <summary>
    /// 
    /// </summary>
    private class MushroomIdle : BaseAIState
    {
        [SerializeField]
        private float PatrolRange = 5f;

        private float TimeUntilTurnAroundIfStuck = 2f;
        private Vector3 OriginalPosition;
        private float LeftEndPoint;
        private float RightEndPoint;
        private bool bPatrolRight;
        private EHBaseMovementComponent MovementComponent;


        public override void InitilalizeState(EHBaseAIController AIControllerOwner)
        {
            base.InitilalizeState(AIControllerOwner);
            OriginalPosition = AIControllerOwner.transform.position;
            MovementComponent = AIControllerOwner.GetComponent<EHBaseMovementComponent>();
        }

        public override void OnStateBegin()
        {
            bPatrolRight = Random.Range(0, 2) != 0;
            LeftEndPoint = OriginalPosition.x - PatrolRange / 2f;
            RightEndPoint = OriginalPosition.x + PatrolRange / 2f;
        }

        public override void OnStateEnded()
        {
            
        }

        public override void OnStateTick(float DeltaTime)
        {
            MovementComponent.SetHorizontalInput(bPatrolRight ? 1 : -1);
            if (bPatrolRight)
            {
                if (AIControllerOwner.transform.position.x > RightEndPoint)
                {
                    bPatrolRight = !bPatrolRight;
                }
            }
            else
            {
                if (AIControllerOwner.transform.position.x < LeftEndPoint)
                {
                    bPatrolRight = !bPatrolRight;
                }
            }
        }
    }

    [System.Serializable]
    private class MushroomAgro : BaseAIState
    {
        public override void InitilalizeState(EHBaseAIController AIControllerOwner)
        {
            base.InitilalizeState(AIControllerOwner);
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
