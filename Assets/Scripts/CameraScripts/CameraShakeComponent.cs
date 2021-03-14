using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Component to handle our camera shake logic
/// </summary>
public class CameraShakeComponent : MonoBehaviour
{
    // Time until our camera shake ends
    private float TimeRemainingForCameraShake;
    // The intensity of our camera shake
    private float CameraShakeIntensity;
    
    // Indicates if our camera shake is currently running
    private bool CameraShakeRunning;


    /// <summary>
    /// starts a coroutine to begin the camera shake process. Keep in mind that the last Camera shake process will override previous camera shake calls
    /// even if they are currently running
    /// </summary>
    /// <param name="TimeForCameraShake"></param>
    /// <param name="CameraIntensity"></param>
    public void BeginCameraShake(float TimeForCameraShake, float CameraIntensity)
    {
        this.TimeRemainingForCameraShake = TimeForCameraShake;
        this.CameraShakeIntensity = CameraIntensity;
        StartCoroutine(BeginCameraShakeCoroutine());
    }

    /// <summary>
    /// Coroutine to handle the logic for our camera shake
    /// </summary>
    /// <returns></returns>
    private IEnumerator BeginCameraShakeCoroutine()
    {
        if (CameraShakeRunning) yield break;
        CameraShakeRunning = true;
        Vector3 OriginalLocalPosition = transform.localPosition;

        float XShakePos, YShakePos;

        while (TimeRemainingForCameraShake > 0)
        {
            XShakePos = Random.Range(-1f, 1) * CameraShakeIntensity;
            YShakePos = Random.Range(-1f, 1f) * CameraShakeIntensity;
            transform.localPosition = new Vector3(XShakePos, YShakePos, 0) + OriginalLocalPosition;

            yield return null;
            TimeRemainingForCameraShake -= EHTime.DeltaTime;
        }

        transform.localPosition = OriginalLocalPosition;
        CameraShakeRunning = false;
    }
}
