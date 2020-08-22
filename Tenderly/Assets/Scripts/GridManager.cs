using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridManager : MonoBehaviour
{
    // Public variables
    public Tilemap groundTiles;
    public Tilemap plantTiles;

    // Private variables
    private Vector3Int[] cube_directions = {new Vector3Int(+1, -1, 0), new Vector3Int(+1, 0, -1),
                                            new Vector3Int(0, +1, -1), new Vector3Int(-1, +1, 0),
                                            new Vector3Int(-1, 0, +1), new Vector3Int(0, -1, +1)};
    // Start is called before the first frame update
    void Start()
    {
        Vector3Int target = new Vector3Int(0, 0, 0);

        // Debug.
        Debug.Log("Origin Tile at (0,0): " + groundTiles.GetTile(target));
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                target = new Vector3Int(x, y, 0);
                // Debug.Log("Ground Tile at " + x + "," + y + ": " + groundTiles.GetTile(target));
                // Debug.Log("Plant Tile at " + x + "," + y + ": " + plantTiles.GetTile(target));

            }
        }

        // Cube - Hex conversion testing.
        Vector3Int Hex = new Vector3Int(2, -2, 0);
        Vector3Int Cube = new Vector3Int(3, -1, -2);

        Debug.Log("Hex to Cube. Does " + oddr_to_cube(Hex) + " equal " + Cube + " ?");
        Debug.Log("Cube to Hex. Does " + cube_to_oddr(Cube) + " equal " + Hex + " ?");


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
    Vector3Int cube_to_oddr(Vector3Int cube)
    {
        int col = cube.x + (cube.z - (cube.z & 1)) / 2;
        int row = cube.z;
        return new Vector3Int(col, row, 0);
    }

    // Odd pointy top offset coords to cube coords.
    // Taken from https://www.redblobgames.com/grids/hexagons/#conversions-offset
    Vector3Int oddr_to_cube(Vector3Int hex)
    {
        int x = hex.x - (hex.y - (hex.y & 1)) / 2;
        int z = hex.y;
        int y = -x - z;
        return new Vector3Int(x, y, z);
    }

    
    // Cube directions from a simple int representation of the 6 directions.
    // NW is 0, NE is 1, so on clockwise until W is 5.
    Vector3Int cube_direction(int direction)
    {
        return cube_directions[direction];
    }

    // Cube neighbors finds the neighbor in a direction from a given Vector3int cube coord.
    Vector3Int cube_neighbor(Vector3Int cube, int direction)
    {
        return cube + cube_direction(direction);
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
}
