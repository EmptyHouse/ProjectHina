using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispAIController : EHBaseAIController
{
    /// <summary>
    /// State to follow our player. You can set the range at which we will begin following our player
    /// </summary>
    [SerializeField]
    private WispFollowPlayer WispFollowPlayerState;

    /// <summary>
    /// Returns the 
    /// </summary>
    [SerializeField]
    private WispIdle WispIdleState;

    private EHFlightMovementComponent FlightMovement;


    protected override void Awake()
    {
        FlightMovement = GetComponent<EHFlightMovementComponent>();
        base.Awake();
        StartNewState(WispFollowPlayerState);
    }

    private void OnValidate()
    {
        if (!WispFollowPlayerState)
        {
            WispFollowPlayerState = ScriptableObject.CreateInstance<WispFollowPlayer>();
            WispIdleState = ScriptableObject.CreateInstance<WispIdle>();
        }
    }

    #region AI States
    private class WispIdle : BaseAIState
    {
        private Transform TargetTransform;

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

    private class WispFollowPlayer : BaseAIState
    {
        [SerializeField]
        private float Range = 15f;
        private WispAIController WispController;
        private Transform TargetTransform;
        private EHBox2DCollider PlayerCollider;


        public override void InitilalizeState(EHBaseAIController AIControllerOwner)
        {
            base.InitilalizeState(AIControllerOwner);
            WispController = (WispAIController)AIControllerOwner;
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
    #endregion AI States
}
