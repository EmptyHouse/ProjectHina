using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraFollow : MonoBehaviour
{
    [Range(0f, 1f)]
    public float CameraFollowSpeed = .5f;

    private Vector2 CameraOffsetFromTarget;
    private Transform TargetTransform;
    private Camera CameraComponent;

    #region monobehaviour methods
    private void Awake()
    {
        CameraComponent = GetComponentInChildren<Camera>();
    }
    #endregion monobehaviour methods

    public void SetCameraFollowTarget(Transform TargetTransform)
    {
        this.TargetTransform = TargetTransform;
    }

    public void SetCameraOffsetFromTarget(Vector2 CameraOffsetFromTarget)
    {
        this.CameraOffsetFromTarget = CameraOffsetFromTarget;
    }
}
