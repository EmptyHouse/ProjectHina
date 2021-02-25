using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAIController : EHBaseAIController
{
    [SerializeField]
    private RatIdleState RatIdle;
    [SerializeField]
    private RatAttackState RatAttack;

    protected override void Awake()
    {
        base.Awake();
        StartNewState(RatAttack);
    }

    [System.Serializable]
    private class RatIdleState : BaseAIState
    {
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

    [System.Serializable]
    private class RatAttackState : BaseAIState
    {
        public float TimeBetweenShots = 5;
        public float RangeBeforeIgnore = 10f;

        private float TimeSinceLastShot;
        private EHProjectileLauncher RatProjectilLauncherComponent;

        public override void OnStateBegin()
        {
            if (!RatProjectilLauncherComponent)
            {
                RatProjectilLauncherComponent = AIControllerOwner.GetComponent<EHProjectileLauncher>();
            }
        }

        public override void OnStateEnded()
        {
            
        }

        public override void OnStateTick(float DeltaTime)
        {
            TimeSinceLastShot += DeltaTime;
            if (TimeSinceLastShot >= TimeBetweenShots)
            {
                RatProjectilLauncherComponent.OnLaucnhProjectile();
                TimeSinceLastShot = 0;
                Debug.Log(TimeSinceLastShot);
            }
        }
    }
}
