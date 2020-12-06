using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EHMovementComponent))]
public class DashComponent : MonoBehaviour
{
    [Tooltip("")]
    public float InitialDashSpeed = 20f;
    [Tooltip("")]
    public float DashTime = .016f * 4;
    [Tooltip("")]
    public float DelayBeforeStartDash;
    [Tooltip("The amount of time to wait before you can use dash again")]
    public float DashCoolDown = .1f;
    [Tooltip("Curve will determine the velocity of our character when using the dash ability")]
    public AnimationCurve DashAnimationCurve;

    private bool bIsPerformingDash;


    public void AttemptDash()
    {
        if (!bIsPerformingDash)
        {
            StartCoroutine(BeginDash());
        }
    }

    private IEnumerator BeginDash()
    {
        bIsPerformingDash = true;
        yield return StartCoroutine(PerformDelayBeforeDash());
        bIsPerformingDash = false;
    }

    private IEnumerator PerformDelayBeforeDash()
    {
        if (DelayBeforeStartDash <= 0) yield break;
        float CachedTimeScale = EHTime.TimeScale;
        EHTime.SetTimeScale(0);
        float TimeThatHasPassed = 0;
        while (TimeThatHasPassed < DelayBeforeStartDash)
        {
            yield return null;
            TimeThatHasPassed += EHTime.RealDeltaTime;
        }
        EHTime.SetTimeScale(CachedTimeScale);
    }

    private IEnumerator PerformDash()
    {
        yield break;
    }

    private IEnumerator PerformDashCoolDown()
    {
        yield break;
    }
}
