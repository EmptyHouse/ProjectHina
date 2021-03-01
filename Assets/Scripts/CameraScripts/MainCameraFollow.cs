using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraFollow : MonoBehaviour
{ 
    public float CameraFollowSpeed = .5f;
    public CameraShakeComponent CameraShake { get; private set; }
    [Tooltip("The area that we will cover before moving toward the target. Acts as a buffer so that we are not ALWAYS following the player")]
    public Vector2 CameraCaptureRange;

    private Vector3 CameraOffsetFromTarget;
    private Transform TargetTransform;
    private Camera CameraComponent;

    #region monobehaviour methods
    private void Awake()
    {
        CameraComponent = GetComponentInChildren<Camera>();
        CameraShake = GetComponentInChildren<CameraShakeComponent>();

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
        UpdateCameraBounds();
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

    /// <summary>
    /// This should be called whenever a new room in our game is loaded
    /// </summary>
    /// <param name="RoomThatWasLoaded"></param>
    public void OnRoomLoaded(RoomActor RoomThatWasLoaded)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateCameraBounds()
    {
        // A lot of this stuff can probably be calculated at the beginning to avoid doind this every
        if (!BaseGameOverseer.Instance.CurrentlyLoadedRoom)
        {
            return;
        }
        Vector2 MinBounds, MaxBounds;
        float OrthoSize = CameraComponent.orthographicSize;
        Vector2 CameraSize = new Vector2(OrthoSize * Screen.width / Screen.height, OrthoSize);
        MinBounds = new Vector2(transform.position.x, transform.position.y) - CameraSize;
        MaxBounds = MinBounds + CameraSize * 2;
        Vector2 RoomMin = BaseGameOverseer.Instance.CurrentlyLoadedRoom.GetMinRoomBounds();
        Vector2 RoomMax = BaseGameOverseer.Instance.CurrentlyLoadedRoom.GetMaxRoomBounds();
        ////////////////////////////////////////////////////////////////////////////////////


        if (MinBounds.x < RoomMin.x)
        {
            transform.position += Vector3.right * (RoomMin.x - MinBounds.x);
        }
        else if (MaxBounds.x > RoomMax.x)
        {
            transform.position += Vector3.left * (MaxBounds.x - RoomMax.x);
        }

        if (MinBounds.y < RoomMin.y)
        {
            transform.position += Vector3.up * (RoomMin.y - MinBounds.y);
        }
        else if (MaxBounds.y > RoomMax.y)
        {
            transform.position += Vector3.down * (MaxBounds.y - RoomMax.y);
        }
    }
}
