using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundTileManager : MonoBehaviour
{
    public static GroundTileManager instance;
    public Tilemap tilemap;
    public int marshDistance = 2;
    public int soilDistance = 5;
    public Dictionary<Vector3, GroundTile> groundTiles;
    public GroundTilePallete groundTilePallete;
    private Tile[] groundTileArray;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        groundTileArray = groundTilePallete.groundTilePallete;
        GetGroundTiles();
    }

    // Use this for initialization
    private void GetGroundTiles()
    {
        groundTiles = new Dictionary<Vector3, GroundTile>();
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos)) continue;

            Dictionary<int, List<Vector3Int>> nearbyWaterTiles = GetNearbyWaterTiles(pos, tilemap, soilDistance);
            int distanceToWater = GetDistanceToWater(pos, nearbyWaterTiles);
            // Set this tile to the type corresponding to its DistanceToWater, MarshDistance, and SoilDistance.
            Tile thisTile = GetTileByWaterDistance(distanceToWater, marshDistance, soilDistance);
            tilemap.SetTile(pos, thisTile);

            var tile = new GroundTile
            {
                GridVector = pos,
                WorldVector = tilemap.CellToWorld(pos),
                ThisTile = tilemap.GetTile(pos),
                TilemapMember = tilemap,
                Name = pos.x + "," + pos.y,
                NearbyWaterTiles = nearbyWaterTiles,
                DistanceToWater = distanceToWater,
                MarshDistance = marshDistance,
                SoilDistance = soilDistance
            };

            groundTiles.Add(tile.GridVector, tile);
        }
    }


    public Dictionary<int, List<Vector3Int>> GetNearbyWaterTiles(Vector3Int tilePos, Tilemap tilemap, 
                                                                int soilDistance)
    {
        Dictionary<int, List<Vector3Int>> result = new Dictionary<int, List<Vector3Int>>();

        // Use HexMath to get list of nearbyTiles.
        List<Vector3Int> neighborPositions = HexMath.OddrRange(tilePos, soilDistance);

        // For each neighborPosition, check the tile there, looking out for water tiles.
        foreach (Vector3Int nPos in neighborPositions)
        {
            // Is there even a tile here?
            if (!tilemap.HasTile(nPos)) continue;
            // Is this tile water based off its sprite?
            if (tilemap.GetTile<Tile>(nPos).sprite == groundTileArray[0].sprite)
            {
                // Get the distance from this tile to the water tile.
                int distanceToWater = HexMath.OddrDistance(tilePos, nPos);

                // If the result dictionary has an entry for this distance, add nPos to its value's list.
                if (result.ContainsKey(distanceToWater))
                {
                    result[distanceToWater].Add(nPos);
                }
                // Otherwise, the result dictionary has no entry for this distance yet, so make one.
                else
                {
                    result.Add(distanceToWater, new List<Vector3Int> { nPos });
                }
            }
        }

        return result;
    }


    public int GetDistanceToWater(Vector3Int tilePos, Dictionary<int, List<Vector3Int>> nearbyWaterTiles)
    {
        int result = soilDistance + 1;

        for (int d = 0; d < soilDistance +1; d++)
        {
            // Check if the dictionary contains this distance key or not.
            if (nearbyWaterTiles.ContainsKey(d) && nearbyWaterTiles[d].Count > 0)
            {
                return d;
            }
        }

        return result;
    }


    public Tile GetTileByWaterDistance(int distanceToWater, int marshDistance, int soilDistance)
    {
        if (distanceToWater == 0)
        {
            // The tile should be Water.
            return groundTileArray[0];
        }
        else if (distanceToWater > 0 && distanceToWater <= marshDistance)
        {
            // The tile should be Marsh.
            return groundTileArray[1];
        }
        else if (distanceToWater > marshDistance && distanceToWater <= soilDistance)
        {
            // The tile should be Soil.
            return groundTileArray[2];
        }
        else
        {
            // Tile should be Barren otherwise.
            return groundTileArray[3];
        }
    }


    // PLACE WATER
    public void PlaceWater(Vector3Int tileLocation)
    {
        // Set the target tile to be a water tile.
        tilemap.SetTile(tileLocation, groundTileArray[0]);
        // Update groundTiles dictionary.
        groundTiles[tileLocation].ThisTile = groundTileArray[0];

        // Get the neighbors.
        List<Vector3Int> neighbors = HexMath.OddrRange(tileLocation, 5);
        foreach (Vector3Int nPosition in neighbors)
        {
            // Look for the tile in the groundTiles dictionary.
            if (groundTiles.ContainsKey(nPosition))
            {
                // Take a look at the neighbor. Look at its NearbyWaterTiles dictionary and DistanceToWater.
                // We want to add this new water into the dictionary and, if DistanceToWater just shrank, update
                // the neighbor to the appropriate tile type.
                int dTW = HexMath.OddrDistance(tileLocation, nPosition);
                if (groundTiles[nPosition].NearbyWaterTiles.ContainsKey(dTW))
                {
                    // This value for dTW exists in the dictionary. Merely add this new water tile to the list.
                    groundTiles[nPosition].NearbyWaterTiles[dTW].Add(tileLocation);
                }
                else
                {
                    // This value for dTW did not exist in the dictionary. Create a new key value pair.
                    groundTiles[nPosition].NearbyWaterTiles.Add(dTW, new List<Vector3Int> { tileLocation });
                }

                // Did the neighbor get closer to water?
                if (dTW < groundTiles[nPosition].DistanceToWater)
                {
                    // Set this tile according to its distanceToWater.
                    TileBase nTile = GetTileByWaterDistance(dTW, marshDistance, soilDistance);
                    tilemap.SetTile(nPosition, nTile);
                    // Remember to update groundTiles dictionary!
                    groundTiles[nPosition].DistanceToWater = dTW;
                    groundTiles[nPosition].ThisTile = nTile;
                }
            }
        }
    }
    
    
    // REMOVE WATER
    public void RemoveWater(Vector3Int tileLocation)
    {
        // Set the target tile to be a marsh tile. (OR WHATEVER REMOVED WATER TURNS INTO)
        tilemap.SetTile(tileLocation, groundTileArray[1]);
        // Update groundTiles dictionary.
        groundTiles[tileLocation].ThisTile = groundTileArray[1];

        // Get the neighbors.
        List<Vector3Int> neighbors = HexMath.OddrRange(tileLocation, 5);
        foreach (Vector3Int nPosition in neighbors)
        {
            // Look for the tile in the groundTiles dictionary.
            if (groundTiles.ContainsKey(nPosition))
            {
                // Take a look at the neighbor. Look at its NearbyWaterTiles dictionary and DistanceToWater.
                // We want to remove this water from the dictionary and, if DistanceToWater just rose, update
                // the neighbor to the appropriate tile type.
                int dTW = HexMath.OddrDistance(tileLocation, nPosition);
                if (groundTiles[nPosition].NearbyWaterTiles.ContainsKey(dTW))
                {
                    // This value for dTW exists in the dictionary.
                    if (groundTiles[nPosition].NearbyWaterTiles[dTW].Count >= 2)
                    {
                        // Now if it's the not final item in for this key, just remove it from the list.
                        groundTiles[nPosition].NearbyWaterTiles[dTW].Remove(tileLocation);
                    }
                    else
                    {
                        // Otherwise, the list of water tiles at this exact dTW was only this tile. 
                        // Therefore, remove the key value pair from the NearbyWaterTiles dictionary.
                        groundTiles[nPosition].NearbyWaterTiles.Remove(dTW);
                    }
                }
                else
                {
                    // SOMETHING HAS GONE TERRIBLY WRONG: This was a water tile and was within the radius of
                    // influence of this neighbor, yet doesn't show up in the neighbors NearbyWaterTiles?????
                    Debug.Log("Something has gone wrong, removed water tile, at " + tileLocation + " not found" +
                        " in neighbor's near by water tiles, at " + nPosition);
                }

                // Did the neighbor get further from water?
                dTW = GetDistanceToWater(nPosition, groundTiles[nPosition].NearbyWaterTiles);
                if (dTW != groundTiles[nPosition].DistanceToWater)
                {
                    // Set this tile according to its distanceToWater.
                    TileBase nTile = GetTileByWaterDistance(dTW, marshDistance, soilDistance);
                    tilemap.SetTile(nPosition, nTile);
                    // Remember to update groundTiles dictionary!
                    groundTiles[nPosition].DistanceToWater = dTW;
                    groundTiles[nPosition].ThisTile = nTile;
                }
            }
        }
    }
}
