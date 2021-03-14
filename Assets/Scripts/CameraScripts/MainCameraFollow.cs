using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraFollow : MonoBehaviour
{ 
    public float CameraFollowSpeed = .5f;
    // Reference to the camera shake component that is attached to our main camera object
    public CameraShakeComponent CameraShake { get; private set; }
    [Tooltip("The area that we will cover before moving toward the target. Acts as a buffer so that we are not ALWAYS following the player")]
    public Vector2 CameraCaptureRange;

    // The offset that we will display our camera from the target transform
    private Vector3 CameraOffsetFromTarget;
    // A buffer so that our camera only follows our player if they are outside of this zone
    private Vector2 CameraBufferZone;
    // The target transform that our camera is following
    private Transform TargetTransform;
    // The attached CameraComponent
    private Camera CameraComponent;

    // TO-Do add functionality to support Camera zoom
    private float CameraZoomScale;
    private float CachedCameraOrthograpicSize;

    #region room variables
    // The cached bounds of our current room
    private EHBounds2D CachedRoomBounds;
    private bool bHasCachedRoomBounds;
    #endregion room variables

    #region monobehaviour methods
    private void Awake()
    {
        CameraComponent = GetComponentInChildren<Camera>();
        if (CameraComponent)
        {
            CachedCameraOrthograpicSize = CameraComponent.orthographicSize;
        }
        else
        {
            Debug.LogWarning("Cameara Component is not attached to our Main Camera Follow Object");
        }

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
    /// <summary>
    /// Sets our target transform. You can set it to snap to the target immediatley upon setting it.
    /// </summary>
    /// <param name="TargetTransform"></param>
    public void SetCameraFollowTarget(Transform TargetTransform, bool bSnapImmediate = false)
    {
        this.TargetTransform = TargetTransform;
        if (bSnapImmediate)
        {
            FocusCameraImmediate();
        }
    }

    /// <summary>
    /// Sets the offset for our camera from its target trasform
    /// </summary>
    /// <param name="CameraOffsetFromTarget"></param>
    public void SetCameraOffsetFromTarget(Vector2 CameraOffsetFromTarget)
    {
        this.CameraOffsetFromTarget = CameraOffsetFromTarget;
    }

    /// <summary>
    /// Sets the camera to focus on our target's transform immediately
    /// </summary>
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
        RoomActor CurrentRoom = BaseGameOverseer.Instance.CurrentlyLoadedRoom;
        if (CurrentRoom)
        {
            CachedRoomBounds.MaxBounds = BaseGameOverseer.Instance.CurrentlyLoadedRoom.GetMaxRoomBounds();
            CachedRoomBounds.MinBounds = BaseGameOverseer.Instance.CurrentlyLoadedRoom.GetMinRoomBounds();
            bHasCachedRoomBounds = true;
        }
    }

    /// <summary>
    /// Pushes our camera back into bounds if our camera reaches the room's designated bounds
    /// </summary>
    private void UpdateCameraBounds()
    {
        // A lot of this stuff can probably be calculated at the beginning to avoid doind this every
        if (!bHasCachedRoomBounds)
        {
            return;
        }
        Vector2 MinBounds, MaxBounds;
        float OrthoSize = CameraComponent.orthographicSize;
        Vector2 CameraSize = new Vector2(OrthoSize * Screen.width / Screen.height, OrthoSize);
        MinBounds = new Vector2(transform.position.x, transform.position.y) - CameraSize;
        MaxBounds = MinBounds + CameraSize * 2;
        Vector2 RoomMin = CachedRoomBounds.MinBounds;
        Vector2 RoomMax = CachedRoomBounds.MaxBounds;
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
