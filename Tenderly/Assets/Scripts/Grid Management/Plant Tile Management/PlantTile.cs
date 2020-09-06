using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Taken from template code found: https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
public class PlantTile : ScriptableObject
{
    public Vector3Int GridVector { get; set; }
    public Vector3 WorldVector { get; set; }
    public Tile ThisTile { get; set; }
    public Tilemap TilemapMember { get; set; }
    public string Name { get; set; }
    public Plant Plant { get; set; }

    // What unique data does a plant need to store?
}
