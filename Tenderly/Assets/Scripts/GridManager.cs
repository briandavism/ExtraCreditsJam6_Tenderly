using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

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
