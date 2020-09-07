using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Taken from template code found: https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
public class GroundTile : ScriptableObject
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


    // TODO: Add terrain?
    public Terrain Terrain { get; set; }
}
