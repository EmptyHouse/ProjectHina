using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispAIController : EHBaseAIController
{
    private EHFlightMovementComponent FlightMovement;


    protected override void Awake()
    {
        FlightMovement = GetComponent<EHFlightMovementComponent>();
        base.Awake();
    }

    protected override void InitializeAIController()
    {
        AddState(new FollowPlayerState(this));
    }

    private class FollowPlayerState : BaseAIState
    {
        private WispAIController WispController;
        private Transform TargetTransform;
        private float Range = 15f;

        public FollowPlayerState(EHBaseAIController AIController) : base(AIController)
        {
            WispController = (WispAIController)AIController;
        }

        public override void OnStateBegin()
        {
            TargetTransform = BaseGameOverseer.Instance.PlayerController.transform;
        }

        public override void OnStateEnded()
        {
            
        }

        public override void OnStateTick(float DeltaTime)
        {
            float Distance = Vector2.Distance(TargetTransform.position, AIControllerOwner.transform.position);
            if (Distance < Range)
            {
                Vector2 DirectionToFly = TargetTransform.position - AIControllerOwner.transform.position;
                DirectionToFly.Normalize();
                WispController.FlightMovement.SetHorizontalInput(DirectionToFly.x);
                WispController.FlightMovement.SetVerticalInput(DirectionToFly.y);
            }
            else
            {
                WispController.FlightMovement.SetVerticalInput(0);
                WispController.FlightMovement.SetHorizontalInput(0);
            }
        }
    }
}
