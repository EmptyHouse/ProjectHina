using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EHEnvironment : MonoBehaviour
{
    public Tilemap EnvironmentTilemap;
    public Transform ColliderContainer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CreateWorldColliders();
        }
    }

    private void CreateWorldColliders()
    {
        RoomActor AssociatedRoomActor = GameObject.FindObjectOfType<RoomActor>();
        if (!AssociatedRoomActor)
        {
            return;
        }

        BoundsInt RoomBounds = new BoundsInt();
        RoomBounds.min = new Vector3Int(Mathf.FloorToInt(AssociatedRoomActor.GetMinRoomBounds().x), Mathf.FloorToInt(AssociatedRoomActor.GetMinRoomBounds().y), 0);
        RoomBounds.max = new Vector3Int(Mathf.CeilToInt(AssociatedRoomActor.GetMaxRoomBounds().x), Mathf.CeilToInt(AssociatedRoomActor.GetMaxRoomBounds().y), 0);
        Vector2Int TileMapSize = new Vector2Int(RoomBounds.max.x - RoomBounds.min.x, RoomBounds.max.y - RoomBounds.min.y);

        TileData tileData = new TileData();
        foreach (Tile tile in EnvironmentTilemap.GetTilesBlock(RoomBounds))
        {
            
        }
    }
}
