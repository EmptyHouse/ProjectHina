using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeComponent : MonoBehaviour
{
    public float TimeRemainingForCameraShake;
    public float CameraShakeIntensity;




    private IEnumerator CameraShakeCoroutine()
    {
        yield return null;
    }
}
