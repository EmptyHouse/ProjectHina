using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EHGlobalEffectManager : ITickableComponent
{
    #region camera shake effect variables
    private float TimeRemainingForCameraShake;
    private float CachedCameraShakeIntensity;
    #endregion

    #region freeze time effect variables
    private float TimeRemainingFreezeTime;
    private float CachedTimeForFreezeTime;
    private float CachedTimeScaleBeforeFreeze;
    private UnityAction OnFreezeTimeComplete;
    #endregion


    public void Tick(float DeltaTime)
    {
        if (TimeRemainingForCameraShake > 0)
        {
            UpdateCameraShake(DeltaTime);
        }

        if (TimeRemainingFreezeTime > 0)
        {
            UpdateFreezeTime(EHTime.RealDeltaTime);
        }
    }    

    /// <summary>
    /// Begins a camera shake coroutine that will take place for the desired amount of time. The camera shake with the higher camera shake intenstity will take precedents
    /// 
    /// </summary>
    /// <param name="TimeInSeconds"></param>
    /// <param name="CameraShakeIntensity"></param>
    public void StartCameraShake(float TimeInSeconds, float CameraShakeIntensity)
    {
        if (CameraShakeIntensity < CachedCameraShakeIntensity)
        {
            return;
        }
        CachedCameraShakeIntensity = CameraShakeIntensity;
        TimeRemainingForCameraShake = TimeInSeconds;
    }

    private void UpdateCameraShake(float DeltaTime)
    {
        TimeRemainingForCameraShake -= DeltaTime;
        if (TimeRemainingForCameraShake <= 0)
        {
            CameraShakeEnd();
        }
    }

    private void CameraShakeEnd()
    {
        CachedCameraShakeIntensity = 0;
    }

    /// <summary>
    /// Sets a timer that will freeze the game for the desired seconds that are passed in. If there is already a freeze time that is set in place
    /// the one that has more time remaining will take precedent
    /// </summary>
    /// <param name="TimeInSeconds"></param>
    public void StartFreezeTimeForSeconds(float TimeInSeconds, UnityAction OnFreezeTimeComplete = null)
    {
        if (TimeInSeconds > CachedTimeForFreezeTime)
        {
            TimeRemainingFreezeTime = TimeInSeconds;
            CachedTimeForFreezeTime = TimeInSeconds;
            CachedTimeScaleBeforeFreeze = EHTime.TimeScale;
            EHTime.SetTimeScale(0);
        }
        if (OnFreezeTimeComplete != null)
        {
            this.OnFreezeTimeComplete += OnFreezeTimeComplete;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="RealDeltaTime"></param>
    private void UpdateFreezeTime(float RealDeltaTime)
    {
        TimeRemainingFreezeTime -= RealDeltaTime;
        if (TimeRemainingFreezeTime <= 0)
        {
            FreezeTimeEnd();
        }
    }

    private void FreezeTimeEnd()
    {
        CachedTimeForFreezeTime = 0;
        CachedTimeScaleBeforeFreeze = 1;
        EHTime.SetTimeScale(CachedTimeScaleBeforeFreeze);

        OnFreezeTimeComplete?.Invoke();
        OnFreezeTimeComplete = null;
    }
}
