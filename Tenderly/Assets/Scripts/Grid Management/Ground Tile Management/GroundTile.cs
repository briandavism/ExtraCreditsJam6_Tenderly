using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Taken from template code found: https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
public class GroundTile
{
    public Vector3Int GridVector { get; set; }
    public Vector3 WorldVector { get; set; }
    public Tile ThisTile { get; set; }
    public Tilemap TilemapMember { get; set; }
    public string Name { get; set; }


    public int DistanceToWater { get; set; }
    public Dictionary<int, List<Vector3Int>> NearbyWaterTiles { get; set; }
    public int MarshDistance { get; set; }
    public int SoilDistance { get; set; }
}




/*
[CreateAssetMenu]
public class GroundTile : TileBase
{
    public Sprite[] groundSprites;
    public int marshDistance = 2;
    public int soilDistance = 5;


    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        // If this tile is a water tile, refresh all the other neighbors to check for sprite changes.
        if (tilemap.GetSprite(position) == groundSprites[0])
        {
            // Foreach neighborLocation in range of the soilDistance, refresh that tile.
            List<Vector3Int> neighborsInRange = HexMath.OddrRange(position, soilDistance);
            foreach (Vector3Int neighborLocation in neighborsInRange)
            {
                if (tilemap.GetTile(neighborLocation) == null)
                {
                    continue;
                }

                // Refresh every tile.
                tilemap.RefreshTile(neighborLocation);
            }
        }

        // Now refresh this tile.
        tilemap.RefreshTile(position);
    }


    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        // Assume distanceToWater is 6 or greater, meaning this tile should be barren.
        int distanceToWater = 6;

        // Foreach neighborLocation in range of the soilDistance, look for nearest water.
        List<Vector3Int> neighborsInRange = HexMath.OddrRange(position, soilDistance);
        foreach (Vector3Int neighborLocation in neighborsInRange)
        {
            if (tilemap.GetTile(neighborLocation) == null)
            {
                continue;
            }

            Sprite neighborSprite = tilemap.GetSprite(neighborLocation);

            if (neighborSprite == groundSprites[0])
            {
                // The neighbor is a water tile. Calculate its distance to this tile.
                int dTW = HexMath.OddrDistance(position, neighborLocation);

                // If it's closer than current distanceToWater, update dTW.
                if (dTW < distanceToWater)
                {
                    distanceToWater = dTW;
                }
            }

        }

        // After checking all the neighbors, adjust this tile's sprite based on distanceToWater.
        if (distanceToWater == 0)
        {
            // The tile should be Water.
            tileData.sprite = groundSprites[0];
        }
        else if (distanceToWater > 0 && distanceToWater <= marshDistance)
        {
            // The tile should be Marsh.
            tileData.sprite = groundSprites[1];
        }
        else if (distanceToWater > marshDistance && distanceToWater <= soilDistance)
        {
            // The tile should be Soil.
            tileData.sprite = groundSprites[2];
        }
        else
        {
            // Tile should be Barren otherwise.
            tileData.sprite = groundSprites[3];
        }
    }


    public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject go)
    {

        return true;
    }


    // Remove a water source from our water source list, given a Tile.
    public void RemoveWaterSource(GroundTile waterSource)
    {
        //waterSources.Remove(waterSource);

        // After removing a water source, does this tile get drier?
    }

    // Add a water source to our water source list, given a Tile.
    public void AddWaterSource(GroundTile waterSource)
    {
        // First check if this water source is withing our relevant radius.
        if (true)//GridManager.OddrDistance(waterSource.GetTileVector(), tileVector))
        {
            // waterSources.Add(waterSource);
        }

        // After adding a water source, does this tile get wetter?

    }
} */


/*
[CreateAssetMenu]
public class GroundTile : TileBase
{
    public Sprite[] groundSprites;
    public int distanceToWater;
    public Dictionary<int, List<Vector3Int>> nearbyWaterTiles = new Dictionary<int, List<Vector3Int>>();

    public int marshDistance = 2;
    public int soilDistance = 5;


    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        // If this tile is a water tile, we'll need to refresh neighbors within a certain radius as well.
        if (distanceToWater == 0)
        {
            // Foreach neighborLocation in range of the soilDistance, call AddWaterAlert().
            List<Vector3Int> neighborsInRange = HexMath.OddrRange(position, soilDistance);
            foreach (Vector3Int neighborLocation in neighborsInRange)
            {
                AddWaterAlert(neighborLocation, position, tilemap);
            }
        }

        // Now refresh this tile.
        tilemap.RefreshTile(position);
    }


    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        // Basic validation incase the groundSprite array is messed up.
        if (groundSprites != null && groundSprites.Length <= 0)
        {
            return;
        }

        // Set the tileData.sprite to whichever is appropriate based on distanceToWater.
        if (distanceToWater == 0)
        {
            // The tile should be Water.
            tileData.sprite = groundSprites[0];
        }
        else if (distanceToWater > 0 && distanceToWater <= marshDistance)
        {
            // The tile should be Marsh.
            tileData.sprite = groundSprites[1];
        }
        else if (distanceToWater > marshDistance && distanceToWater <= soilDistance)
        {
            // The tile should be Soil.
            tileData.sprite = groundSprites[2];
        }
        else
        {
            // Tile should be Barren otherwise.
            tileData.sprite = groundSprites[3];
        }


    }


    public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject go)
    {

        return true;
    }


    // Remove a water source from our water source list, given a Tile.
    public void RemoveWaterSource(GroundTile waterSource)
    {
        //waterSources.Remove(waterSource);

        // After removing a water source, does this tile get drier?
    }

    // Add a water source to our water source list, given a Tile.
    public void AddWaterSource(GroundTile waterSource)
    {
        // First check if this water source is withing our relevant radius.
        if (true)//GridManager.OddrDistance(waterSource.GetTileVector(), tileVector))
        {
            // waterSources.Add(waterSource);
        }

        // After adding a water source, does this tile get wetter?

    }

    // Given a Vector3Int and tilemap, add this tile to its nearbyWaterTiles, maybe update its 
    //  distanceToWater, and refresh it if necessary.
    public void AddWaterAlert(Vector3Int neighborLocation, Vector3Int waterLocation, ITilemap tilemap)
    {
        GroundTile neighborTile = tilemap.GetTile<GroundTile>(neighborLocation);

        // If neighborTile is null, return.
        if (neighborTile == null)
        {
            return;
        }

        // Get the distance from the neighbor to this water tile.
        int distance = HexMath.OddrDistance(neighborLocation, waterLocation);

        // Also get the neighbors distanceToWater.
        int oldDistanceToWater = neighborTile.distanceToWater;

        // Add this water tile to the neighbor's nearbyWaterTiles.
        if (neighborTile.nearbyWaterTiles.ContainsKey(distance))
        {
            // There are water tiles at the same distance, add this one to the list.
            neighborTile.nearbyWaterTiles[distance].Add(waterLocation);
        }
        else
        {
            // There were no water tiles that distance away, so make a list with this water and add it.
            List<Vector3Int> newWaterTile = new List<Vector3Int> { waterLocation };
            neighborTile.nearbyWaterTiles.Add(distance, newWaterTile);
        }

        // Now that the tile's been added to the list, we want to check if it needs refreshing.
        // We do this by comparing the oldDistanceToWater with this water's distance.
        if (distance < oldDistanceToWater)
        {
            // New water tile is closer. Update distanceToWater on the neighborTile.
            neighborTile.distanceToWater = distance;

            // Refresh that tile.
            tilemap.RefreshTile(neighborLocation);
        }
    }
} */
