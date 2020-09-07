using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GroundTileManager : MonoBehaviour
{
    public GameObject gameData;
    public static GroundTileManager instance;
    private PlantTileManager plantTileManager;
    public Tilemap tilemap;
    private List<Vector3Int> allTilePositions;
    public int marshDistance = 2;
    public int soilDistance = 5;
    public Dictionary<Vector3Int, GroundTile> groundTiles = new Dictionary<Vector3Int, GroundTile>();
    public Dictionary<string, Tile> groundTileFromName = new Dictionary<string, Tile>();
    public Dictionary<string, Terrain> terrainFromName = new Dictionary<string, Terrain>();

    // For tweaking speed that tiles change based on distance to water
    public float baseChangeTime;

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
    }


    private void Start()
    {
        plantTileManager = gameObject.GetComponent<GridManager>().plantTileManager;
        groundTileFromName = gameData.GetComponent<LoadGameData>().groundTileFromName;
        terrainFromName = gameData.GetComponent<LoadGameData>().terrainFromName;
        allTilePositions = gameObject.GetComponent<GridManager>().allTilePositions;
        GetGroundTiles();
    }


    // Use this for initialization
    private void GetGroundTiles()
    {
        foreach (Vector3Int pos in allTilePositions)
        {
            if (!tilemap.HasTile(pos)) continue;

            Dictionary<int, List<Vector3Int>> nearbyWaterTiles = GetNearbyWaterTiles(pos, tilemap, soilDistance);
            int distanceToWater = GetDistanceToWater(pos, nearbyWaterTiles);
            // Set this tile to the type corresponding to its DistanceToWater, MarshDistance, and SoilDistance.
            Tile thisTile = GetTileByWaterDistance(distanceToWater, marshDistance, soilDistance);
            tilemap.SetTile(pos, thisTile);

            var tile = ScriptableObject.CreateInstance<GroundTile>();
            tile.GridVector = pos;
            tile.WorldVector = tilemap.CellToWorld(pos);
            tile.ThisTile = tilemap.GetTile<Tile>(pos);
            tile.TilemapMember = tilemap;
            tile.Name = pos.x + "," + pos.y;
            tile.NearbyWaterTiles = nearbyWaterTiles;
            tile.DistanceToWater = distanceToWater;
            tile.MarshDistance = marshDistance;
            tile.SoilDistance = soilDistance;
            tile.Terrain = terrainFromName[thisTile.name];

            groundTiles.Add(tile.GridVector, tile);
        }
    }


    public Dictionary<int, List<Vector3Int>> GetNearbyWaterTiles(Vector3Int tilePos, Tilemap tilemap, int soilDistance)
    {
        Dictionary<int, List<Vector3Int>> result = new Dictionary<int, List<Vector3Int>>();

        // Use HexMath to get list of nearbyTiles.
        List<Vector3Int> neighborPositions = HexMath.OddrRange(tilePos, soilDistance);

        // For each neighborPosition, check the tile there, looking out for water tiles.
        foreach (Vector3Int nPos in neighborPositions)
        {
            // Is there even a tile here?
            if (!tilemap.HasTile(nPos)) continue;

            // Is this tile water?
            if (tilemap.GetTile<Tile>(nPos).name.Equals("Water", System.StringComparison.Ordinal))
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
            return groundTileFromName["Water"];
        }
        else if (distanceToWater > 0 && distanceToWater <= marshDistance)
        {
            // The tile should be Marsh.
            return groundTileFromName["Marsh"];

        }
        else if (distanceToWater > marshDistance && distanceToWater <= soilDistance)
        {
            // The tile should be Soil.
            return groundTileFromName["Soil"];

        }
        else
        {
            // Tile should be Barren otherwise.
            return groundTileFromName["Barren"];
        }
    }


    // PLACE WATER
    public void PlaceWater(Vector3Int tileLocation)
    {
        // Set the target tile to be a water tile.
        tilemap.SetTile(tileLocation, groundTileFromName["Water"]);

        // Update groundTiles dictionary.
        groundTiles[tileLocation].ThisTile = groundTileFromName["Water"];
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

        // Last but not least, clear any plant on that tile?
        plantTileManager.ClearPlants(tileLocation);
    }
    
    
    // REMOVE WATER
    public void RemoveWater(Vector3Int tileLocation)
    {
        // Set the target tile to be a marsh tile. (OR WHATEVER REMOVED WATER TURNS INTO)
        tilemap.SetTile(tileLocation, groundTileFromName["Marsh"]);
        // Update groundTiles dictionary.
        groundTiles[tileLocation].ThisTile = groundTileFromName["Marsh"];

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
            float timeToWait = baseChangeTime * RandomNumberFunctions.RandomGaussian() * Mathf.Pow((1.0f + Mathf.Sqrt(5.0f)) / 2.0f, dTW);
            
            // Pass the time with a gentle song...
            yield return new WaitForSeconds(timeToWait);

            // After that time has passed, did our distance to water change?
            int newDTW = groundTiles[tilePos].DistanceToWater;

            // Also, is it still appropriate to change into the tile we set out to become?
            Tile newNTile = GetTileByWaterDistance(newDTW, marshDistance, soilDistance);

            // If distanceToWater changed, there was a shift in water tile locations while we waited. 
            //  We assume that groundTiles dictionary has the correct distanceToWater.
            if (dTW == newDTW)
            {
                // There was no change in distance while we waited. 
                // The last thing we want to do is make sure not to directly from barren to marsh or marsh to barren.
                if ((newNTile.name.Equals("Barren", System.StringComparison.Ordinal) && thisTile.name.Equals("Marsh", System.StringComparison.Ordinal)) ||
                    (newNTile.name.Equals("Marsh", System.StringComparison.Ordinal) && thisTile.name.Equals("Barren", System.StringComparison.Ordinal)))
                {
                    // Go to the intermediary Soil tile instead and loop around again.
                    tilemap.SetTile(tilePos, groundTileFromName["Soil"]);

                    // Remember to update groundTiles dictionary!
                    groundTiles[tilePos].ThisTile = nTile;
                    groundTiles[tilePos].Terrain = terrainFromName[nTile.name];

                }
                else
                {
                    // We are safe to jump to the final tile.
                    tilemap.SetTile(tilePos, newNTile);

                    // Remember to update groundTiles dictionary!
                    groundTiles[tilePos].ThisTile = newNTile;

                    // Break from the while loop.
                    break;
                }
                
            }
            else
            {
                // There was a shift in distance to water while we waited. We may want to repeat this loop.
            }
        }
    }
}
