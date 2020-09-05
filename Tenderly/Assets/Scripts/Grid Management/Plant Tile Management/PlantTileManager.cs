using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantTileManager : MonoBehaviour
{
    public static PlantTileManager instance;
    public Tilemap tilemap;

    // plantTiles: A dictionary that keys every vector3int location on the grid to a PlantTile at that location.
    public Dictionary<Vector3Int, PlantTile> plantTiles;
    // plantTileDict: Used for, among other things, getting a tile based on its name.
    public Dictionary<string, Tile> plantTileDict;
    public PlantTilePalette plantTilePalette;

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

        plantTileDict = plantTilePalette.plantTilePalette;
        GetPlantTiles();
    }

    // Use this for initialization
    private void GetPlantTiles()
    {
        plantTiles = new Dictionary<Vector3Int, PlantTile>();
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos)) continue;

            Tile thisTile = plantTileDict["Empty"];
            tilemap.SetTile(pos, thisTile);

            var tile = new PlantTile
            {
                GridVector = pos,
                WorldVector = tilemap.CellToWorld(pos),
                ThisTile = tilemap.GetTile<Tile>(pos),
                TilemapMember = tilemap,
                Name = pos.x + "," + pos.y,

            };

            plantTiles.Add(tile.GridVector, tile);
        }
    }

}
