using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EHBox2DCollider))]
public class EnvironmentBoxCollider : MonoBehaviour
{
    [SerializeField]
    private EHBox2DCollider AssociatedCollier;
    private Vector2Int LockedPosition;
    [SerializeField]
    private Vector2Int ColliderSize = Vector2Int.one;

    #region monobehaviour methods
    private void Awake()
    {
        if (!AssociatedCollier)
        {
            Debug.LogWarning("Please Be Sure To Assign the Associated Box Collider");
            AssociatedCollier = GetComponent<EHBox2DCollider>();
        }
    }

    private void OnValidate()
    {
        LockedPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        if (ColliderSize.x < 0)
        {
            ColliderSize.x = 0;
        }
        if (ColliderSize.y < 0)
        {
            ColliderSize.y = 0;
        }
        if (!AssociatedCollier)
        {
            AssociatedCollier = GetComponent<EHBox2DCollider>();
        }
        AssociatedCollier.ColliderSize = ColliderSize;
        AssociatedCollier.transform.position = new Vector3(LockedPosition.x, LockedPosition.y);
        if (ColliderSize.x % 2 != 0)
        {
            AssociatedCollier.ColliderOffset.x = .5f;
        }
        else
        {
            AssociatedCollier.ColliderOffset.x = 0;
        }
        if (ColliderSize.y % 2 != 0)
        {
            AssociatedCollier.ColliderOffset.y = .5f;
        }
        else
        {
            AssociatedCollier.ColliderOffset.y = 0;
        }
    }
    #endregion monobehaviour methods
}
