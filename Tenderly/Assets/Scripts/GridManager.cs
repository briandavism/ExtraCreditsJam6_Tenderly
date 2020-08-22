using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;


public class GridManager : MonoBehaviour
{
    // Public variables
    public Tilemap groundTiles;
    public Tilemap plantTiles;

    // Private variables
    private Vector3Int[] cubeDirections = {new Vector3Int(0, +1, -1),  // NW
                                            new Vector3Int(+1, 0, -1),  // NE
                                            new Vector3Int(+1, -1, 0),  // E
                                            new Vector3Int(0, -1, +1),  // SE
                                            new Vector3Int(-1, 0, +1),  // SW
                                            new Vector3Int(-1, +1, 0)}; // W
                                            
                                            
    // Start is called before the first frame update
    void Start()
    {
        // Call dbug.
        Dbug();

        


    }

    // Update is called once per frame
    void Update()
    {

    }

    /* Grid:
     *  - Fundamentally, there is a hexagonal grid of tiles.
     *  - Each tile can be some kind of ground type.
     *      - Current assumption is 4 types: Water, Marsh, Soil, Barren.
     *  - Unity uses Odd Offset coordinates, while easy algorithms use cubic.
     */

    // Cube coords to odd pointy top offset coords.
    // Taken from https://www.redblobgames.com/grids/hexagons/#conversions-offset
    public Vector3Int CubeToOddr(Vector3Int cube)
    {
        int col = cube.x + (cube.y - (cube.y & 1)) / 2;
        int row = cube.y;
        return new Vector3Int(col, row, 0);
    }

    // Odd pointy top offset coords to cube coords.
    // Taken from https://www.redblobgames.com/grids/hexagons/#conversions-offset
    public Vector3Int OddrToCube(Vector3Int oddr)
    {
        int x = oddr.x - (oddr.y - (oddr.y & 1)) / 2;
        int y = oddr.y;
        int z = -x - y;
        return new Vector3Int(x, y, z);
    }
    
    // Cube directions from a simple int representation of the 6 directions.
    // NW is 0, NE is 1, so on clockwise until W is 5.
    public Vector3Int CubeDirection(int direction)
    {
        return cubeDirections[direction];
    }

    // Cube neighbors finds the neighbor in a direction from a given Vector3int cube coord.
    // NW is 0, NE is 1, so on clockwise until W is 5.
    public Vector3Int CubeNeighbor(Vector3Int cube, int direction)
    {
        return cube + CubeDirection(direction);
    }

    // To find the oddr offset neighbor, just covert to cube and use cubic function.
    public Vector3Int OddrNeighbor(Vector3Int oddr, int direction)
    {
        return CubeToOddr(CubeNeighbor(OddrToCube(oddr), direction));
    }

    // Cube distance: Given two cubes, a and b,find distance, return int.
    public int CubeDistance(Vector3Int cubeA, Vector3Int cubeB)
    {
        return Mathf.Max(Mathf.Abs(cubeA.x - cubeB.x), Mathf.Abs(cubeA.y - cubeB.y), Mathf.Abs(cubeA.z - cubeB.z));
    }

    // Oddr distance: Given two hexes, a and b, convert from oddr to cube, find distance, return int.
    public int OddrDistance(Vector3Int oddrA, Vector3Int oddrB)
    {
        Vector3Int cubeA = OddrToCube(oddrA);
        Vector3Int cubeB = OddrToCube(oddrB);

        return CubeDistance(cubeA, cubeB);
    }

    // Cuve Range: Given a Cube coord and a distance, return an array of tiles that fall within the range.
    public List<Vector3Int> CubeRange(Vector3Int cube, int distance)
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

    // Oddr Range: Given an Oddr coord and a distance, return an array of tiles that fall within the range.
    public List<Vector3Int> OddrRange(Vector3Int oddr, int distance)
    {
        // List of tiles in Oddr coords.
        List<Vector3Int> results = new List<Vector3Int>();
        Vector3Int cube = OddrToCube(oddr);

        // From the center x, search from d distance in the negative and positive directions from x.
        for (int x = -distance; x <= distance; x++)
        {
            for (int y = Mathf.Max(-distance, -x - distance); y <= Mathf.Min(distance, x + distance); y++)
            {
                int z = -x - y;
                results.Add(CubeToOddr(cube + new Vector3Int(x, y, z)));
            }
        }

        return results;
    }

    /* Water:
     *  - In the beginning, all is barren.
     *  - When water is placed, find all tiles within a certain radius and enque them.
     *      - Potentially, we can queue the nearest neighbors and only enque more neighbors when a 
     *          tile becomes marsh.
     *  - 
     */





