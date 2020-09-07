using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Terrain
{
    public string name;
    public float spawnRate;
    public Dictionary<string, int> dropsDict;
    public Tile tile;
}
