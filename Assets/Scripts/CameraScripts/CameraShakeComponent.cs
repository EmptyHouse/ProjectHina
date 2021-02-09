using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeComponent : MonoBehaviour
{
    private float TimeRemainingForCameraShake;
    private float CameraShakeIntensity;

    private bool CameraShakeRunning;



    public void BeginCameraShake(float TimeForCameraShake, float CameraIntensity)
    {
        this.TimeRemainingForCameraShake = TimeForCameraShake;
        this.CameraShakeIntensity = CameraIntensity;
        StartCoroutine(BeginCameraShakeCoroutine());
    }

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
