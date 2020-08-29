using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;

public class Debugger : MonoBehaviour
{
    public GroundTile[] groundTilePallete;

    // Start is called before the first frame update
    void Start()
    {
        //DebugTiles();
        //DebugHexMath();
    }


    void DebugTiles()
    {

        // Can we access the grid?
        //Debug.Log(FindObjectOfType<Grid>().ToString());
        // Can we access the tilemap?
        Tilemap testTilemap = FindObjectOfType<Grid>().GetComponentInChildren<Tilemap>();
        //Debug.Log(testTilemap.ToString());
        // Can we tell the tilemap to spawn a tile?
        

        /* // Does neighbor range still work? // Status: it does
        List<Vector3Int> tilesInRange = HexMath.OddrRange(origin, 3);
        foreach (Vector3Int v in tilesInRange)
        {
            // Turn each into soil!
            testTilemap.SetTile(v, FindObjectOfType<GroundTiles>().GetGroundTiles()[2]);
        } */

    }

    void DebugHexMath()
    {
        Vector3Int origin = new Vector3Int(0, 0, 0);
        Vector3Int target = new Vector3Int(2, 2, 0);
        Vector3Int targetCube = new Vector3Int(1, -3, 2);


        // Distance testing: Oddr then Cube
        Assert.AreEqual(HexMath.OddrDistance(origin, origin), 0);
        Assert.AreEqual(HexMath.OddrDistance(origin, origin + HexMath.CubeDirection(0)), 1);
        Assert.AreEqual(HexMath.OddrDistance(origin, origin + HexMath.CubeDirection(0)*2), 2);
        Assert.AreEqual(HexMath.OddrDistance(origin, target), 3);
        Assert.AreEqual(HexMath.OddrDistance(origin, target + HexMath.CubeDirection(0)), 2);
        Assert.AreEqual(HexMath.OddrDistance(origin, new Vector3Int(-6, -6, 0)), 9);

        Assert.AreEqual(HexMath.CubeDistance(origin, origin), 0);
        Assert.AreEqual(HexMath.CubeDistance(origin, origin + HexMath.CubeDirection(0) * 2), 2);
        Assert.AreEqual(HexMath.CubeDistance(origin, new Vector3Int(0, 0, 0)), 0);
        Assert.AreEqual(HexMath.CubeDistance(origin, targetCube), 3);
        Assert.AreEqual(HexMath.CubeDistance(origin, target + HexMath.CubeDirection(0)), 2);
        Assert.AreEqual(HexMath.CubeDistance(origin, new Vector3Int(-1, 4, -3)), 4);

        // Cube - Hex conversion testing.
        Vector3Int Hex = new Vector3Int(1, 1, 0);
        Vector3Int Cube = new Vector3Int(1, -2, 1);
        Assert.AreEqual(Hex, HexMath.CubeToOddr(Cube));
        Assert.AreEqual(Cube, HexMath.OddrToCube(Hex));

        Hex = new Vector3Int(-2, -2, 0);
        Cube = new Vector3Int(-1, 3, -2);
        Assert.AreEqual(Hex, HexMath.CubeToOddr(Cube));
        Assert.AreEqual(Cube, HexMath.OddrToCube(Hex));

        Hex = new Vector3Int(1, -2, 0);
        Cube = new Vector3Int(2, 0, -2);
        Assert.AreEqual(Hex, HexMath.CubeToOddr(Cube));
        Assert.AreEqual(Cube, HexMath.OddrToCube(Hex));

        Hex = new Vector3Int(-2, 2, 0);
        Cube = new Vector3Int(-3, 1, 2);
        Assert.AreEqual(Hex, HexMath.CubeToOddr(Cube));
        Assert.AreEqual(Cube, HexMath.OddrToCube(Hex));

        /* Neighbor Testing: Testing around Origin: (0, 0, 0).
         *  - Radius 0. It's a marsh, so a neighbor test of radius 0 should simply return one marsh.
         *  - Radius 1. Should add 6 soil tiles and count equal 7
         *  - Radius 2. Should add 12 barren tiles and count equal 19.
         *  No tiles should be water for this setup.
         */
        NeighborTestHelper(0);
        NeighborTestHelper(1);
        NeighborTestHelper(2);
        NeighborTestHelper(3);

    }

    // Neighbor Test Helper
    void NeighborTestHelper(int radius)
    {
        int neighborCount = 1;
        for (int c = 0; c <= radius; c++)
        {
            neighborCount += c * 6;
        }

        Tilemap tilemap = GameObject.FindGameObjectWithTag("testTilemap").GetComponent<Tilemap>();
        List<Vector3Int> neighborTiles = new List<Vector3Int>();
        neighborTiles = HexMath.OddrRange(new Vector3Int(0,0,0), radius);
        Dictionary<string, int> neighborTestDict = new Dictionary<string, int>();
        Assert.AreEqual(neighborTiles.Count, neighborCount);

        for (int i = 0; i < neighborTiles.Count; i++)
        {
            if (neighborTestDict.ContainsKey(tilemap.GetTile(neighborTiles[i]).name))
            {
                neighborTestDict[tilemap.GetTile(neighborTiles[i]).name]++;
            }
            else
            {
                neighborTestDict.Add(tilemap.GetTile(neighborTiles[i]).name, 1);
            }
        }
        foreach (KeyValuePair<string, int> kvp in neighborTestDict)
        {
            Debug.Log(kvp.Key + ": " + kvp.Value);
        }
    }
}
