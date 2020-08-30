﻿using System.Collections;
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
    // For tweaking speed that tiles change based on distance to water
    public float baseChangeTime;
    public float minGaussian;
    public float maxGaussian;

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
                ThisTile = tilemap.GetTile<Tile>(pos),
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
        groundTiles[tileLocation].DistanceToWater = 0;

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
                if (dTW == 0)
                {
                    // The tile has 0 distance to water, as in, it's looking at itself, so skip this one.
                    continue;
                }
                else if (groundTiles[nPosition].NearbyWaterTiles.ContainsKey(dTW))
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
                    // dTW changed, so we can update groundTiles dictionary.
                    groundTiles[nPosition].DistanceToWater = dTW;

                    // Which tile should it be, according to its distanceToWater?
                    Tile nTile = GetTileByWaterDistance(dTW, marshDistance, soilDistance);


                    // Start a cooroutine to change the tile.
                    StartCoroutine(DelayedGroundTileChange(nPosition));
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
                    // dTW changed, so we can update groundTiles dictionary.
                    groundTiles[nPosition].DistanceToWater = dTW;
                    
                    // Which tile should it be, according to its distanceToWater?
                    Tile nTile = GetTileByWaterDistance(dTW, marshDistance, soilDistance);

                    // Start a cooroutine to change the tile.
                    StartCoroutine(DelayedGroundTileChange(nPosition));
                }
            }
        }
    }

    // Delayed Tile Change: Water changes tiles, but tiles further away take longer.
    IEnumerator DelayedGroundTileChange(Vector3Int tilePos)
    {
        while (true)
        {
            // What are we?
            Tile thisTile = groundTiles[tilePos].ThisTile;

            // How far are we from water?
            int dTW = groundTiles[tilePos].DistanceToWater;

            // What should we be changing into?
            Tile nTile = GetTileByWaterDistance(dTW, marshDistance, soilDistance);

            // How long then should we wait to change?
            float timeToWait = baseChangeTime * RandomGaussian() * Mathf.Pow(dTW, (1.0f + Mathf.Sqrt(5.0f)) / 2.0f);

            // Pass the time with a gentle song...
            yield return new WaitForSeconds(timeToWait);

            // After that time has passed, did our distance to water change?
            int newDTW = groundTiles[tilePos].DistanceToWater;

            // Also, is it still appropriate to change into the tile we set out to become?
            Tile newNTile = GetTileByWaterDistance(newDTW, marshDistance, soilDistance);

            // If distanceToWater changed, there was a shift in water tile locations while we waited. We assume
            //  that groundTiles dictionary has the correct distanceToWater.
            if (dTW == newDTW)
            {
                // There was no change in distance while we waited. 
                tilemap.SetTile(tilePos, newNTile);

                // Remember to update groundTiles dictionary!
                groundTiles[tilePos].ThisTile = nTile;

                // Break from the while loop.
                break;
            }
            else
            {
                // There was a shift in distance to water while we waited. We may want to repeat this loop.
            }
        }
    }


    // For finding a normal distribution value.
    // Code borrowed from: https://answers.unity.com/questions/421968/normal-distribution-random.html
    public float RandomGaussian()
    {
        float minValue = minGaussian;
        float maxValue = maxGaussian;

        float u, v, S;

        do
        {
            u = 2.0f * Random.value - 1.0f;
            v = 2.0f * Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 5.0f;

        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }
}
