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
        private EHBox2DCollider PlayerCollider;
        private float Range = 15f;

        public FollowPlayerState(EHBaseAIController AIController) : base(AIController)
        {
            WispController = (WispAIController)AIController;
        }

        public override void OnStateBegin()
        {
            TargetTransform = BaseGameOverseer.Instance.PlayerController.transform;
            PlayerCollider = BaseGameOverseer.Instance.PlayerController.GetComponent<EHBox2DCollider>();
        }

        public override void OnStateEnded()
        {
            
        }

        public override void OnStateTick(float DeltaTime)
        {
            float Distance = Vector2.Distance(TargetTransform.position, AIControllerOwner.transform.position);
            if (Distance < Range)
            {
                EHBounds2D PlayerCharacterBounds = PlayerCollider.GetBounds();
                Vector3 CenterColliderPosition = PlayerCharacterBounds.MinBounds + (PlayerCharacterBounds.MaxBounds - PlayerCharacterBounds.MinBounds) / 2f;
                Vector2 DirectionToFly = CenterColliderPosition - AIControllerOwner.transform.position;
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
