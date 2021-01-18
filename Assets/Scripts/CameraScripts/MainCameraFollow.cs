using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraFollow : MonoBehaviour
{ 
    public float CameraFollowSpeed = .5f;

    [Tooltip("The area that we will cover before moving toward the target. Acts as a buffer so that we are not ALWAYS following the player")]
    public Vector2 CameraCaptureRange;

    private Vector3 CameraOffsetFromTarget;
    private Transform TargetTransform;
    private Camera CameraComponent;

    #region monobehaviour methods
    private void Awake()
    {
        CameraComponent = GetComponentInChildren<Camera>();
        SetCameraFollowTarget(this.transform.parent);
        CameraOffsetFromTarget = this.transform.position - TargetTransform.position;

        this.transform.parent = null;
    }

    private void Start()
    {
        BaseGameOverseer.Instance.MainGameCamera = this;
    }

    private void LateUpdate()
    {
        Vector3 TargetPosition = TargetTransform.position + CameraOffsetFromTarget;


        this.transform.position = Vector3.Lerp(transform.position, TargetPosition, EHTime.DeltaTime * CameraFollowSpeed);
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

    public void FocusCameraImmediate()
    {
        this.transform.position = TargetTransform.position + CameraOffsetFromTarget;
    }
}
