using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class groundTile : Tile
{
    public Sprite[] groundSprites;
    private Vector3Int tileVector;
    // For tracking wetness.
    public int wetnesSpread = 5;
    public List<groundTile> waterSources = new List<groundTile>();
    

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        // On refresh, we want to check if we need to get wetter or drier.
        if (waterSources != null && waterSources.Count == 0)
        {
            // Zero water sources, we need to dry out.
            
        }




        for (int yd = -1; yd <= 1; yd++)
        {
            Vector3Int location = new Vector3Int(position.x, position.y + yd, position.z);
            if (IsNeighbour(location, tilemap))
                tilemap.RefreshTile(location);
        }
        for (int xd = -1; xd <= 1; xd++)
        {
            Vector3Int location = new Vector3Int(position.x + xd, position.y, position.z);
            if (IsNeighbour(location, tilemap))
                tilemap.RefreshTile(location);
        }
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
    }

    public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject go)
    {
        if (go != null)
        {
            tileVector = location;
        }
        return true;
    }

    private bool IsNeighbour(Vector3Int position, ITilemap tilemap)
    {
        TileBase tile = tilemap.GetTile(position);
        return (tile != null && tile == this);
    }

    // Remove a water source from our water source list, given a Tile.
    public void RemoveWaterSource(groundTile waterSource)
    {
        waterSources.Remove(waterSource);

        // After removing a water source, does this tile get drier?
    }

    // Add a water source to our water source list, given a Tile.
    public void AddWaterSource(groundTile waterSource)
    {
        // First check if this water source is withing our relevant radius.
        if (true)//GridManager.OddrDistance(waterSource.GetTileVector(), tileVector))
        {
            // waterSources.Add(waterSource);
        }

        // After adding a water source, does this tile get wetter?

    }

    // Get's tile location
    public Vector3Int GetTileVector()
    {
        return tileVector;
    }
}