    /* Growth Phase:
     *  - For each tile, add a random 4 digit decimal from 0 to 9999 to the Tier number of the object.
     *  - Sort the objects from lowest to highest.
     *  - For each object in this list, check to see if something happens.
     *      - Tier 0: Empty tile spawn new objects
     *          - Water: Lilypad
     *          - Marsh: Soft Rush
     *          - Soil: Fiddlehead
     *      - Tier 1:
     *          - Water: Lilypad * 3 = Water Lily
     *          - Marsh: Soft Rush * 3 = Bullrush
     *          - Soil: Fiddlehead * 3 = Bracken
     *  - When something happens, 
     *  
     *  
     *  - For each tier, keep a queue of tiles belonging to that tier.
     *  - For each tier, randomly iterate over each tile in that tier queue to check for an event. 
     *      - 
     *  - When a tile leaves a tier due to an event, add it to the appropriate tier's list and remove
     *      it from its previous tier.
     *      
     *  - Am I part of a recipie?
     *      - Create an empty list of completed recipies.
     *      - Create an empty list of partial recipies.
     *      - Populate a list of initial recipies that require me.
     *      - Enque my neighbors randomly.
     *      - Look at my next neighbor. 
     *          - For every partial recipie, does the neighbor complete it?
     *              - If it does, make a new completed recipe out of the partial recipie's tiles and
     *                  this new neighbor tile. Don't remove the partial recipie though.
     *          - For every initial recipe, does the neighbor also help fulfill it?
     *              - If it does, make a new partial recipe out of the neighbor and my tiles.
     *                  Don't remove the initial recipe though.
     *              - If it doesn't, this neighbor can't be used to make anything with me. Move on.
     *      - When no neighbors remain, were there completed recipies?
     *          - If yes, pick one of the completed recipies, at random or by some other method.
     *              Then pick one of the tiles at random to become the new tile, based on the recipe.
     *          - If not, this tile isn't the center of a recipe.
     *              
     *                  
     */


    // Is this tile the center of a recipie? If it is, grow it into something!
    void GrowTile()//??? tile)
    {
        
    }

    // Debug and testing misc function.
    private void Dbug()
    {
        Vector3Int origin = new Vector3Int(0, 0, 0);
        Vector3Int target = new Vector3Int(2, 2, 0);
        Vector3Int targetCube = new Vector3Int(2, 2, -4);


        // Neighbor testing: Keep in mind, the range function also captures yourself!
        List<Vector3Int> cubeNeighbors = CubeRange(origin, 0);
        List<Vector3Int> oddrNeighbors = OddrRange(origin, 0);
        Assert.AreEqual(cubeNeighbors.Count, 1);
        Assert.AreEqual(oddrNeighbors.Count, 1);

        List<Vector3Int> cubeNeighbors2 = CubeRange(targetCube, 1);
        List<Vector3Int> oddrNeighbors2 = OddrRange(target, 2);
        List<Vector3Int> oddrNeighbors3 = OddrRange(target, 3);
        Assert.AreEqual(cubeNeighbors2.Count, 7);
        Assert.AreEqual(oddrNeighbors2.Count, 19);
        Assert.AreEqual(oddrNeighbors3.Count, 37);


        // For loop
        /* 
        Debug.Log("Center at: " + origin);
        Debug.Log("Neighbor count: " + cubeNeighbors2.Count);
        foreach (Vector3Int v in cubeNeighbors2)
        {
            Debug.Log("Neighbor at: " + v);
        } 
        * /


        // Distance testing:
        /* 
        Assert.AreEqual(OddrDistance(origin, target), 3);
        * Done, works great.*/
        Assert.AreEqual(CubeDistance(origin, targetCube), 4);

        // Neighbors debug.
        /*
        for (int d = 0; d <= 5; d++)
        {
            target = oddr_offset_neighbor(origin, d);
            Debug.Log("Ground neighbor of target at " + origin + " in " + cube_to_oddr(cube_direction(d)) + ": " 
                + groundTiles.GetTile(target));
            Debug.Log("Plant neighbor of target at " + origin + " in " + cube_to_oddr(cube_direction(d)) + ": " 
                + plantTiles.GetTile(target));

        } 
        * Done, works good. */

        // Cube - Hex conversion testing.
        /*
        Vector3Int Hex = new Vector3Int(2, -2, 0);
        Vector3Int Cube = new Vector3Int(3, -2, -1);
        Debug.Log("Cube to Hex. Does " + Hex + " equal " + cube_to_oddr(Cube) + " ?");
        Debug.Log("Hex to Cube. Does " + Cube + " equal " + oddr_to_cube(Hex) + " ?");
        * Done, works good. */
    }
}
