using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantTileManager : MonoBehaviour
{
    public GameObject gameData;
    public static PlantTileManager instance;
    public Tilemap plantTilemap;
    private GridManager gridManager;
    private List<Vector3Int> allTilePositions;

    public Dictionary<Vector3Int, GroundTile> groundTileFromPosition = new Dictionary<Vector3Int, GroundTile>();
    public Dictionary<Vector3Int, PlantTile> plantTileFromPosition = new Dictionary<Vector3Int, PlantTile>();
    public Dictionary<string, Tile> groundTileFromName = new Dictionary<string, Tile>();
    public Dictionary<string, Tile> plantTileFromName = new Dictionary<string, Tile>();
    public Dictionary<string, Terrain> terrainFromName = new Dictionary<string, Terrain>();
    public Dictionary<string, Plant> plantFromName = new Dictionary<string, Plant>();

    // For Plant Spawning Coroutine
    public float spawnTimer;
    public float spawnTimerFudged;
    
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

        spawnTimerFudged = spawnTimer;
    }


    // Start is called before the first frame update
    void Start()
    {
        gridManager = gameObject.GetComponent<GridManager>();

        groundTileFromPosition = gridManager.GetComponent<GroundTileManager>().groundTiles;
        groundTileFromName = gameData.GetComponent<LoadGameData>().groundTileFromName;
        plantTileFromName = gameData.GetComponent<LoadGameData>().plantTileFromName;
        allTilePositions = gridManager.allTilePositions;
        terrainFromName = gameData.GetComponent<LoadGameData>().terrainFromName;
        plantFromName = gameData.GetComponent<LoadGameData>().plantFromName;

        GetPlantTiles();

        // To start spawning things, call the SpawnPlants method, which will start a coroutine.
        StartCoroutine(SpawnPeriodicPlants());
    }


    // Use this for initialization
    private void GetPlantTiles()
    {
        foreach (Vector3Int pos in allTilePositions)
        {
            if (!plantTilemap.HasTile(pos)) continue;

            Tile thisTile = gameData.GetComponent<LoadGameData>().plantTileFromName["Empty"];
            plantTilemap.SetTile(pos, thisTile);

            var tile = ScriptableObject.CreateInstance<PlantTile>();
            tile.GridVector = pos;
            tile.WorldVector = plantTilemap.CellToWorld(pos);
            tile.ThisTile = plantTilemap.GetTile<Tile>(pos);
            tile.TilemapMember = plantTilemap;
            tile.Name = pos.x + "," + pos.y;
            tile.Plant = gameData.GetComponent<LoadGameData>().plantFromName["Empty"];

            plantTileFromPosition.Add(tile.GridVector, tile);
        }
    }


    // Spawning:
    // Spawn New Plants:
    IEnumerator SpawnPeriodicPlants()
    {
        while (true)
        {
            // Spawn plants on some tile from the list of allTilePositions.
            foreach (Vector3Int tilePosition in allTilePositions)
            {
                PlantTile plantTile = plantTileFromPosition[tilePosition];
                Tile groundTile = gridManager.GetTiles(tilePosition)[0];

                // If there is a ground tile and the tile has no plants...
                if (plantTile is null || plantTile.ThisTile.name.Equals("Empty", System.StringComparison.Ordinal))
                {
                    switch (groundTile.name)
                    {
                        case "Water":
                            // Check if this tile will spawn by rolling under its associated spawnChance
                            if ((Random.Range(0f, 1f)) < terrainFromName["Water"].spawnRate)
                            {
                                // Which tile spawns? Get a random number from 1 to the totalSpawnChance
                                int randomDrop = Random.Range(1, terrainFromName["Water"].totalSpawnChance);

                                // Loop over each plant in the tarrain dropsDict and stop when randomDrop is <= 0
                                foreach (KeyValuePair<string, int> kvp in terrainFromName["Water"].dropsDict)
                                {
                                    randomDrop -= kvp.Value;

                                    if (randomDrop <= 0)
                                    {
                                        // Start a cooroutine to change the tile.
                                        StartCoroutine(DelayedPlantSpawn(tilePosition, plantTileFromName[kvp.Key]));
                                        CheckForMerge(tilePosition);
                                        break;
                                    }
                                }
                            }

                            break;
                        case "Marsh":
                            // Check if this tile will spawn by rolling under its associated spawnChance
                            if ((Random.Range(0f, 1f)) < terrainFromName["Marsh"].spawnRate)
                            {
                                // Which tile spawns? Get a random number from 1 to the totalSpawnChance
                                int randomDrop = Random.Range(1, terrainFromName["Marsh"].totalSpawnChance);

                                // Loop over each plant in the tarrain dropsDict and stop when randomDrop is <= 0
                                foreach (KeyValuePair<string, int> kvp in terrainFromName["Marsh"].dropsDict)
                                {
                                    randomDrop -= kvp.Value;

                                    if (randomDrop <= 0)
                                    {
                                        // Start a cooroutine to change the tile.
                                        StartCoroutine(DelayedPlantSpawn(tilePosition, plantTileFromName[kvp.Key]));
                                        break;
                                    }
                                }
                            }

                            break;
                        case "Soil":
                            // Check if this tile will spawn by rolling under its associated spawnChance
                            if ((Random.Range(0f, 1f)) < terrainFromName["Soil"].spawnRate)
                            {
                                // Which tile spawns? Get a random number from 1 to the totalSpawnChance
                                int randomDrop = Random.Range(1, terrainFromName["Soil"].totalSpawnChance);

                                // Loop over each plant in the tarrain dropsDict and stop when randomDrop is <= 0
                                foreach (KeyValuePair<string, int> kvp in terrainFromName["Soil"].dropsDict)
                                {
                                    randomDrop -= kvp.Value;

                                    if (randomDrop <= 0)
                                    {
                                        // Start a cooroutine to change the tile.
                                        StartCoroutine(DelayedPlantSpawn(tilePosition, plantTileFromName[kvp.Key]));
                                        break;
                                    }
                                }
                            }

                            break;
                        case "Barren":
                            // Check if this tile will spawn by rolling under its associated spawnChance
                            if ((Random.Range(0f, 1f)) < terrainFromName["Barren"].spawnRate)
                            {
                                // Which tile spawns? Get a random number from 1 to the totalSpawnChance
                                int randomDrop = Random.Range(1, terrainFromName["Barren"].totalSpawnChance);

                                // Loop over each plant in the tarrain dropsDict and stop when randomDrop is <= 0
                                foreach (KeyValuePair<string, int> kvp in terrainFromName["Barren"].dropsDict)
                                {
                                    randomDrop -= kvp.Value;

                                    if (randomDrop <= 0)
                                    {
                                        // Start a cooroutine to change the tile.
                                        StartCoroutine(DelayedPlantSpawn(tilePosition, plantTileFromName[kvp.Key]));
                                        break;
                                    }
                                }
                            }

                            break;
                    }
                }
            }

            // Fudge with the spawnTimer
            int degrees = Mathf.RoundToInt(Time.time) % 360;
            spawnTimerFudged = 11 * Mathf.Cos(7 * degrees) + spawnTimer;
            yield return new WaitForSeconds(spawnTimerFudged);
        }

    }


    // Delayed Tile Change: Water changes tiles, but tiles further away take longer.
    IEnumerator DelayedPlantSpawn(Vector3Int tilePosition, Tile tile)
    {
        float randomSpawn = Random.Range(1, spawnTimerFudged);

        yield return new WaitForSeconds(randomSpawn);

        // We have a winner! Using the plantTilePalette dictionary, lookup the correct tile and plant it.
        // TODO: Add a spawn delay!
        plantTilemap.SetTile(tilePosition, tile);

        // Now that a plant spawned there, update the plantTileFromPosition dictionary.
        plantTileFromPosition[tilePosition].ThisTile = tile;

        // Be sure to check if this tile can merge!
        // TODO: Add a post-spawn merge check-delay!
        CheckForMerge(tilePosition);
    }


    /* Merge:
     *  - Given a tile position, attempt to use it to merge into a new plant nearby.
     */
    public void CheckForMerge(Vector3Int tilePosition)
    {
        return;
    }


    /* Clear Plants:
     * - Clear the tile under the cursor of plants, if any.
     */
    public void ClearPlants(Vector3Int tilePosition)
    {
        Tile groundUnderPlant = groundTileFromPosition[tilePosition].ThisTile;
        Tile plantToClear = plantTileFromPosition[tilePosition].ThisTile;
        if (plantToClear != null && groundUnderPlant != null)
        {
            plantTilemap.SetTile(tilePosition, plantTileFromName["Empty"]);

            // Now that there's no more plant here, update the plantTileFromPosition dictionary.
            plantTileFromPosition[tilePosition].ThisTile = plantTileFromName["Empty"];
        }
    }
}
