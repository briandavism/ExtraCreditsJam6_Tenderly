using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMath : MonoBehaviour
{
    // Private variables
    public static Vector3Int[] cubeDirections = {new Vector3Int(0, +1, -1), // NW
                                                new Vector3Int(+1, 0, -1),  // NE
                                                new Vector3Int(+1, -1, 0),  // E
                                                new Vector3Int(0, -1, +1),  // SE
                                                new Vector3Int(-1, 0, +1),  // SW
                                                new Vector3Int(-1, +1, 0)}; // W

    /* Grid:
     *  - Fundamentally, there is a hexagonal grid of tiles.
     *  - Each tile can be some kind of ground type.
     *      - Current assumption is 4 types: Water, Marsh, Soil, Barren.
     *  - Unity uses Odd Offset coordinates, while easy algorithms use cubic.
     */
    /*************************************: CUBE AND ODDR FUNCTIONS :********************************************/
    // Cube coords to odd pointy top offset coords.
    // Taken from https://www.redblobgames.com/grids/hexagons/#conversions-offset
    public static Vector3Int CubeToOddr(Vector3Int cube)
    {
        int col = cube.x + (cube.y - (cube.y & 1)) / 2;
        int row = cube.y;
        return new Vector3Int(col, row, 0);
    }

    // Odd pointy top offset coords to cube coords.
    // Taken from https://www.redblobgames.com/grids/hexagons/#conversions-offset
    public static Vector3Int OddrToCube(Vector3Int oddr)
    {
        int x = oddr.x - (oddr.y - (oddr.y & 1)) / 2;
        int y = oddr.y;
        int z = -x - y;
        return new Vector3Int(x, y, z);
    }

    // Cube directions from a simple int representation of the 6 directions.
    // NW is 0, NE is 1, so on clockwise until W is 5.
    public static Vector3Int CubeDirection(int direction)
    {
        return cubeDirections[direction];
    }

    // Cube neighbors finds the neighbor in a direction from a given Vector3int cube coord.
    // NW is 0, NE is 1, so on clockwise until W is 5.
    public static Vector3Int CubeNeighbor(Vector3Int cube, int direction)
    {
        return cube + CubeDirection(direction);
    }

    // To find the oddr offset neighbor, just covert to cube and use cubic function.
    public static Vector3Int OddrNeighbor(Vector3Int oddr, int direction)
    {
        return CubeToOddr(CubeNeighbor(OddrToCube(oddr), direction));
    }

    // Cube distance: Given two cubes, a and b,find distance, return int.
    public static int CubeDistance(Vector3Int cubeA, Vector3Int cubeB)
    {
        return Mathf.Max(Mathf.Abs(cubeA.x - cubeB.x), Mathf.Abs(cubeA.y - cubeB.y), Mathf.Abs(cubeA.z - cubeB.z));
    }

    // Oddr distance: Given two hexes, a and b, convert from oddr to cube, find distance, return int.
    public static int OddrDistance(Vector3Int oddrA, Vector3Int oddrB)
    {
        Vector3Int cubeA = OddrToCube(oddrA);
        Vector3Int cubeB = OddrToCube(oddrB);

        return CubeDistance(cubeA, cubeB);
    }

    // Cuve Range: Given a Cube coord and a distance, return a list of tile locations that fall within the range.
    public static List<Vector3Int> CubeRange(Vector3Int cube, int distance)
    {
        // List of tiles in Cube coords, not oddr coords.
        List<Vector3Int> results = new List<Vector3Int>();

        // From the center x, search from d distance in the negative and positive directions from x.
        for (int x = -distance; x <= distance; x++)
        {
            for (int y = Mathf.Max(-distance, -x - distance); y <= Mathf.Min(distance, -x + distance); y++)
            {
                int z = -x - y;
                results.Add(cube + new Vector3Int(x, y, z));
            }
        }

        return results;
    }

    // Oddr Range: Given an Oddr coord and a distance, return a list of tile locations that fall within the range.
    public static List<Vector3Int> OddrRange(Vector3Int oddr, int distance)
    {
        // List of tiles in Oddr coords.
        List<Vector3Int> results = new List<Vector3Int>();
        Vector3Int cube = OddrToCube(oddr);

        // From the center x, search from d distance in the negative and positive directions from x.
        for (int x = -distance; x <= distance; x++)
        {
            for (int y = Mathf.Max(-distance, -x - distance); y <= Mathf.Min(distance, -x + distance); y++)
            {
                int z = -x - y;
                results.Add(CubeToOddr(cube + new Vector3Int(x, y, z)));
            }
        }

        return results;

        // DEBUG MATH
        // Distance = 5, x = -4 for this part of the loop.
        // What is the max of (-5 and --4 - 5)? -1.
        // What is the min of (5 and --4 +5) 5?
    }

}
